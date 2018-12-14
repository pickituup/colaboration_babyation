using System;
using System.Collections.Generic;
using System.Windows.Input;
using BabyationApp.Controls.Buttons;
using Xamarin.Forms;
using System.Linq;
using BabyationApp.Models;
using Xamarin.Forms.Xaml;

namespace BabyationApp.Controls.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionTypeSelector : ContentView
    {
        public static readonly BindableProperty RequestResetProperty = BindableProperty.Create(nameof(RequestReset), typeof(bool), typeof(SessionTypeSelector), false, BindingMode.TwoWay, propertyChanged: OnRequestResetChanged);
        public bool RequestReset
        {
            get => (bool)GetValue(RequestResetProperty);
            set => SetValue(RequestResetProperty, value);
        }
        static void OnRequestResetChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as SessionTypeSelector;
            if (null != self && null != newValue && true == (bool)newValue)
            {
                self.ResetControl();
            }
        }

        public static readonly BindableProperty UpdateMeasureCommandProperty = BindableProperty.Create(nameof(UpdateMeasureCommand), typeof(ICommand), typeof(SessionTypeSelector), default(ICommand), BindingMode.TwoWay);
        public ICommand UpdateMeasureCommand
        {
            get => (ICommand)GetValue(UpdateMeasureCommandProperty);
            set => SetValue(UpdateMeasureCommandProperty, value);
        }

        public static readonly BindableProperty ToggleSessionCommandProperty = BindableProperty.Create(nameof(ToggleSessionCommand), typeof(ICommand), typeof(SessionTypeSelector), default(ICommand));
        public ICommand ToggleSessionCommand
        {
            get => (ICommand)GetValue(ToggleSessionCommandProperty);
            set => SetValue(ToggleSessionCommandProperty, value);
        }

        public static readonly BindableProperty ToggleChildsCommandProperty = BindableProperty.Create(nameof(ToggleChildsCommand), typeof(ICommand), typeof(SessionTypeSelector), default(ICommand));
        public ICommand ToggleChildsCommand
        {
            get => (ICommand)GetValue(ToggleChildsCommandProperty);
            set => SetValue(ToggleChildsCommandProperty, value);
        }

        public static readonly BindableProperty ToggleBottleCommandProperty = BindableProperty.Create(nameof(ToggleBottleCommand), typeof(ICommand), typeof(SessionTypeSelector), default(ICommand));
        public ICommand ToggleBottleCommand
        {
            get => (ICommand)GetValue(ToggleBottleCommandProperty);
            set => SetValue(ToggleBottleCommandProperty, value);
        }

        public static readonly BindableProperty ChildsDatasourceProperty = BindableProperty.Create(nameof(ChildsDatasource), typeof(List<ChildItem>), typeof(SessionTypeSelector), null, BindingMode.OneWay, propertyChanged: OnChildsDatasourceChanged);
        public List<ChildItem> ChildsDatasource
        {
            get => (List<ChildItem>)GetValue(ChildsDatasourceProperty);
            set => SetValue(ChildsDatasourceProperty, value);
        }

        static void OnChildsDatasourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as SessionTypeSelector;
            if (null != self && null != newValue)
            {
                self.UpdateChildsContainer((List<ChildItem>)newValue);
            }
        }

        private ImageButton _currentSession;
        private ImageButton _currentChild;
        private ImageButton _currentBottleType;

        private readonly ButtonExGroup _btnSessionGroup = new ButtonExGroup();
        private readonly ButtonExGroup _btnChildsGroup = new ButtonExGroup();
        private readonly ButtonExGroup _btnBottleGroup = new ButtonExGroup();

        public SessionTypeSelector()
        {
            InitializeComponent();

            // Session selector
            BtnPumping.Tag = SessionType.Pump;
            BtnNursing.Tag = SessionType.Nurse;
            BtnBottle.Tag = SessionType.BottleFeed;

            BtnPumping.IsTogglable = true;
            BtnNursing.IsTogglable = true;
            BtnBottle.IsTogglable = true;

            BtnPumping.Clicked += BtnSession_Clicked;
            BtnNursing.Clicked += BtnSession_Clicked;
            BtnBottle.Clicked += BtnSession_Clicked;

            // Childs selector
            // --/--

            // Bottle content selector
            BtnBreastMilk.Tag = SessionType.Breastmilk;
            BtnFormula.Tag = SessionType.Formula;

            BtnBreastMilk.IsTogglable = true;
            BtnFormula.IsTogglable = true;

            BtnBreastMilk.Clicked += BtnBottleType_Clicked;
            BtnFormula.Clicked += BtnBottleType_Clicked;

            ResetControl();
        }

        public void ResetControl()
        {
            SelectSessionButton(SessionType.Pump);
            SelectChildButton(-1);
            SelectBottleTypeButton(SessionType.Max);

            UpdateDescendantViews();

            RequestReset = false;
        }

        protected void UpdateChildsContainer(List<ChildItem> childItems)
        {
            ChildsContainer.Children.Clear();

            if (0 < childItems.Count)
            {
                int index = 0;
                foreach (ChildItem item in childItems)
                {
                    ImageButton button = new ImageButton()
                    {
                        Tag = index,
                        Text = item.Name,
                        Style = (Style)this.Resources["SessionSelectorButton"]
                    };
                    index++;

                    button.IsTogglable = true;
                    button.Clicked += BtnChild_Clicked;

                    ChildsContainer.Children.Add(button);
                }
            }
            (ChildsContainer.Parent as ScrollView)?.ScrollToAsync(0, 0, true);

            UpdateDescendantViews();
        }


        #region Private 

        private void BtnSession_Clicked(object sender, EventArgs e)
        {
            SessionType tag = (SessionType)((ImageButton)sender).Tag;
            SelectSessionButton(tag);
        }

        private void BtnChild_Clicked(object sender, EventArgs e)
        {
            int idx = (int)((ImageButton)sender).Tag;
            SelectChildButton(idx);
        }

        private void BtnBottleType_Clicked(object sender, EventArgs e)
        {
            SessionType tag = (SessionType)((ImageButton)sender).Tag;
            SelectBottleTypeButton(tag);
        }

        private void SelectSessionButton(SessionType type)
        {
            if( null != _currentSession )
            {
                if( type == (SessionType)_currentSession.Tag )
                {
                    _currentSession.IsToggled = true;
                    return;
                }
                _currentSession.IsToggled = false;
            }

            switch(type)
            {
                case SessionType.Pump:
                    {
                        _currentSession = BtnPumping;
                    }
                    break;
                case SessionType.Nurse:
                    {
                        _currentSession = BtnNursing;
                    }
                    break;
                case SessionType.BottleFeed:
                    {
                        _currentSession = BtnBottle;
                    }
                    break;
            }

            _currentSession.IsToggled = true;

            ToggleSessionCommand?.Execute(type);

            // Clear filters explicitly
            //
            SelectChildButton(-1);
            SelectBottleTypeButton(SessionType.Max);

            UpdateDescendantViews();
        }

        private void SelectChildButton(int index)
        {
            if (null != _currentChild)
            {
                _currentChild.IsToggled = false;
                if( index == (int)_currentChild.Tag )
                {
                    index = -1;
                }
                _currentChild = null;
            }

            foreach (ImageButton btn in ChildsContainer.Children)
            {
                if(index == (int)btn.Tag)
                {
                    _currentChild = btn;
                    _currentChild.IsToggled = true;
                }
                else
                {
                    btn.IsToggled = false;
                }
            }

            ToggleChildsCommand?.Execute(index);
        }

        private void SelectBottleTypeButton(SessionType type)
        {
            if (null != _currentBottleType)
            {
                _currentBottleType.IsToggled = false;
                if( type == (SessionType)_currentBottleType.Tag )
                {
                    type = SessionType.Max;
                }
                _currentBottleType = null;
            }

            switch (type)
            {
                case SessionType.Breastmilk:
                    {
                        _currentBottleType = BtnBreastMilk;
                        _currentBottleType.IsToggled = true;
                    }
                    break;
                case SessionType.Formula:
                    {
                        _currentBottleType = BtnFormula;
                        _currentBottleType.IsToggled = true;
                    }
                    break;
                case SessionType.Max:
                    {
                        BtnBreastMilk.IsToggled = false;
                        BtnFormula.IsToggled = false;
                    }
                    break;
            }

            ToggleBottleCommand?.Execute(type);
        }

        private void UpdateDescendantViews()
        {
            // Show/hide Childs and Bottle lines
            //
            GridChildsLine.IsVisible = CanShowChilds();
            GridBottleLine.IsVisible = CanShowBottleType();

            InvalidateMeasure();
            UpdateMeasureCommand?.Execute(this);
        }

        private bool CanShowChilds()
        {
            return (0 != ChildsDatasource?.Count() 
                    && (SessionType.Nurse == (null == _currentSession ? SessionType.Max : (SessionType)_currentSession.Tag)
                        || SessionType.BottleFeed == (null == _currentSession ? SessionType.Max : (SessionType)_currentSession.Tag)));
        }

        private bool CanShowBottleType()
        {
            return (0 != ChildsDatasource?.Count() && SessionType.BottleFeed == (null == _currentSession ? SessionType.Max : (SessionType)_currentSession.Tag));
        }

        #endregion
    }

    public class ChildItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
