using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Honours_Project.Models;

namespace Honours_Project.Controllers
{
    /// <summary>
    /// Default Controller of the Application
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Method to display landing view of the application
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
