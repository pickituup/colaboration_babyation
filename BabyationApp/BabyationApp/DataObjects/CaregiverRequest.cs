namespace BabyationApp.DataObjects
{
    public class CaregiverRequest : EntityData
    {
        public string ProfileId { get; set; }
        public string Email { get; set; }
        public string VerificationCode { get; set; }

        private bool _isDeleteRequested = false;
        public bool IsDeleteRequested
        {
            get => _isDeleteRequested;
            set => _isDeleteRequested = value;
        }
    }
}
