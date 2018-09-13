using CommerceTraining.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Models.ViewModels
{
    public class FindResultViewModel
    {
        public string SearchText { get; set; }
        public string ResultCount { get; set; }
        public List<ShirtVariation> ShirtVariants { get; set; }
    }
}