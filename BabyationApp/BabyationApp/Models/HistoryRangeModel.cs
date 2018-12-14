using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.DataObjects;
using System.Runtime.CompilerServices;
using System.Globalization;
using BabyationApp.Extensions;
using System.Diagnostics;
using Xamarin.Forms;

namespace BabyationApp.Models
{
    public enum Timeframe
    {
        Week,
        Month
    }

    public class HistoryRangeModel : ModelItemBase
    {
        //static readonly List<String> _weekRangeNames = new List<string>() { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };
        //static readonly List<int> _dayOfWeekToBin = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };
        private List<String> _rangeNames;
        private double[,] _rangeValues;
        private List<double> _maxRangeValue = new List<double>(new double[(int)SessionType.Max]);
        private List<double> _total = new List<double>(new double[(int)SessionType.Max]);
        private List<double> _totalAverage = new List<double>(new double[(int)SessionType.Max]);
        private List<double> _leftAverage = new List<double>(new double[(int)SessionType.Max]);
        private List<double> _rightAverage = new List<double>(new double[(int)SessionType.Max]);
        private double _totalBreast = 0.0;
        private double _totalFormula = 0.0;

        public void Process(IEnumerable<HistoryModel> sessions, DateTime start, Timeframe timeframe, string childID)
        {
            int daysInMonth;
            int bins;
            int bin;
            string month;
            int range;
            int left;
            int rangeStart;
            int rangeEnd;
            int[] binStart;
            int[] binEnd;
            int iter;
            TimeSpan totalTime;
            TimeSpan leftTime;
            TimeSpan rightTime;
            int[] totalSessions = new int[(int)SessionType.Max];  

            if (timeframe == Timeframe.Week)
            {
                // Week

                // Start of the week correction:
                start = start.DayOfWeek != DayOfWeek.Sunday ? start.StartOfWeek(DayOfWeek.Sunday) : start;

                DateTime end = start + TimeSpan.FromDays(6);
                RangeInfo = String.Format("{0} {1} - {2} {3}, {4}", 
                                          start.ToString("MMM", CultureInfo.CurrentCulture), 
                                          start.Day.ToString(),
                                          end.ToString("MMM", CultureInfo.CurrentCulture),
                                          end.Day.ToString(), end.Year.ToString());

                bins = 7;
                if( null == _rangeNames )
                {
                    _rangeNames = new List<string>();
                }
                _rangeNames.Clear();

                // Prepare formatted (day name | month/day) text:
                //
                DateTime dateIterator = start;
                int tMonth = dateIterator.Month;
                int tDay = dateIterator.Day;
                CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                string[] dayNames = cultureInfo.DateTimeFormat.AbbreviatedDayNames;
                for (int i = 0; i < bins; i++)
                {
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        _rangeNames.Add(String.Format("{0}\n{1}/{2}", dayNames[(int)dateIterator.DayOfWeek].ToUpper(), tMonth, tDay));
                    }
                    else
                    {
                        _rangeNames.Add(String.Format("{0}\n\n{1}/{2}", dayNames[(int)dateIterator.DayOfWeek].ToUpper(), tMonth, tDay));
                    }

                    dateIterator = dateIterator.AddDays(1);
                    tMonth = dateIterator.Month;
                    tDay = dateIterator.Day;
                }

                _rangeValues = new double[(int)SessionType.Max, bins];

                foreach (HistoryModel session in sessions)
                {
                    int dayOfWeek = (int)session.StartTime.DayOfWeek;

                    if ( (null == childID) || (null != childID && (session?.ChildID?.Equals(childID) ?? false)) )
                    {
                        if(SessionType.Nurse == session.SessionType)
                        {
                            // Nursing is minutes based
                            totalTime = session.EndTime - session.StartTime;

                            _total[(int)SessionType.Nurse] += totalTime.TotalMinutes;
                            _rangeValues[(int)SessionType.Nurse, (int)dayOfWeek] += totalTime.TotalMinutes;

                            UpdateMaxRangeFor(SessionType.Nurse, dayOfWeek);
                        }
                        else if(SessionType.BottleFeed == session.SessionType)
                        {
                            if (MilkType.BreastMilk == session.Milk)
                            {
                                _totalBreast += session.TotalMilkVolume;
                                _total[(int)SessionType.Breastmilk] += session.TotalMilkVolume;
                                _rangeValues[(int)SessionType.Breastmilk, dayOfWeek] += session.TotalMilkVolume;

                                UpdateMaxRangeFor(SessionType.Breastmilk, dayOfWeek);
                            }
                            else if (MilkType.Formula == session.Milk)
                            {
                                _totalFormula += session.TotalMilkVolume;
                                _total[(int)SessionType.Formula] += session.TotalMilkVolume;
                                _rangeValues[(int)SessionType.Formula, dayOfWeek] += session.TotalMilkVolume;

                                UpdateMaxRangeFor(SessionType.Formula, dayOfWeek);
                            }

                            // Totals for Bottle:
                            _total[(int)SessionType.BottleFeed] += session.TotalMilkVolume;
                            _rangeValues[(int)SessionType.BottleFeed, dayOfWeek] += session.TotalMilkVolume;

                            UpdateMaxRangeFor(SessionType.BottleFeed, dayOfWeek);
                        }
                        else // All other like pump
                        {
                            // All the rest are volume based
                            _total[(int)session.SessionType] += session.TotalMilkVolume;
                            _rangeValues[(int)session.SessionType, (int)dayOfWeek] += session.TotalMilkVolume;

                            UpdateMaxRangeFor(session.SessionType, dayOfWeek);
                        }

                        totalSessions[(int)session.SessionType]++;
                    }
                }
            }
            else
            {
                // Month
                RangeInfo = start.ToString("MMMM, yyyy");
                daysInMonth = DateTime.DaysInMonth(start.Year, start.Month);
                bins = 4;
                _rangeNames = new List<string>(new string[bins]);
                binStart = new int[bins];
                binEnd = new int[bins];
                month = start.ToString("MMM");
                range = daysInMonth / bins;
                left = daysInMonth % bins;
                rangeStart = 1;
                _rangeValues = new double[(int)SessionType.Max, bins];

                for (iter = 0; iter < bins; iter++)
                {
                    rangeEnd = rangeStart + range - 1;

                    if (left > 0)
                    {
                        rangeEnd++;
                        left--;
                    }

                    _rangeNames[iter] = string.Format("{0}{1}-{2}", month, rangeStart, rangeEnd);

                    binStart[iter] = rangeStart;
                    binEnd[iter] = rangeEnd;

                    rangeStart = rangeEnd + 1;
                }

                foreach (HistoryModel session in sessions)
                {
                    bin = 0;

                    for (iter = 0; iter < bins; iter++)
                    {
                        if (session.StartTime.Day > binEnd[iter])
                        {
                            bin++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (session.SessionType == SessionType.Nurse)
                    {
                        // Nursing is hour based
                        totalTime = session.EndTime - session.StartTime;
                        leftTime = session.LeftBreastEndTime - session.LeftBreastStartTime;
                        rightTime = session.RightBreastEndTime - session.RightBreastStartTime;
                        _rangeValues[(int)session.SessionType, bin] += totalTime.TotalHours;
                        _total[(int)session.SessionType] += totalTime.TotalHours;
                        _totalAverage[(int)session.SessionType] += totalTime.TotalHours;
                        _leftAverage[(int)session.SessionType] += leftTime.TotalHours;
                        _rightAverage[(int)session.SessionType] += rightTime.TotalHours;
                    }
                    else
                    {
                        // All the rest are volume based
                        _rangeValues[(int)session.SessionType, bin] += session.TotalMilkVolume;
                        _total[(int)session.SessionType] += session.TotalMilkVolume;
                        _totalAverage[(int)session.SessionType] += session.TotalMilkVolume;
                        _leftAverage[(int)session.SessionType] += session.LeftBreastMilkVolume;
                        _rightAverage[(int)session.SessionType] += session.RightBreastMilkVolume;
                    }

                    if (_rangeValues[(int)session.SessionType, bin] > _maxRangeValue[(int)session.SessionType])
                    {
                        _maxRangeValue[(int)session.SessionType] = _rangeValues[(int)session.SessionType, bin];
                    }

                    totalSessions[(int)session.SessionType]++;
                }

                for (iter = 0; iter < (int)SessionType.Max; iter++)
                {
                    if (totalSessions[iter] != 0)
                    {
                        _totalAverage[iter] /= totalSessions[iter];
                        _leftAverage[iter] /= totalSessions[iter];
                        _rightAverage[iter] /= totalSessions[iter];
                    }
                }
            }
        }

        private void UpdateMaxRangeFor(SessionType type, int day)
        {
            if (_rangeValues[(int)type, day] > _maxRangeValue[(int)type])
            {
                _maxRangeValue[(int)type] = _rangeValues[(int)type, day];
            }
        }

        private String _rangeInfo;

        public String RangeInfo
        {
            get { return _rangeInfo; }
            set => SetPropertyChanged(ref _rangeInfo, value);
        }

        public List<double> TotalAverage
        {
            get
            {
                return _totalAverage;
            }
        }

        public List<double> LeftAverage
        {
            get
            {
                return _leftAverage;
            }
        }

        public List<double> RightAverage
        {
            get
            {
                return _rightAverage;
            }
        }

        public List<double> Total
        {
            get
            {
                return _total;
            }
        }

        public List<String> RangeNames
        {
            get
            {
                return _rangeNames;
            }
        }

        public double[,] RangeValues
        {
            get
            {
                return _rangeValues;
            }
            internal set
            {
                _rangeValues = value;
            }
        }

        public List<double> MaxRangeValue
        {
            get
            {
                return _maxRangeValue;
            }
        }

        public double TotalBreast
        {
            get
            {
                return _totalBreast;
            }
        }

        public double TotalFormula
        {
            get
            {
                return _totalFormula;
            }
        }
    }
}
