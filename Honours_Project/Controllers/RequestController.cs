using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Honours_Project.Controllers
{
    /// <summary>
    /// Controller responsible for handling displaying the main request page of application
    /// </summary>
    public class RequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}