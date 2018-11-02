using CommerceTraining.Models.Catalog;
using Mediachase.Commerce.InventoryService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Models.ViewModels
{
    public class InventoryDemoViewModel
    {
        public ShirtVariation Shirt { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<InventoryRecord> Inventories { get; set; }
    }
}