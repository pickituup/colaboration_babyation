using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BabyationApp.Pages
{
    public class StackPagesContainerPage : MultiPage<ContentPage>
    {
        public StackPagesContainerPage()
        {
            
        }
        protected override ContentPage CreateDefault(object item)
        {
            return null;//return PageManager.Me.GetRootView(item as Type) as ContentPage;
        }
    }
}
