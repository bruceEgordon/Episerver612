using CommerceTraining.Models.ViewModels;
using EPiServer;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    public class MarketsDemoController : Controller
    {
        private IMarketService _marketService;
        private ICurrentMarket _currentMarket;
        private IContentLoader _contentLoader;

        public MarketsDemoController(IMarketService marketService, ICurrentMarket currentMarket, IContentLoader contentLoader)
        {
            _marketService = marketService;
            _currentMarket = currentMarket;
            _contentLoader = contentLoader;
        }
        // GET: MarketsDemo
        public ActionResult Index()
        {
            var viewModel = new DemoMarketsViewModel();
            viewModel.MarketList = _marketService.GetAllMarkets();
            viewModel.SelectedMarket = _currentMarket.GetCurrentMarket();


            return View(viewModel);
        }

        public ActionResult ChangeDefaultMarket(string marketId)
        {
            if (marketId != null)
            {
                //_currentMarket.SetCurrentMarket(marketId);
            }
            var viewModel = new DemoMarketsViewModel();
            viewModel.MarketList = _marketService.GetAllMarkets();
            viewModel.SelectedMarket = _currentMarket.GetCurrentMarket();

            return View("Index", viewModel);
        }
    }
}