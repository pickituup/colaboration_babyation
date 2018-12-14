using AppServiceHelpers.Models;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    public class AccessGroup: EntityData
    {
        public List<AccessType> Accesses { get; set; }
        public List<ProfileUser> ProfileUsers { get; set; }
    }
}