using CommerceTraining.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Models.ViewModels
{
    public class PaymentDemoViewModel
    {
        public ShirtVariation Shirt { get; set; }
        public string ImageUrl { get; set; }
        public int PurchaseQuantity { get; set; }
        public string MessageOutput { get; set; }
    }
}