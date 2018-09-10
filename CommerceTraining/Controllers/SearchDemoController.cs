using CommerceTraining.Models.ViewModels;
using EPiServer;
using EPiServer.Core;
using EPiServer.Globalization;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Website.Search;
using Mediachase.Search;
using Mediachase.Search.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

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
            vmodel.FacetList = new List<string>();
            // Create criteria
            CatalogEntrySearchCriteria criteria = new CatalogEntrySearchCriteria
            {
                RecordsToRetrieve = 200, // there is a default of 50
                // Locale have to be there... else no hits 
                Locale = ContentLanguage.PreferredCulture.TwoLetterISOLanguageName,
                SearchPhrase = keyWord
            };

            string _SearchConfigPath =
            @"C:\Episerver612\CommerceTraining\CommerceTraining\Configs\Mediachase.Search.Filters.config";

            TextReader reader = new StreamReader(_SearchConfigPath);
            XmlSerializer serializer = new XmlSerializer((typeof(SearchConfig)));
            var _SearchConfig = (SearchConfig)serializer.Deserialize(reader);
            reader.Close();

            foreach (SearchFilter filter in _SearchConfig.SearchFilters)
            {
                // Step 1 - use the XML file
                criteria.Add(filter); 
            }

            // use the manager for search and for index management
            SearchManager manager = new SearchManager("ECApplication");

            // Do search
            ISearchResults results = manager.Search(criteria);

            vmodel.SearchResults = results.Documents.ToList();
            vmodel.FacetGroups = results.FacetGroups.ToList();
            vmodel.ResultCount = results.Documents.Count.ToString();

            return View(vmodel);
        }

        public ActionResult ProviderModelFilteredSearch(string keyWord, string group, string facet)
        {
            var vmodel = new PMSearchResultViewModel();
            vmodel.SearchQueryText = keyWord;
            
            CatalogEntrySearchCriteria criteria = new CatalogEntrySearchCriteria
            { 
                Locale = ContentLanguage.PreferredCulture.TwoLetterISOLanguageName,
                SearchPhrase = keyWord
            };

            string _SearchConfigPath =
            @"C:\Episerver612\CommerceTraining\CommerceTraining\Configs\Mediachase.Search.Filters.config";

            TextReader reader = new StreamReader(_SearchConfigPath);
            XmlSerializer serializer = new XmlSerializer((typeof(SearchConfig)));
            var _SearchConfig = (SearchConfig)serializer.Deserialize(reader);
            reader.Close();

            foreach (SearchFilter filter in _SearchConfig.SearchFilters)
            {
                // Step 1 - use the XML file
                criteria.Add(filter);
            }

            foreach (SearchFilter filter in _SearchConfig.SearchFilters)
            {
                if(filter.field.ToLower() == group.ToLower())
                {
                    var svFilter = filter.Values.SimpleValue.FirstOrDefault(x => x.value.Equals(facet, StringComparison.OrdinalIgnoreCase));
                    if (svFilter != null)
                    {
                        //This overload to Add causes the filter to be applied
                        criteria.Add(filter.field, svFilter);
                    }
                }
            }

            // use the manager for search and for index management
            SearchManager manager = new SearchManager("ECApplication");

            // Do search
            ISearchResults results = manager.Search(criteria);

            vmodel.SearchResults = results.Documents.ToList();
            vmodel.FacetGroups = results.FacetGroups.ToList();
            vmodel.ResultCount = results.Documents.Count.ToString();

            return View("ProviderModelQuery", vmodel);
        }
    }
}