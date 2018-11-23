using CommerceTraining.Models.ViewModels;
using Mediachase.BusinessFoundation.Data;
using Mediachase.BusinessFoundation.Data.Meta.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    public class BisFoundController : Controller
    {
        // GET: BisFound
        public ActionResult Index()
        {
            var viewModel = new BisFoundViewModel();
            if(DataContext.Current.MetaModel.MetaClasses["ClubCard"] != null)
            {
                viewModel.ClubCardExists = true;
            }

            return View(viewModel);
        }

        public ActionResult CreateClubCard()
        {
            var viewModel = new BisFoundViewModel();

            using(MetaClassManagerEditScope metaEdit = DataContext.Current.MetaModel.BeginEdit())
            {
                MetaClass clubCard = DataContext.Current.MetaModel.CreateMetaClass("ClubCard",
                    "Club Card", "ClubCards", "demoClub_Cards", PrimaryKeyIdValueType.Integer);
                clubCard.AccessLevel = AccessLevel.Customization;

                metaEdit.SaveChanges();
            }

            using(MetaFieldBuilder fieldBuilder = new MetaFieldBuilder(DataContext.Current.GetMetaClass("ClubCard")))
            {
                MetaField titleField = fieldBuilder.CreateText("TitleField", "Title Field", false, 100, false);
                fieldBuilder.MetaClass.TitleFieldName = titleField.Name;
            }

            return View("Index", viewModel);
        }
    }
}