using AppServiceHelpers.Models;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    public class Role : EntityData
    {
        public string Description { get; set; }
        public List<ProfileUser> ProfileUsers { get; set; }
    }
}