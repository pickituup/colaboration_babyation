using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using BabyationApp.Controls.Buttons;
using BabyationApp.Common;
using BabyationApp.Converters;
using BabyationApp.Managers;
using BabyationApp.Models;
using BabyationApp.Helpers;
using System.Windows.Input;
using System.Dynamic;
using BabyationApp.Controls.Views;
using BabyationApp.Resources;

namespace BabyationApp.Pages.BottleSession
{
    /// <summary>
    /// This class represents Bottle Feeding Start page from the design
    /// </summary>
    public partial class BottleFeedAmountPage : PageBase
    {
        public BottleFeedAmountModel ViewModel { get; set; }

        private readonly ButtonExGroup _btnGroupMilk = new ButtonExGroup();
        private readonly ButtonExGroup _btnGroupBreastMilk = new ButtonExGroup();

        public BottleFeedAmountPage()
        {
            try
            {
                InitializeComponent();

                ConfigureBottleAndInventory();

                UpdateParams();
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }

        public void UpdateParams()
        {
            ViewModel = new BottleFeedAmountModel(ShowInventory, ShowSavedOverlay, FinishSession);
            BindingContext = ViewModel;

            _btnGroupMilk.Toggled += _btnGroupMilk_Toggled;

            Titlebar.IsVisible = true;
        }


        private void _btnGroupMilk_Toggled(ButtonExGroup sender, ButtonBase item, int index)
        {
            ViewModel.IsBottleTypeSelected = item.IsToggled;
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            // Restore style:
            Titlebar.IsVisible = true;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_NavigationOnTop"];

            var model = (BottleFeedAmountModel)BindingContext;
            if (model != null)
            {
                model.Reset();
            }
        }

        #region Private

        private void ConfigureBottleAndInventory()
        {
            _btnGroupMilk.AddButton(_btnMilkFormula);
            _btnGroupMilk.AddButton(_btnBreastMilk);
            _btnGroupMilk.Toggled += (sender, item, index) =>
            {
                _gridMilkOptions.IsVisible = item == _btnBreastMilk;
                if (_gridMilkOptions.IsVisible == false)
                {
                    _btnGroupBreastMilk.UpdateCurrentButton(null);
                }
                this.InvalidateMeasure();
            };

            _btnGroupBreastMilk.AddButton(_btnMilkFridge);
            _btnGroupBreastMilk.AddButton(_btnMilkFreezer);
            _btnGroupBreastMilk.AddButton(_btnMilkOther);

            _btnGroupBreastMilk.Toggled += (sender, item, index) =>
            {
                if (!_updatingMilkStorageFromCode)
                {
                    if (item == _btnMilkFridge)
                    {
                        InventoryView.Initialize(InventoryFilter.Fridge);
                        ViewModel.ShowInventoryCommand?.Execute(true);
                        LeftPageType = null;
                    }
                    else if (item == _btnMilkFreezer)
                    {
                        InventoryView.Initialize(InventoryFilter.Freezer);
                        ViewModel.ShowInventoryCommand?.Execute(true);
                        LeftPageType = null;
                    }
                    else if (item == _btnMilkOther)
                    {
                        InventoryView.Initialize(InventoryFilter.Other);
                        ViewModel.ShowInventoryCommand?.Execute(true);
                        LeftPageType = null;
                    }
                    else
                    {
                        ViewModel.ShowInventoryCommand?.Execute(false);
                    }
                }
            };
        }

        private bool _updatingMilkStorageFromCode = false;
        private void UpdateStorageType(bool fromModelToGui = true)
        {
            _updatingMilkStorageFromCode = true;
            var currentSession = ViewModel.CurrentSession;
            if (currentSession == null) return;

            if (fromModelToGui)
            {
                if (currentSession.Milk == MilkType.Formula)
                {
                    _btnGroupMilk.UpdateCurrentButton(_btnMilkFormula);
                    _btnGroupBreastMilk.UpdateCurrentButton(null);
                }
                else if (currentSession.Milk == MilkType.BreastMilk)
                {
                    _btnGroupMilk.UpdateCurrentButton(_btnBreastMilk);
                    switch (currentSession.Storage)
                    {
                        case StorageType.Fridge:
                            _btnGroupBreastMilk.UpdateCurrentButton(_btnMilkFridge);
                            break;
                        case StorageType.Freezer:
                            _btnGroupBreastMilk.UpdateCurrentButton(_btnMilkFreezer);
                            break;
                        case StorageType.Other:
                            _btnGroupBreastMilk.UpdateCurrentButton(_btnMilkOther);
                            break;
                        default:
                            _btnGroupBreastMilk.UpdateCurrentButton(null);
                            break;
                    }
                }
            }
            else
            {
                if (_btnGroupMilk.CurrentButton == _btnMilkFormula)
                {
                    currentSession.Milk = MilkType.Formula;
                    currentSession.Storage = StorageType.Other;
                }
                else if (_btnGroupMilk.CurrentButton == _btnBreastMilk)
                {
                    currentSession.Milk = MilkType.BreastMilk;

                    if (_btnGroupBreastMilk.CurrentButton == _btnMilkFridge)
                    {
                        currentSession.Storage = StorageType.Fridge;
                    }
                    else if (_btnGroupBreastMilk.CurrentButton == _btnMilkFreezer)
                    {
                        currentSession.Storage = StorageType.Freezer;
                    }
                    else
                    {
                        currentSession.Storage = StorageType.Other;
                    }
                }
            }

            _updatingMilkStorageFromCode = false;
        }

        private void ShowInventory(bool state)
        {
            if (state)
            {
                Titlebar.LeftButton.IsVisible = true;
                Titlebar.LeftButton.SetDynamicResource(StyleProperty, "CancelButton");
                Titlebar.LeftButton.Clicked += LeftButton_Clicked;
                InventoryView.ItemUseNowEvent += InventoryView_ItemUseNowEvent;
            }
            else
            {
                Titlebar.LeftButton.IsVisible = false;
                Titlebar.LeftButton.Clicked -= LeftButton_Clicked;
                InventoryView.ItemUseNowEvent -= InventoryView_ItemUseNowEvent;
            }
        }

        void LeftButton_Clicked(object sender, EventArgs e)
        {
            ViewModel.ShowInventoryCommand?.Execute(false);
        }

        void InventoryView_ItemUseNowEvent(HistoryModel model)
        {
            if (IsVisible && ViewModel.CurrentSession != null)
            {
                ViewModel.CurrentSession.Milk = model.Milk;

                // Other is the catch all for this page
                ViewModel.CurrentSession.Storage = model.Storage != StorageType.Freezer || model.Storage != StorageType.Fridge
                                                                            ? StorageType.Other : model.Storage;

                HistoryManager.Instance.RemoveInventory(model);

                UpdateStorageType();

                ViewModel.ShowInventoryCommand?.Execute(false);
            }
        }

        /// <summary>
        /// Enable fullscreen UI besfore showing SAVED congrat
        /// </summary>
        private void ShowSavedOverlay()
        {
            Titlebar.IsVisible = false;
            RootLayout.Style = (Style)Application.Current.Resources["AbsoluteLayout_FullScreen"];
        }

        private void FinishSession()
        {
            PageManager.Me.SetCurrentPage(typeof(DashboardTabPage));
        }

        #endregion
    }


