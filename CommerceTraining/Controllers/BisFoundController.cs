using CommerceTraining.Models.ViewModels;
using Mediachase.BusinessFoundation.Core;
using Mediachase.BusinessFoundation.Data;
using Mediachase.BusinessFoundation.Data.Business;
using Mediachase.BusinessFoundation.Data.Meta.Management;
using Mediachase.Commerce.Customers;
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
            FillModel(viewModel);

            return View(viewModel);
        }

        public void FillModel(BisFoundViewModel viewModel)
        {
            if (DataContext.Current.MetaModel.MetaClasses["ClubCard"] != null)
            {
                viewModel.ClubCardExists = true;
                viewModel.ClubCards = BusinessManager.List("ClubCard", new[] { new SortingElement("Email", SortingElementType.Desc )});
            }
        }

        public ActionResult CreateClubCardClass()
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

        public ActionResult DeleteClubCardClass()
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

        public ActionResult NewCard()
        {
            var viewModel = new BisFoundViewModel();
            FillModel(viewModel);
            viewModel.IsNew = true;
            viewModel.SelectedCard = new ClubCard();
            viewModel.ContactList = CustomerContext.Current.GetContacts();
            MetaFieldType cardEnum = DataContext.Current.MetaModel.RegisteredTypes["CardType"];
            viewModel.CardTypeList = cardEnum.EnumItems;
            return View("Index", viewModel);
        }

        public ActionResult SubmitCard([Bind(Prefix = "SelectedCard")]ClubCard clubCard)
        {
            var viewModel = new BisFoundViewModel();
            EntityObject card = BusinessManager.InitializeEntity("ClubCard");
            card["TitleField"] = clubCard.TitleField;
            card["CardOwnerName"] = clubCard.CardOwnerName;
            card["Email"] = clubCard.Email;
            card["Balance"] = clubCard.Balance;
            card["ContactRefId"] = (PrimaryKeyId)clubCard.ContactId;
            card["CardTypeEnum"] = clubCard.CardType;
            BusinessManager.Create(card);

            FillModel(viewModel);
            return View("Index", viewModel);
        }

        public ActionResult EditCard(int CardId)
        {
            var viewModel = new BisFoundViewModel();
            FillModel(viewModel);
            viewModel.IsNew = false;
            EntityObject card = BusinessManager.Load("ClubCard", (PrimaryKeyId)CardId);
            viewModel.SelectedCard = new ClubCard();
            viewModel.ContactList = CustomerContext.Current.GetContacts();
            MetaFieldType cardEnum = DataContext.Current.MetaModel.RegisteredTypes["CardType"];
            viewModel.CardTypeList = cardEnum.EnumItems;
            return View("Index", viewModel);
        }
    }
}