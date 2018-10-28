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
        // GET: WareHouseDemo
        public ActionResult Index()
        {
            
            return View();
        }
    }
}