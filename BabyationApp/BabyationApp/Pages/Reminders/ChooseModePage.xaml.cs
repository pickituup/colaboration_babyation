using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using BabyationApp.Common;
using BabyationApp.Models;
using BabyationApp.Managers;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using BabyationApp.Resources;
using Xamarin.Forms.Internals;

namespace BabyationApp.Pages.Reminders
{
    public partial class ChooseModePage : PageBase
    {
        public ChooseModeModel ViewModel { get; set; }

        private AlarmItem alarmItem;

        public ChooseModePage()
        {
            InitializeComponent();

            ViewModel = new ChooseModeModel(Back, ShowSavedOverlay, FinishSession);
            BindingContext = ViewModel;

            Titlebar.IsVisible = true;
            LeftPageType = typeof(DashboardTabPage);
            Titlebar.LeftButton.IsVisible = true;
            Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");
            Titlebar.LeftButton.Clicked += CancelButton_Clicked;
        }

        public void Initialize(AlarmItem item)
        {
            if (null != item)
            {
                alarmItem = item;
            }
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore style:
            Titlebar.IsVisible = true;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];
            
            ViewModel.Reset();
            ViewModel.AlarmItem = alarmItem;
        }

        void CancelButton_Clicked(object sender, EventArgs e)
        {
            // Cleanup
            alarmItem = null;
            ViewModel.AlarmItem = null;
        }

        #region Private

        private void ShowSavedOverlay()
        {
            Titlebar.IsVisible = false;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_FullScreen"];
        }

        private void Back(AlarmItem item)
        {
            alarmItem = null;
            ViewModel.AlarmItem = null;
            ViewModel.Reset();
            PageManager.Me.SetCurrentPage(typeof(CreateReminderPage), View => (View as CreateReminderPage).Initialize(item));
        }

        private void FinishSession()
        {
            PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
        }

        #endregion
    }

    public class ChooseModeModel : ObservableObject
    {
        private readonly DeviceTimer _timer = new DeviceTimer();
        private Action<AlarmItem> BackAction { get; set; }
        private Action SavedOverlayAction { get; set; }
        private Action FinishSessionAction { get; set; }

        public ChooseModeModel(Action<AlarmItem> backAction, Action savedOverlayAction, Action finishSessionAction)
        {
            BackAction = backAction;
            SavedOverlayAction = savedOverlayAction;
            FinishSessionAction = finishSessionAction;
        }

        public void Reset()
        {
            ShowMainPage = true;
            ShowSavedPopupPage = false;

            Refresh();
        }

        #region Public UI properties

        private bool _showMainPage;
        public bool ShowMainPage
        {
            get => _showMainPage;
            set => SetPropertyChanged(ref _showMainPage, value);
        }

        private bool _showSavedPopupPage;
        public bool ShowSavedPopupPage
        {
            get => _showSavedPopupPage;
            set => SetPropertyChanged(ref _showSavedPopupPage, value);
        }

        #endregion

        #region Data properties

        private AlarmItem _alarmItem;
        public AlarmItem AlarmItem 
        { 
            get => _alarmItem; 
            set
            {
                _alarmItem = value;
                if( null != value && 0 < (Datasource?.Count ?? 0) )
                {
                    Datasource.ForEach(x => x.IsSelected = x.Id.Equals(value.ModeId));
                    SetPropertyChanged(nameof(Datasource));
                }
            }
        }
        public List<ModeItem> Datasource { get; private set; }

        private bool _refreshing;
        public bool Refreshing
        {
            get => _refreshing;
            set => SetPropertyChanged(ref _refreshing, value);
        }

        public bool IsReadyToGo
        {
            get => !String.IsNullOrEmpty(AlarmItem?.ModeId ?? null) && !String.IsNullOrEmpty(AlarmItem?.ModeName ?? null);
        }

        #endregion

        #region Commands

        ICommand _selectModeCommand;
        public ICommand SelectModeCommand
        {
            get
            {
                _selectModeCommand = _selectModeCommand ?? new Command((obj) =>
                {
                    if( !obj.GetType().Equals(typeof(ModeItem)))
                        return;

                    ModeItem model = (ModeItem)obj;

                    foreach (var item in Datasource)
                    {
                        if(item.Id.Equals(model.Id))
                        {
                            item.IsSelected = !item.IsSelected;
                            model.IsSelected = item.IsSelected;
                            continue;
                        }
                        item.IsSelected = false;
                    }

                    AlarmItem.ModeId = (model.IsSelected ? model.Id : null);
                    AlarmItem.ModeName = (model.IsSelected ? model.Title : null);

                    //Datasource.ForEach(item => item.IsSelected = item == model);
                    SetPropertyChanged(nameof(Datasource));
                    SetPropertyChanged(nameof(IsReadyToGo));
                });
                return _selectModeCommand;
            }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                _refreshCommand = _refreshCommand ?? new Command(Refresh);
                return _refreshCommand;
            }
        }

        ICommand _backCommand;
        public ICommand BackCommand
        {
            get
            {
                _backCommand = _backCommand ?? new Command(() =>
                {
                    BackAction?.Invoke(AlarmItem);
                });
                return _backCommand;
            }
        }

        ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                _saveCommand = _saveCommand ?? new Command(() =>
                {
                    SavedOverlayAction?.Invoke();  // Ask codebehind to prepare fullscreen UI

                    ShowMainPage = false;
                    ShowSavedPopupPage = true;

                    //TODO: Save pump session reminder here

                    _timer.Enable = true;
                    _timer.Start(() =>
                    {
                        FinishSessionAction?.Invoke();  // Ask codebehind to switch to next page
                        return false;
                    });
                });

                return _saveCommand;
            }
        }

        ICommand _closeSaveViewCommand;
        public ICommand CloseSaveViewCommand
        {
            get
            {
                // Short circuiting the timer
                _closeSaveViewCommand = _closeSaveViewCommand ?? new Command(() =>
                {
                    _timer.Enable = false;
                    FinishSessionAction?.Invoke();
                });

                return _closeSaveViewCommand;
            }
        }

        #endregion

        #region Private

        private void Refresh()
        {
            Refreshing = true;

            if( null != ExperienceManager.Instance.AllExperiences)
            {
                var items = (from item in ExperienceManager.Instance.AllExperiences
                             select new ModeItem()
                             {
                                 Id = item.Id,
                                 Title = item.Name,
                                 Description = item.Description,
                                 CreationDate = item.CreatedAt,
                                 IsPredefined = item.CreatedBy.Equals("babyation"),
                                 IsNew = (item.CreatedBy.Equals("babyation") && item.IsNew),
                                 IsSelected = (AlarmItem?.ModeId == item.Id),
                                 SelectModeCommand = SelectModeCommand
                             }).ToList();

                items = items.OrderByDescending(x => x.CreationDate).ToList();

                if (null == Datasource)
                {
                    Datasource = new List<ModeItem>(items);
                }
                else
                {
                    Datasource.ForEach(item => item.IsSelected = (AlarmItem?.ModeId == item.Id));
                }
            }
            else
            {
                Datasource = null;
            }
            SetPropertyChanged(nameof(Datasource));

            Refreshing = false;
        }

        #endregion
    }
}
