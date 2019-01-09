using System;
using System.Dynamic;
namespace BabyationApp.Models
{
    public class CaregiverModel : ModelItemBase
    {
        public CaregiverModel()
        {
            IsRequest = false;
        }

        public bool IsRequest { get; set; }

        public string ProfileId { get; set; }

        public string CaregiverId { get; set; }

        public string CaregiverEmail { get; set; }

    }
}
