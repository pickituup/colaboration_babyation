using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BabyationApp.ViewModels;

namespace BabyationApp.Pages.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAnotherChildPage : PageBase
    {
        public AddAnotherChildViewModel ViewModel { get; set; }

        public AddAnotherChildPage()
        {
            InitializeComponent();
           
            Titlebar.IsVisible = true;

            UpdateParams();
        }

        public void UpdateParams()
        {
            this.ViewModel = new AddAnotherChildViewModel();
            this.BindingContext = this.ViewModel;
        }
    }
}