    public class BottleFeedAmountModel : ObservableObject
    {
        private readonly DeviceTimer _timer = new DeviceTimer();
        private Action<bool> ShowInventoryAction { get; set; }
        private Action SavedOverlayAction { get; set; }
        private Action FinishSessionAction { get; set; }

        public BottleFeedAmountModel(Action<bool> showInventoryAction, Action savedOverlay, Action finishSessionAction)
        {
            ShowInventoryAction = showInventoryAction;
            SavedOverlayAction = savedOverlay;
            FinishSessionAction = finishSessionAction;

            Reset();
        }

        public void Reset()
        {
            AmountValue = "";
            ShowBottleFeedAmountPage = true;
            ShowInventory = false;
            ShowSavedPopupPage = false;
        }

        #region Public UI properties

        public SessionModel CurrentSession
        {
            get => SessionManager.Instance.CurrentSession;
        }

        bool _isBottleTypeSelected;
        public bool IsBottleTypeSelected
        {
            get { return _isBottleTypeSelected; }
            set
            {
                SetPropertyChanged(ref _isBottleTypeSelected, value);
                SetPropertyChanged(nameof(IsDataValid));
            }
        }

        private string _amountValue;
        public string AmountValue
        {
            get => _amountValue;
            set
            {
                SetPropertyChanged(ref _amountValue, value);
                SetPropertyChanged(nameof(IsDataValid));
            }
        }

        public string ChildName
        {
            get => ProfileManager.Instance.CurrentProfile?.CurrentBaby?.Name;
        }

        public bool IsDataValid
        {
            get => !string.IsNullOrEmpty(AmountValue) && IsBottleTypeSelected;
        }

        private bool _showBottleFeedAmountPage = true;
        public bool ShowBottleFeedAmountPage
        {
            get => _showBottleFeedAmountPage;
            set => SetPropertyChanged(ref _showBottleFeedAmountPage, value);
        }

        private bool _showInventory = false;
        public bool ShowInventory
        {
            get => _showInventory;
            set => SetPropertyChanged(ref _showInventory, value);
        }

        private bool _showSavedPopupPage;
        public bool ShowSavedPopupPage
        {
            get => _showSavedPopupPage;
            set => SetPropertyChanged(ref _showSavedPopupPage, value);
        }

        #endregion

        #region Commands

        private ICommand _showInventoryCommand;
        public ICommand ShowInventoryCommand
        {
            get
            {
                _showInventoryCommand = _showInventoryCommand ?? new Command((obj) =>
                {
                    if (!obj.GetType().Equals(typeof(bool)))
                        return;

                    bool flag = (bool)obj;

                    ShowInventory = flag;
                    ShowBottleFeedAmountPage = !flag;

                    ShowInventoryAction?.Invoke(ShowInventory); // Ask codebehind to prepare UI
                });

                return _showInventoryCommand;
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                _saveCommand = _saveCommand ?? new Command(() =>
                {
                    SavedOverlayAction?.Invoke();  // Ask codebehind to prepare fullscreen UI

                    ShowBottleFeedAmountPage = false;
                    ShowSavedPopupPage = true;

                    if (CurrentSession != null)
                    {
                        CurrentSession.TotalMilkVolume = Double.Parse(AmountValue);

                        SessionManager.Instance.Save();
                    }

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
    }
}
