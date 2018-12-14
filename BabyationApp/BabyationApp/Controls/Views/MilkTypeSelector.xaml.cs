using System;
using System.Collections.Generic;
using System.Windows.Input;
using BabyationApp.Controls.Buttons;
using Xamarin.Forms;
using System.Linq;
using BabyationApp.Models;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

namespace BabyationApp.Controls.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MilkTypeSelector : ContentView
    {
        public static readonly BindableProperty ResetMilkTypeSelectorProperty = BindableProperty.Create(nameof(ResetMilkTypeSelector), typeof(bool), typeof(MilkTypeSelector), false, BindingMode.Default, propertyChanged: OnMilkTypeSelectorChanged);
        public bool ResetMilkTypeSelector
        {
            get => (bool)GetValue(ResetMilkTypeSelectorProperty);
            set => SetValue(ResetMilkTypeSelectorProperty, value);
        }
        static void OnMilkTypeSelectorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as MilkTypeSelector;
            if (null != self && null != newValue && true == (bool)newValue)
            {
                self.ResetControl();
            }
        }

        public static readonly BindableProperty MilkTypeSelectorLabelsProperty = BindableProperty.Create(nameof(MilkTypeSelectorLabels), typeof(List<string>), typeof(MilkTypeSelector), null, BindingMode.Default, propertyChanged: OnMilkTypeSelectorLabelsChanged);
        public List<string> MilkTypeSelectorLabels
        {
            get => (List<string>)GetValue(MilkTypeSelectorLabelsProperty);
            set => SetValue(MilkTypeSelectorLabelsProperty, value);
        }
        static void OnMilkTypeSelectorLabelsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as MilkTypeSelector;
            if (null != self && null != newValue && 0 < ((List<string>)newValue).Count)
            {
                self.UpdateLabels(newValue as List<string>);
            }
        }

        public static readonly BindableProperty UpdateMilkTypeProperty = BindableProperty.Create(nameof(UpdateMilkType), typeof(StorageType), typeof(MilkTypeSelector), StorageType.Unspecified, BindingMode.TwoWay, propertyChanged: OnMilkTypeChanged);
        public StorageType UpdateMilkType
        {
            get => (StorageType)GetValue(UpdateMilkTypeProperty);
            set => SetValue(UpdateMilkTypeProperty, value);
        }
        static void OnMilkTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as MilkTypeSelector;
            if (null != self && null != newValue)
            {
                self.SelectMilkTypeButton((StorageType)newValue, isReset:false, isFromCode:true);
            }
        }

        public static readonly BindableProperty TitleTextProperty = BindableProperty.Create("TitleText", typeof(String), typeof(MilkTypeSelector), null);
        /// <summary>
        /// Gets/Sets title of the titlebar
        /// </summary>
        public String TitleText
        {
            get { return (String)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        public static readonly BindableProperty ToggleMilkTypeCommandProperty = BindableProperty.Create(nameof(ToggleMilkTypeCommand), typeof(ICommand), typeof(MilkTypeSelector), default(ICommand));
        public ICommand ToggleMilkTypeCommand
        {
            get => (ICommand)GetValue(ToggleMilkTypeCommandProperty);
            set => SetValue(ToggleMilkTypeCommandProperty, value);
        }

        private ImageButton _currentMilkType;

        public MilkTypeSelector()
        {
            InitializeComponent();

            // Milk type selector
            BtnStorageFridge.Tag = StorageType.Fridge;
            BtnStorageFreezer.Tag = StorageType.Freezer;
            BtnStorageFeed.Tag = StorageType.Feed;
            BtnStorgaeTrash.Tag = StorageType.Trash;

            BtnStorageFridge.IsTogglable = true;
            BtnStorageFreezer.IsTogglable = true;
            BtnStorageFeed.IsTogglable = true;
            BtnStorgaeTrash.IsTogglable = true;

            BtnStorageFridge.Clicked += BtnMilkType_Clicked;
            BtnStorageFreezer.Clicked += BtnMilkType_Clicked;
            BtnStorageFeed.Clicked += BtnMilkType_Clicked;
            BtnStorgaeTrash.Clicked += BtnMilkType_Clicked;

            ResetControl();
        }

        public void ResetControl()
        {
            SelectMilkTypeButton(StorageType.Unspecified, true);
        }

        private void UpdateLabels(List<string> newLabels)
        {
            if (4 <= newLabels?.Count)
            {
                BtnStorageFridge.Text = newLabels[0];
                BtnStorageFreezer.Text = newLabels[1];
                BtnStorageFeed.Text = newLabels[2];
                BtnStorgaeTrash.Text = newLabels[3];
            }
        }

        #region Private 

        private void BtnMilkType_Clicked(object sender, EventArgs e)
        {
            StorageType tag = (StorageType)((ImageButton)sender).Tag;
            SelectMilkTypeButton(tag);
        }

        /// <summary>
        /// Selects the milk type button.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="isReset">If set to <c>true</c> do not call toggle command.</param>
        /// <param name="isFromCode">If set to <c>true</c> do not clear type because it's doesn't user toggle.</param>
        private void SelectMilkTypeButton(StorageType type, bool isReset = false, bool isFromCode = false)
        {
            if (null != _currentMilkType)
            {
                _currentMilkType.IsToggled = false;

                if (type == (StorageType)_currentMilkType.Tag && !isFromCode)
                {
                    type = StorageType.Unspecified;
                }

                _currentMilkType = null;
            }

            switch (type)
            {
                case StorageType.Fridge:
                    {
                        _currentMilkType = BtnStorageFridge;
                        _currentMilkType.IsToggled = true;
                    }
                    break;
                case StorageType.Freezer:
                    {
                        _currentMilkType = BtnStorageFreezer;
                        _currentMilkType.IsToggled = true;
                    }
                    break;
                case StorageType.Feed:
                    {
                        _currentMilkType = BtnStorageFeed;
                        _currentMilkType.IsToggled = true;
                    }
                    break;
                case StorageType.Trash:
                    {
                        _currentMilkType = BtnStorgaeTrash;
                        _currentMilkType.IsToggled = true;
                    }
                    break;
                case StorageType.Unspecified:
                    {
                        _currentMilkType = null;
                        BtnStorageFridge.IsToggled = false;
                        BtnStorageFreezer.IsToggled = false;
                        BtnStorageFeed.IsToggled = false;
                        BtnStorgaeTrash.IsToggled = false;
                    }
                    break;
            }
            
            if (isReset)
            {
                return;
            }

            ToggleMilkTypeCommand?.Execute(type);
        }

        #endregion
    }
}

