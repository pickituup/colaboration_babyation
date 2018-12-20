using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Buttons;
using BabyationApp.Managers;
using BabyationApp.Models;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using BabyationApp.Controls.Views;
using BabyationApp.Resources;

namespace BabyationApp.Pages.BottleSession
{
    /// <summary>
    /// Invetory View from the design to be used in BottleFeedLogPage, BottleFeedStartPage and MyInvetoryPage
    /// </summary>
    public partial class MyInventoryView : RootViewBase
    {
        public static readonly BindableProperty CaregiverFlowProperty = BindableProperty.Create(nameof(CaregiverFlow), typeof(bool), typeof(MyInventoryView), false);
        public bool CaregiverFlow
        {
            get => (bool)GetValue(CaregiverFlowProperty);
            set
            {
                SetValue(CaregiverFlowProperty, value);
            }
        }

        ButtonExGroup _btnGroupMilk = new ButtonExGroup();
        IEnumerable<HistoryModel> _currentModels;
        static bool _isSortAscending = true;

        /// <summary>
        /// Fires the event to let the user of this class know that an inventory selected to use
        /// </summary>
        public event HistoryItemUseNowEvent ItemUseNowEvent;

        /// <summary>
        /// Constructor -- initilizes and binds to button actionsf (e.g. Sorting)
        /// </summary>
        public MyInventoryView()
        {
            InitializeComponent();

            if (CaregiverFlow)
            {
                // Set style:
                Titlebar.IsVisible = true;
                Titlebar.Title = AppResource.InventoryUpper;
                RootLayout.Style = (Style)Application.Current.Resources["StackLayout_NavigationOnTop"];
            }

            _btnGroupMilk.AddButton(_btnMilkAll);
            _btnGroupMilk.AddButton(_btnMilkFridge);
            _btnGroupMilk.AddButton(_btnMilkFreezer);
            _btnGroupMilk.AddButton(_btnMilkOther);

            _btnGroupMilk.Toggled += (sender, item, index) =>
            {
                if (_currentModels != null)
                {
                    foreach (var hi in _currentModels)
                    {
                        hi.UseNowEvent -= OnItemUseNowHandler;
                    }
                }

                _currentModels = null;

                if (_btnMilkAll == item)
                {
                    _currentModels = HistoryManager.Instance.GetInventory(InventoryFilter.All);
                }
                else if (_btnMilkFridge == item)
                {
                    _currentModels = HistoryManager.Instance.GetInventory(InventoryFilter.Fridge);
                }
                else if (_btnMilkFreezer == item)
                {
                    _currentModels = HistoryManager.Instance.GetInventory(InventoryFilter.Freezer);
                }
                else if (_btnMilkOther == item)
                {
                    _currentModels = HistoryManager.Instance.GetInventory(InventoryFilter.Other);
                }

                if (_currentModels != null)
                {
                    foreach (var hi in _currentModels)
                    {
                        hi.UseNowEvent += OnItemUseNowHandler;
                    }
                }

                Sort();
            };

            _btnGroupMilk.UpdateCurrentButton(_btnMilkAll);
        }

        public override void AboutToShow()
        {
            base.AboutToShow();

            if (CaregiverFlow)
            {
                // Restore style:
                Titlebar.IsVisible = true;
                Titlebar.Title = AppResource.InventoryUpper;
                RootLayout.Style = (Style)Application.Current.Resources["StackLayout_NavigationOnTop"];
            }
        }

        /// <summary>
        /// Initializes the view based on filter type provided
        /// </summary>
        /// <param name="filterBy"></param>
        public void Initialize(InventoryFilter filterBy = InventoryFilter.All)
        {
            _btnGroupMilk.UpdateCurrentButton(null);

            ButtonBase btn = _btnMilkAll;

            switch (filterBy)
            {
                case InventoryFilter.Freezer:
                    btn = _btnMilkFreezer;
                    break;
                case InventoryFilter.Fridge:
                    btn = _btnMilkFridge;
                    break;
                case InventoryFilter.Other:
                    btn = _btnMilkOther;
                    break;
            }

            _btnGroupMilk.UpdateCurrentButton(btn);

            listView.SelectedItem = null;
        }


        #region Private

        /// <summary>
        /// Performs sorting bases on user input
        /// </summary>
        private void Sort()
        {
            if (_currentModels != null)
            {
                _currentModels = _isSortAscending ? _currentModels.OrderBy(s => s.StartTime)
                                                                  : _currentModels.OrderByDescending(s => s.StartTime);
            }

            var items = new ObservableCollection<HistoryModel>();

            if (_currentModels != null)
            {
                foreach (HistoryModel item in _currentModels)
                {
                    items.Add(item);
                }
            }

            listView.ItemsSource = items;
        }

        private void OnItemUseNowHandler(HistoryModel model) => ItemUseNowEvent?.Invoke(model);

        #endregion
    }
}
