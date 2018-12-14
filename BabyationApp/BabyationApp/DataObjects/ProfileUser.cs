using AppServiceHelpers.Models;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    public class ProfileUser: EntityData
    {
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string AccessGroupId { get; set; }
        public AccessGroup AccessGroup { get; set; }
        public string RoleId { get; set; }
        public Role Role { get; set; }
    }
}