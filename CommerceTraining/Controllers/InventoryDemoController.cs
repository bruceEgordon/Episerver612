using CommerceTraining.Models.Catalog;
using CommerceTraining.Models.ViewModels;
using EPiServer;
using Mediachase.Commerce.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    public class InventoryDemoController : Controller
    {
        private ReferenceConverter _referenceConverter;
        private IContentLoader _contentLoader;

        public InventoryDemoController(ReferenceConverter referenceConverter, IContentLoader contentLoader)
        {
            _referenceConverter = referenceConverter;
            _contentLoader = contentLoader;
        }
        // GET: InventoryDemo
        public ActionResult Index()
        {
            var viewModel = new InventoryDemoViewModel();
            var shirtRef = _referenceConverter.GetContentLink("Long Sleeve Shirt White Small_1");
            viewModel.Shirt = _contentLoader.Get<ShirtVariation>(shirtRef);

            return View(viewModel);
        }
    }
}