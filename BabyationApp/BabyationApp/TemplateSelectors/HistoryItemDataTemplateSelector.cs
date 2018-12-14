using System;
using BabyationApp.Interfaces;
using BabyationApp.Models;
using Xamarin.Forms;

namespace BabyationApp.TemplateSelectors
{
    public class HistoryItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PumpingTemplate { get; set; }
        public DataTemplate NursingTemplate { get; set; }
        public DataTemplate BottleTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is ISessionItem sessionItem)
            {
                switch (sessionItem.SessionType)
                {
                    case SessionType.BottleFeed:
                    case SessionType.Breastmilk:
                    case SessionType.Formula:
                        return BottleTemplate;
                    case SessionType.Pump:
                        return PumpingTemplate;
                    case SessionType.Nurse:
                        return NursingTemplate;
                }
            }

            return null;
        }
    }
}
