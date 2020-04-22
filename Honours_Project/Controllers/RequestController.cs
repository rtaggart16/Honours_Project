/*
    Name: Ross Taggart
    ID: S1828840
*/

using Microsoft.AspNetCore.Mvc;

namespace Honours_Project.Controllers
{
    /// <summary>
    /// Controller responsible for handling displaying the main request page of application
    /// </summary>
    public class RequestController : Controller
    {
        //! Section: Methods

        /// <summary>
        /// Method to display the Request page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        //! END Section: Methods
    }
}