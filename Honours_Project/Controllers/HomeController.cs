/*
    Name: Ross Taggart
    ID: S1828840
*/

using Microsoft.AspNetCore.Mvc;

namespace Honours_Project.Controllers
{
    /// <summary>
    /// Default Controller of the Application
    /// </summary>
    public class HomeController : Controller
    {
        //! Section: Methods

        /// <summary>
        /// Method to display landing view of the application
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        //! END Section: Methods
    }
}
