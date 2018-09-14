using CommerceTraining.Models.Catalog;
using CommerceTraining.Models.ViewModels;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    public class FindSearchDemoController : Controller
    {
        // GET: FindSearchDemo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FindQueryIntegrated(string keyWord)
        {
            var viewModel = new FindResultViewModel
            {
                SearchText = keyWord
            };

            IClient client = SearchClient.Instance;
            

            var result = client.Search<ShirtVariation>()
              .For(keyWord).FilterOnLanguages(new string[] { "en" })
              .TermsFacetFor(x => x.Brand)
              .TermsFacetFor(x => x.Size)
              .GetContentResult();

            viewModel.ResultCount = result.Items.Count().ToString();
            viewModel.ShirtVariants = result.ToList();

            return View(viewModel);
        }
    }
}