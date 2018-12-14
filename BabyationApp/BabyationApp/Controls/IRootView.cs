using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Controls.Views;

namespace BabyationApp.Controls
{
    interface IRootView
    {
        Titlebar Titlebar { get; }
        Type LeftPageType { get; set; }
        Type RightPageType { get; set; }

        void PageCreationDone();

        void AboutToShow();
        void AboutToHide();
    }
}
