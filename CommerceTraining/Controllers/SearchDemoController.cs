using CommerceTraining.Models.ViewModels;
using EPiServer;
using EPiServer.Core;
using EPiServer.Globalization;
using Mediachase.Commerce.Catalog;
using Mediachase.Search;
using Mediachase.Search.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    public class SearchDemoController : Controller
    {
        private ReferenceConverter _referenceConverter;
        private IContentLoader _contentLoader;

        public SearchDemoController(ReferenceConverter referenceConverter, IContentLoader contentLoader)
        {
            _referenceConverter = referenceConverter;
            _contentLoader = contentLoader;
        }
        // GET: SearchDemo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProviderModelQuery(string keyWord)
        {
            var vmodel = new PMSearchResultViewModel();
            vmodel.SearchQueryText = keyWord;

            // Create criteria
            CatalogEntrySearchCriteria criteria = new CatalogEntrySearchCriteria
            {
                RecordsToRetrieve = 200, // there is a default of 50
                // Locale have to be there... else no hits 
                Locale = ContentLanguage.PreferredCulture.TwoLetterISOLanguageName,
                SearchPhrase = keyWord
            };

            // use the manager for search and for index management
            SearchManager manager = new SearchManager("ECApplication");

            // Do search
            ISearchResults results = manager.Search(criteria);

            vmodel.SearchResults = results.Documents.ToList();
            //int[] ints = results.GetKeyFieldValues<int>();
            ViewBag.resultCount = results.Documents.Count;

            //List<ContentReference> refs = new List<ContentReference>();
            //ints.ToList().ForEach(i => refs.Add(_referenceConverter.GetContentLink(i, CatalogContentType.CatalogEntry, 0)));

            //vmodel.allContent = _contentLoader.GetItems(refs, new LoaderOptions());

            return View(vmodel);
        }
    }
}