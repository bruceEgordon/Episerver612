using CommerceTraining.Models.ViewModels;
using Mediachase.BusinessFoundation.Core;
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

            if (DataContext.Current.MetaModel.RegisteredTypes["CardType"] != null)
            {
                viewModel.ViewMessage = "The CardType exists!";
                MetaFieldType mf = DataContext.Current.MetaModel.RegisteredTypes["CardType"];
                if (MetaEnum.IsUsed(mf)) viewModel.ViewMessage += " And its in use!";
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

            MetaFieldType cardEnum = DataContext.Current.MetaModel.RegisteredTypes["CardType"];

            if(cardEnum == null)
            {
                using(MetaClassManagerEditScope metaEdit = DataContext.Current.MetaModel.BeginEdit())
                {
                    cardEnum = MetaEnum.Create("CardType", "Club Card Type", false);
                    cardEnum.AccessLevel = AccessLevel.Customization;
                    metaEdit.SaveChanges();
                    MetaEnum.AddItem(cardEnum, "Gold", 1);
                    MetaEnum.AddItem(cardEnum, "Silver", 2);
                    MetaEnum.AddItem(cardEnum, "Bronze", 3);
                }
            }

            using(MetaFieldBuilder fieldBuilder = new MetaFieldBuilder(DataContext.Current.GetMetaClass("ClubCard")))
            {
                MetaField titleField = fieldBuilder.CreateText("TitleField", "Title Field", false, 100, false);
                fieldBuilder.MetaClass.TitleFieldName = titleField.Name;

                fieldBuilder.CreateText("CardOwnerName", "Card Owner Name", false, 100, false);
                fieldBuilder.CreateEmail("Email", "Email", false, 100, true);
                fieldBuilder.CreateInteger("Balance", "Balance", true, 0);
                var mf = fieldBuilder.CreateEnumField("CardTypeEnum", "Card Type", cardEnum.Name, true, String.Empty, true);
                mf.AccessLevel = AccessLevel.Customization;

                fieldBuilder.SaveChanges();

                MetaDataWrapper.CreateReference("Contact", "ClubCard", "ContactRef", "Contact References", false, "InfoBlock", "ClubCard", "10");
            }

            viewModel.ClubCardExists = true;

            return View("Index", viewModel);
        }

        public ActionResult DeleteClubCard()
        {
            var viewModel = new BisFoundViewModel();

            DataContext.Current.MetaModel.DeleteMetaClass("ClubCard");
            MetaFieldType cardEnum = DataContext.Current.MetaModel.RegisteredTypes["CardType"];
            if (cardEnum != null && !MetaEnum.IsUsed(cardEnum))
            {
                MetaEnum.Remove(cardEnum);
            }

            return View("Index", viewModel);
        }
    }
}