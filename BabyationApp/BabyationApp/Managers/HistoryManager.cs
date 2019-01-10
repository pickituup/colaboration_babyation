using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Models;
using BabyationApp.Helpers;
using System.Collections.ObjectModel;
using BabyationApp.DataObjects;
using BabyationApp.Extensions;

namespace BabyationApp.Managers
{
    public enum InventoryFilter
    {
        All,
        Fridge,
        Freezer,
        Other
    }

    public class HistoryManager
    {
        private static HistoryManager _instance = null;
        private List<HistoryModel> _inventory = new List<HistoryModel>();
        private List<HistoryModel> _sessions = new List<HistoryModel>();

        public static HistoryManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HistoryManager();
                }
                return _instance;
            }
        }

        private HistoryManager()
        {

        }

        public void Reset()
        {
            _inventory.Clear();
            _sessions.Clear();
        }

        public async Task Initialize()
        {
            await Sync();
        }

        public async Task Sync(string profileId = null)
        {
            HistoryModel historyModel;
            DataManager dataManager = DataManager.Instance;
            IEnumerable<HistoricalSession> historicalSessions;

            historicalSessions = await dataManager.GetHistoricalSessions(profileId);

            foreach (HistoricalSession historicalSession in historicalSessions)
            {
                var existing = _sessions.Where(e => e.Id == historicalSession.Id).FirstOrDefault();

                if (existing == null)
                {
                    historyModel = new HistoryModel()
                    {
                        Id = historicalSession.Id,
                        SessionType = (SessionType)historicalSession.Type,
                        StartTime = historicalSession.SessionStart.ToLocalTime(),
                        EndTime = historicalSession.SessionEnd.ToLocalTime(),
                        LeftBreastStartTime = historicalSession.LeftBreastStart.ToLocalTime(),
                        LeftBreastEndTime = historicalSession.LeftBreastEnd.ToLocalTime(),
                        RightBreastStartTime = historicalSession.RightBreastStart.ToLocalTime(),
                        RightBreastEndTime = historicalSession.RightBreastEnd.ToLocalTime(),
                        LeftBreastMilkVolume = historicalSession.LeftBreastVolume,
                        RightBreastMilkVolume = historicalSession.RightBreastVolume,
                        TotalMilkVolume = historicalSession.TotalVolume,
                        ExpirationTime = historicalSession.Expires.ToLocalTime(),
                        IsUsed = (historicalSession.Available == 0) ? true : false,
                        IsPreferred = (historicalSession.Preferred == 1) ? true : false,
                        Description = historicalSession.Notes,
                        Storage = (StorageType)historicalSession.StorageType,
                        FeedByProfileId = historicalSession.FeedByProfileId
                    };

                    if (historyModel.SessionType == SessionType.Pump)
                    {
                        historyModel.Milk = MilkType.BreastMilk;
                    }

                    // Sanity check the data
                    if ((historyModel.StartTime != DateTime.MinValue) &&
                        (historyModel.StartTime != ExtensionMethods.DefaultDateTime))
                    {
                        _sessions.Add(historyModel);

                        if (!historyModel.IsUsed)
                        {
                            _inventory.Add(historyModel);
                        }
                    }
                }
            }
        }

        private void HistoryModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsPreferred")
            {
                // Update cloud
            }
        }

        public IEnumerable<HistoryModel> GetInventory(InventoryFilter filter)
        {
            IEnumerable<HistoryModel> inventory = null;

            switch (filter)
            {
                case InventoryFilter.All:
                    inventory = _inventory;
                    break;
                case InventoryFilter.Freezer:
                    inventory = _inventory.Where(i => i.Storage == StorageType.Freezer);
                    break;
                case InventoryFilter.Fridge:
                    inventory = _inventory.Where(i => i.Storage == StorageType.Fridge);
                    break;
                default:
                    inventory = _inventory.Where(i => i.Storage != StorageType.Fridge && i.Storage != StorageType.Freezer);
                    break;
            }

            return inventory;
        }

        public async void RemoveInventory(HistoryModel inventory)
        {
            HistoricalSession historicalSession;
            DataManager dataManager = DataManager.Instance;

            _inventory.Remove(inventory);

            inventory.IsUsed = true;

            // Update cloud
            historicalSession = new HistoricalSession()
            {
                Id = inventory.Id,
                Type = (byte)inventory.SessionType,
                SessionStart = inventory.StartTime.ToUniversalTime(),
                SessionEnd = inventory.EndTime.ToUniversalTime(),
                LeftBreastStart = inventory.LeftBreastStartTime.ToUniversalTime(),
                LeftBreastEnd = inventory.LeftBreastEndTime.ToUniversalTime(),
                RightBreastStart = inventory.RightBreastStartTime.ToUniversalTime(),
                RightBreastEnd = inventory.RightBreastEndTime.ToUniversalTime(),
                LeftBreastVolume = inventory.LeftBreastMilkVolume,
                RightBreastVolume = inventory.RightBreastMilkVolume,
                TotalVolume = inventory.TotalMilkVolume,
                Expires = inventory.ExpirationTime.ToUniversalTime(),
                Available = inventory.IsUsed ? (byte)0 : (byte)1,
                Preferred = inventory.IsPreferred ? (byte)1 : (byte)0,
                Notes = inventory.Description,
                StorageType = (byte)inventory.Storage,
                ChildId = inventory.ChildID
            };

            await dataManager.AddHistoricalSession(historicalSession);
        }

        public HistoryModel CreateSession(SessionType type)
        {
            HistoryModel historyModel;
            bool isUsed = true;
            MilkType milk = MilkType.Formula;

            if (type == SessionType.Pump)
            {
                isUsed = false;
                milk = MilkType.BreastMilk;
            }

            historyModel = new HistoryModel()
            {
                Id = Guid.NewGuid().ToString(),
                SessionType = type,
                IsUsed = isUsed,
                Storage = StorageType.Unspecified,
                Milk = milk
            };
            historyModel.PropertyChanged += HistoryModel_PropertyChanged;

            return historyModel;
        }

        public async void AddSession(HistoryModel session)
        {
            HistoricalSession historicalSession;
            DataManager dataManager = DataManager.Instance;
            DateTime startTime = ExtensionMethods.DefaultDateTime;
            DateTime endTime = ExtensionMethods.DefaultDateTime;
            var existingSession = _sessions.FirstOrDefault(e => e.Id == session.Id || e.StartTime == session.StartTime);

            if (existingSession == null)
            {
                if (session.StartTime == ExtensionMethods.DefaultDateTime)
                {
                    if (session.LeftBreastStartTime != ExtensionMethods.DefaultDateTime)
                    {
                        startTime = session.LeftBreastStartTime;
                    }

                    if (session.RightBreastStartTime != ExtensionMethods.DefaultDateTime)
                    {
                        if (startTime != ExtensionMethods.DefaultDateTime)
                        {
                            if (session.RightBreastStartTime < startTime)
                            {
                                startTime = session.RightBreastStartTime;
                            }
                        }
                        else
                        {
                            startTime = session.RightBreastStartTime;
                        }
                    }

                    session.StartTime = startTime;
                }

                if (session.EndTime == ExtensionMethods.DefaultDateTime)
                {
                    if (session.LeftBreastEndTime != ExtensionMethods.DefaultDateTime)
                    {
                        endTime = session.LeftBreastEndTime;
                    }

                    if (session.RightBreastEndTime != ExtensionMethods.DefaultDateTime)
                    {
                        if (endTime != ExtensionMethods.DefaultDateTime)
                        {
                            if (session.RightBreastEndTime > endTime)
                            {
                                endTime = session.RightBreastEndTime;
                            }
                        }
                        else
                        {
                            endTime = session.RightBreastStartTime;
                        }
                    }

                    session.EndTime = endTime;
                }

                if ((session.StartTime != ExtensionMethods.DefaultDateTime) &&
                    (session.EndTime != ExtensionMethods.DefaultDateTime))
                {
                    // Convert time to local
                    if (session.StartTime.Kind == DateTimeKind.Utc)
                    {
                        session.StartTime = session.StartTime.ToLocalTime();
                        session.EndTime = session.EndTime.ToLocalTime();
                        session.RightBreastStartTime = session.RightBreastStartTime.ToLocalTime();
                        session.RightBreastEndTime = session.RightBreastEndTime.ToLocalTime();
                        session.LeftBreastStartTime = session.LeftBreastStartTime.ToLocalTime();
                        session.LeftBreastEndTime = session.LeftBreastEndTime.ToLocalTime();
                    }

                    // Set expiration to 30 days?
                    session.ExpirationTime = session.StartTime + new TimeSpan(30, 0, 0, 0);

                    // Add to local cache
                    _sessions.Add(session);

                    if (!session.IsUsed)
                    {
                        _inventory.Add(session);
                    }

                    // Save to database and cloud
                    historicalSession = new HistoricalSession()
                    {
                        Id = session.Id,
                        Type = (byte)session.SessionType,
                        SessionStart = session.StartTime.ToUniversalTime(),
                        SessionEnd = session.EndTime.ToUniversalTime(),
                        LeftBreastStart = session.LeftBreastStartTime.ToUniversalTime(),
                        LeftBreastEnd = session.LeftBreastEndTime.ToUniversalTime(),
                        RightBreastStart = session.RightBreastStartTime.ToUniversalTime(),
                        RightBreastEnd = session.RightBreastEndTime.ToUniversalTime(),
                        LeftBreastVolume = session.LeftBreastMilkVolume,
                        RightBreastVolume = session.RightBreastMilkVolume,
                        TotalVolume = session.TotalMilkVolume,
                        Expires = session.ExpirationTime.ToUniversalTime(),
                        Available = session.IsUsed ? (byte)0 : (byte)1,
                        Preferred = session.IsPreferred ? (byte)1 : (byte)0,
                        Notes = session.Description,
                        StorageType = (byte)session.Storage,
                        ChildId = session.ChildID,
                        FeedByProfileId = session.FeedByProfileId
                    };

                    await dataManager.AddHistoricalSession(historicalSession);
                }
            }
        }

        public DateTime GetMinDate()
        {
            DateTime min;

            if (_sessions.Count > 0)
            {
                min = _sessions.Select(s => s.StartTime).Min();
                min = new DateTime(min.Year, min.Month, min.Day, 0, 0, 0, DateTimeKind.Local);
            }
            else
            {
                min = DateTime.Now;
            }

            return min;
        }

        public DateTime GetMaxDate()
        {
            DateTime max;

            if (_sessions.Count > 0)
            {
                max = _sessions.Select(s => s.StartTime).Max();
                max = new DateTime(max.Year, max.Month, max.Day, 23, 59, 59, DateTimeKind.Local);
            }
            else
            {
                max = DateTime.Now;
            }

            return max;
        }

        public IEnumerable<HistoryModel> GetDay(DateTime start)
        {
            DateTime end = start + TimeSpan.FromDays(1);

            IEnumerable<HistoryModel> sessions = from s in _sessions
                                                 where ((s.StartTime >= start) && (s.EndTime <= end))
                                                 orderby (s.StartTime)
                                                 select s;

            return sessions;
        }

        public List<HistoryRangeModel> GetWeek(DateTime start)
        {
            if (null == ProfileManager.Instance.CurrentProfile)
                return null;

            if (DateTime.MinValue < start && start < DateTime.MaxValue)
            {
                // Always align with start of the week
                start = start.DayOfWeek != DayOfWeek.Sunday ? start.StartOfWeek(DayOfWeek.Sunday) : start;

                // Always check for existing bounds
                if (start < GetMinDate())
                    start = GetMinDate();

                if (start > GetMaxDate())
                    start = GetMaxDate();

                DateTime end = start + TimeSpan.FromDays(7);

                IEnumerable<HistoryModel> sessions = from s in _sessions
                                                     where ((s.StartTime >= start) && (s.EndTime <= end))
                                                     orderby (s.StartTime)
                                                     select s;

                List<HistoryRangeModel> ranges = new List<HistoryRangeModel>();

                // Total summary:
                HistoryRangeModel range = new HistoryRangeModel();
                range.Process(sessions, start, Timeframe.Week, null);
                ranges.Add(range);

                if (0 < ProfileManager.Instance.CurrentProfile?.Babies?.Count)
                {
                    // Summary for each child:
                    foreach (BabyModel baby in ProfileManager.Instance.CurrentProfile.Babies)
                    {
                        range = new HistoryRangeModel();

                        range.Process(sessions, start, Timeframe.Week, baby.Id);

                        ranges.Add(range);
                    }
                }
                return ranges;
            }

            return null;
        }

        public HistoryRangeModel GetMonth(DateTime start)
        {
            HistoryRangeModel range = new HistoryRangeModel();
            int daysInMonth = DateTime.DaysInMonth(start.Year, start.Month);
            DateTime end = start + TimeSpan.FromDays(daysInMonth);

            IEnumerable<HistoryModel> sessions = from s in _sessions
                                                 where ((s.StartTime >= start) && (s.EndTime <= end))
                                                 select s;
            range.Process(sessions, start, Timeframe.Month, null);

            return range;
        }
    }
}
