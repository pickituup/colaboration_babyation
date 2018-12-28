namespace BabyationApp.DataObjects
{
    public class CaregiverRequest : EntityData
    {
        public string ProfileId { get; set; }
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}
