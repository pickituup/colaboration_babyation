using System.Dynamic;
namespace BabyationApp.DataObjects
{
    public class CaregiverRelation : EntityData
    {
        public string ProfileId { get; set; }

        public string CaregiverProfileId { get; set; }

        public string CaregiverEmail { get; set; }

        private bool _isDeleteRequested = false;
        public bool IsDeleteRequested 
        { 
            get => _isDeleteRequested; 
            set => _isDeleteRequested = value; 
        }
    }
}
