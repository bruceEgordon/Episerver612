using CommerceTraining.Models.ViewModels;
using Mediachase.Commerce.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    public class WareHouseDemoController : Controller
    {
        private IWarehouseRepository _warehouseRepository;

        public WareHouseDemoController(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }
        public ActionResult Index()
        {
            var viewModel = new WarehouseDemoViewModel();
            viewModel.Warehouses = _warehouseRepository.List();
            return View(viewModel);
        }
        public ActionResult Select(string code)
        {
            var viewModel = new WarehouseDemoViewModel();
            viewModel.Warehouses = _warehouseRepository.List();
            viewModel.SelectedWarehouse = viewModel.Warehouses.Where(w => w.Code == code).First();

            return View("Index", viewModel);
        }

        public ActionResult New()
        {
            var viewModel = new WarehouseDemoViewModel();
            viewModel.Warehouses = _warehouseRepository.List();
            viewModel.SelectedWarehouse = new Warehouse();
            return View("Index", viewModel);
        }

        public ActionResult Submit([Bind(Prefix = "SelectedWarehouse", Exclude = "ContactInformation")]Warehouse warehouse,
            [Bind(Prefix = "SelectedWarehouse.ContactInformation")]WarehouseContactInformation warehouseContact)
        {
            var viewModel = new WarehouseDemoViewModel();
            
            return View("Index", viewModel);
        }
    }
}