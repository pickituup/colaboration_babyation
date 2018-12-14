using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BabyationApp.Interfaces
{
    public interface IPlatformAPI
    {
        void UpdateStatusBar(String color, bool visible);
        bool HasTopNotch();
        Thickness SafeArea();
    }
}
