using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Views;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using BabyationApp.Controls.Buttons;
using BabyationApp.Interfaces;
using BabyationApp.Managers;
using BabyationApp.Models;
using BabyationApp.Pages.BottleSession;
using BabyationApp.Pages.Settings.PumpSettings;
using BabyationApp.Resources;

namespace BabyationApp.Pages.Modes
{
    /// <summary>
    /// This class represents Modes Dashbaord page from the design
    /// </summary>
    public partial class ModesDashboardView : RootViewBase
    {
        /// <summary>
        /// Constructor -- Initialize the model and binds buttons events and other ui actions
        /// </summary>
        public ModesDashboardView()
        {
            InitializeComponent();
            Titlebar.IsVisible = true;
            Titlebar.Title = AppResource.PumpingModes;

            btnCreateNewMode.CommandEx = new Command((Object sender) =>
            {
                ExperienceManager.Instance.EditingExperience = ExperienceManager.Instance.Create();
                ExperienceManager.Instance.EditingExperience.Breast = BreastType.Both;
                PageManager.Me.SetCurrentPage(typeof(SelectSpeedPage), view => (view as SelectSpeedPage).Initialize());
            });

            ExperienceManager.Instance.EditExperience += model =>
            {
                ExperienceManager.Instance.EditingExperience = model;
                PageManager.Me.SetCurrentPage(typeof(SelectSpeedPage), view => (view as SelectSpeedPage).Initialize(isEditMode: true));
            };

            ExperienceManager.Instance.ExperienceChangedEvent += Instance_ExperienceChanged;
            ExperienceManager.Instance.ExperienceAddedEvent += Instance_ExperienceChanged;
            ExperienceManager.Instance.CurrentExperienceChanged += Instance_ExperienceChanged;
        }

        /// <summary>
        /// Gets called when this page is about to show and performs the initialization
        /// </summary>
        public override void AboutToShow()
        {
            base.AboutToShow();

            Refresh();
        }

        #region Private

        void Instance_ExperienceChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            ObservableCollection<ModeGroupItemItem> groups = new ObservableCollection<ModeGroupItemItem>();

            if (null != ExperienceManager.Instance.UserExperiences)
            {
                var userItems = (from item in ExperienceManager.Instance.UserExperiences
                                 select new ModeItem()
                                 {
                                     Id = item.Id,
                                     Title = item.Name,
                                     Description = item.Description,
                                     CreationDate = item.CreatedAt,
                                     IsPredefined = item.CreatedBy.Equals("babyation"),
                                     IsNew = (item.CreatedBy.Equals("babyation") && item.IsNew),
                                     IsSelected = false,
                                     SelectModeCommand = SelectModeCommand
                                 }).ToList();

                userItems = userItems.OrderByDescending(x => x.CreationDate).ToList();

                ModeGroupItemItem modesByMe = new ModeGroupItemItem(new ObservableCollection<ModeItem>(userItems))
                {
                    IsSpecialGroup = true,
                    GroupTitle = AppResource.EditYourPumpingModes,
                    GroupKey = "me"
                };

                if (modesByMe.Count > 0)
                {
                    groups.Add(modesByMe);
                }
            }

            if (null != ExperienceManager.Instance.PresetExperiences)
            {
                var presetItems = (from item in ExperienceManager.Instance.PresetExperiences
                                   select new ModeItem()
                                   {
                                       Id = item.Id,
                                       Title = item.Name,
                                       Description = item.Description,
                                       CreationDate = item.CreatedAt,
                                       IsPredefined = item.CreatedBy.Equals("babyation"),
                                       IsNew = (item.CreatedBy.Equals("babyation") && item.IsNew),
                                       IsSelected = false,
                                       SelectModeCommand = null
                                   }).ToList();

                presetItems = presetItems.OrderByDescending(x => x.CreationDate).ToList();

                ModeGroupItemItem modesByBabyation = new ModeGroupItemItem(new ObservableCollection<ModeItem>(presetItems))
                {
                    GroupTitle = AppResource.BabyationModes,
                    GroupKey = "babyation"
                };

                if (modesByBabyation.Count > 0)
                {
                    groups.Add(modesByBabyation);
                }
            }

            listView.ItemsSource = null;
            listView.ItemsSource = groups;
            listView.SelectedItem = null;
        }

        private ICommand _selectModeCommand;
        public ICommand SelectModeCommand
        {
            get
            {
                _selectModeCommand = _selectModeCommand ?? new Command((obj) =>
                {
                    if (!obj.GetType().Equals(typeof(ModeItem)))
                        return;

                    ModeItem item = (ModeItem)obj;

                    ExperienceModel model = ExperienceManager.Instance.UserExperiences.FirstOrDefault(x => x.Id == item.Id);
                    if( null != model )
                    {
                        model.EditCommand?.Execute(this);
                    }
                });
                return _selectModeCommand;
            }
        }

        #endregion
    }

    /// <summary>
    /// This class represents the model for a group of the modes
    /// </summary>
    public class ModeGroupItemItem : ListViewList<ModeItem>
    {
        /// <summary>
        /// Constructor -- initalizes from a item of experience model for a group
        /// </summary>
        /// <param name="source"></param>
        public ModeGroupItemItem(ObservableCollection<ModeItem> source) : base(source)
        {

        }

        /// <summary>
        /// This true only for the first one (Modes by me); Otherwise this group Modes by Babyation
        /// </summary>
        public bool IsSpecialGroup { get; set; }

        /// <summary>
        /// Number of modes in this group
        /// </summary>
        public new int Count
        {
            get { return IsSpecialGroup ? base.Count - 1 : base.Count; }
        }

        /// <summary>
        /// Title for this mode group
        /// </summary>
        public String GroupTitle { get; set; }

        /// <summary>
        /// Text should shows after the group name
        /// </summary>
        public String GroupKey { get; set; }

        /// <summary>
        /// Commands when executes shows the CreateANewModePage
        /// </summary>
        public ICommand SpecialCommand { get; set; }
    }


}
