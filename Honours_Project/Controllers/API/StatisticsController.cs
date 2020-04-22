/*
    Name: Ross Taggart
    ID: S1828840
*/

using Honours_Project.Models;
using Honours_Project.Services;
using Microsoft.AspNetCore.Mvc;

namespace Honours_Project.Controllers
{
    /// <summary>
    /// The API controller that handles Statistics-related requests to the system. Base URL: "api/statistics"
    /// </summary>
    [Route("api/statistics")]
    public class StatisticsController : Controller
    {
        //! Section: Globals

        /// <summary>
        /// Service that allows statistics calculations
        /// </summary>
        private readonly IStatisticsService _statisticsService;

        //! END Section: Globals

        //! Section: Methods

        /// <summary>
        /// Constructor of the controller that handles dependency injection of required services
        /// </summary>
        /// <param name="statisticsService">Service that allows statistics calculations</param>
        public StatisticsController(IStatisticsService statisticsService)
        {
            // Populate the service using dependency injection
            _statisticsService = statisticsService;
        }

        /// <summary>
        /// Method to fetch a collaborator's contribution score
        /// </summary>
        /// <param name="request">Required options for calculating contributions</param>
        /// <returns>A user's contribution score</returns>
        [HttpPost]
        [Route("get/contribution/score")]
        public Contribution_Result Get_Contribution_Score([FromBody] Contribution_Request request)
        {
            // Get a user's contribution score
            return _statisticsService.CalculateBasicCommitContributionScore(request);
        }

        /// <summary>
        /// Method to fetch a collaborator's addition score only
        /// </summary>
        /// <param name="request">Required options for calculating addition score</param>
        /// <returns>A user's addition score</returns>
        [HttpPost]
        [Route("get/addition/score")]
        public decimal Get_Addition_Score([FromBody] Contribution_Request request)
        {
            // Get a user's addition score
            return _statisticsService.Calculate_Addition_Score(request.User.Addition_Total, request.Repo.Addition_Total, request.Author_Total);
        }

        /// <summary>
        /// Method to fetch a collaborator's deletion score only
        /// </summary>
        /// <param name="request">Required options for calculating deletion score</param>
        /// <returns>A user's deletion score</returns>
        [HttpPost]
        [Route("get/deletion/score")]
        public decimal Get_Deletion_Score([FromBody] Contribution_Request request)
        {
            // Get a user's deletion score
            return _statisticsService.Calculate_Deletion_Score(request.User.Deletion_Total, request.Repo.Deletion_Total, request.Author_Total);
        }

        /// <summary>
        /// Method to fetch a collaborator's commit score only
        /// </summary>
        /// <param name="request">Required options for calculating commit score</param>
        /// <returns>A user's commit score</returns>
        [HttpPost]
        [Route("get/commit/score")]
        public decimal Get_Commit_Score([FromBody] Contribution_Request request)
        {
            // Get a user's commit score
            return _statisticsService.Calculate_Commit_Score(request.User.Commit_Total, request.Repo.Commit_Total, request.Author_Total);
        }

        //! END Section: Methods
    }
}