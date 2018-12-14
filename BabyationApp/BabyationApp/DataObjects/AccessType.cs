using AppServiceHelpers.Models;
using System.Collections.Generic;

namespace BabyationApp.DataObjects
{
    public class AccessType : EntityData
    {
        public string Description { get; set; }
        public List<AccessGroup> AccessGroups { get; set; }
    }
}