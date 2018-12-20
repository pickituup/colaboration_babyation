using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Helpers;
using BabyationApp.Interfaces;
using Xamarin.Forms;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BabyationApp.Controls.Views
{
    /// <summary>
    /// The base class to inherit when a page will act like a page, as example in Tab page
    /// </summary>
    public partial class RootViewBase : ContentView, IRootView
    {
        private Titlebar _titleBar;

        /// <summary>
        /// Consstrucotr
        /// </summary>
        /// <remarks>Initializes the titlebar and sets its initial colors</remarks>
        public RootViewBase()
        {
            InitializeComponent();

            _titleBar = Helpers.VisualTreeHelper.GetTemplateChild<BabyationApp.Controls.Views.Titlebar>(this, "MyTitlebar");
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(RootViewBase), "<Title Of>");
        /// <summary>
        /// Title of this view
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Titlebar of thie view
        /// </summary>
        public Titlebar Titlebar { get { return _titleBar; } }

        /// <summary>
        /// Left button (on titlebar) page type to show when the button is clicked
        /// </summary>
        private Type _leftPageType;
        public Type LeftPageType
        {
            get => _leftPageType;
            set => SetProperty(ref _leftPageType, value);
        }

        /// <summary>
        /// Right button (on titlebar) page type to show when the button is clicked
        /// </summary>
        private Type _rightPageType;
        public Type RightPageType
        {
            get => _rightPageType;
            set => SetProperty(ref _rightPageType, value);
        }


        /// <summary>
        /// Called when the page is created
        /// </summary>
        /// <remarks>The inherited classes should call this method too if they override their own. It binds its nav page types with the page-manage</remarks>
        public virtual void PageCreationDone()
        {
            PageManager.Me.SetNavPagesForPage(this);
        }

        /// <summary>
        /// Gets called when the view/page is about to show
        /// </summary>
        public virtual void AboutToShow()
        {
            if (null != Titlebar)
            {
                App.Instance.PlatformAPI?.UpdateStatusBar(Titlebar.TitleBackColor.ToHexString(), Titlebar.IsVisible);

                if (Device.RuntimePlatform == Device.iOS)
                {
                    this.Padding = new Thickness(0, Titlebar.IsVisible ? Double.Parse(Application.Current.Resources["StatusBarHeight"].ToString()) : 0, 0, 0);
                }
            }
        }

        /// <summary>
        /// Gets called the view/page about to hide
        /// </summary>
        public virtual void AboutToHide()
        {

        }

        #region INotifyPropertyChanged

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;

            onChanged?.Invoke();

            OnPropertyChanged(propertyName);
            return true;
        }

        public virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExp = propertyExpression.Body as MemberExpression;
            if (memberExp == null)
            {
                throw new ArgumentException("Expression must be a MemberExpression.", nameof(propertyExpression));
            }

            OnPropertyChanged(memberExp.Member.Name);
        }

        #endregion
    }
}
