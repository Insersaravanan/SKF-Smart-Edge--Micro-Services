using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMaintanenceMicroService.Models
{
    public class AssetHierarchyViewModel
    {
        public string PlantAreaId { get; set; }
        public string PlantName { get; set; }
        public List<EquipmentTemp> Equipments { get; set; }
    }

    public class EquipmentTemp
    {
        public string EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public List<AssetTemp> Assets { get; set; }
    }

    public class AssetTemp
    {
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public string AlarmStatus { get; set; }
    }

    public class StankeyOutput
    {
        public string from { get; set; }
        public string to { get; set; }
        public string value { get; set; }
        public string ptype { get; set; }
        public string etype { get; set; }
        public string id { get; set; }
    }

    public class AlarmStatus
    {
        public string AlarmStatusName { get; set; }
        public string AlarmStatusId { get; set; }
    }

}
