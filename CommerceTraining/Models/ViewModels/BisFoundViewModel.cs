using Mediachase.BusinessFoundation.Data.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Models.ViewModels
{
    public class BisFoundViewModel
    {
        public Boolean ClubCardExists { get; set; }
        public string ViewMessage { get; set; }
        public IEnumerable<EntityObject> ClubCards { get; set; }
        public EntityObject SelectedCard { get; set; }
        public bool IsNew { get; set; }
    }
}