using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace BabyationApp.ViewModels
{
	public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
	{
        public INavigation Navigation { get; set; }
        
	    protected bool ChangeNotificationSuspended { get; set; }

        private bool _refreshing;
	    public bool Refreshing
	    {
	        get => _refreshing;
	        set => SetProperty(ref _refreshing, value);
	    }

        private bool _isFirstLoad = true;
        public bool FirstLoad
        {
            get => _isFirstLoad;
            set => SetProperty(ref _isFirstLoad, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected BaseViewModel()
	    {
	    }

        protected BaseViewModel(INavigation navigation)
        {
            Navigation = navigation;
        }

        public virtual void OnAppearing()
        {
        }

        public virtual void OnDisappearing()
        {
        }

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

		public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (ChangeNotificationSuspended)
			{
				return;
			}

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			
		}

		public void SuspendChangeNotification()
		{
			ChangeNotificationSuspended = true;
		}

		public void ResumeChangeNotification()
		{
			ChangeNotificationSuspended = false;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
    }
}
