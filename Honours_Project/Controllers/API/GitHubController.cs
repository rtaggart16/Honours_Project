/*
    Name: Ross Taggart
    ID: S1828840
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using Honours_Project.Models;
using Honours_Project.Services;
using Microsoft.AspNetCore.Mvc;

namespace Honours_Project.Controllers
{
    /// <summary>
    /// The API controller that handles GitHub-related requests to the system. Base URL: "api/github"
    /// </summary>
    [Route("api/github")]
    public class GitHubController : Controller
    {
        //! Section: Globals

        /// <summary>
        /// Service that allows interaction with the GitHub API v3
        /// </summary>
        private readonly IGitHubService _githubService;

        /// <summary>
        /// Service that allows interaction with the GitHub API v4
        /// </summary>
        private readonly IGraphQLService _graphQLService;

        //! END Section: Globals

        //! Section: Methods

        /// <summary>
        /// Constructor of the controller that handles dependency injection of required services
        /// </summary>
        /// <param name="githubService">Service that allows interaction with the GitHub API v3</param>
        /// <param name="graphQLService">Service that allows interaction with the GitHub API v4</param>
        public GitHubController(IGitHubService githubService, IGraphQLService graphQLService)
        {
            // Populate the services using dependency injection
            _githubService = githubService;
            _graphQLService = graphQLService;
        }

        /// <summary>
        /// Method that handles requests for a specific user's repos. Returns a list of Repo_List_Result
        /// </summary>
        /// <param name="userName">The name of the target user to fetch repos for</param>
        /// <returns>A list of a user's repositories</returns>
        [HttpGet]
        [Route("get/users/repositories/{userName}")]
        public async Task<Repo_List_Result> Get_User_Repositories(string userName)
        {
            // Fetch the user's repositories
            return await _githubService.Get_User_Repositories(userName);
        }

        /// <summary>
        /// Method to fetch repository bias for a given repository
        /// </summary>
        /// <param name="requestData">Required information about a repository</param>
        /// <returns>List of bias commits</returns>
        [HttpPost]
        [Route("get/repo/bias")]
        public Task<Repo_Bias_Result> Get_Repo_Bias([FromBody]Repo_Stat_Request requestData)
        {
            // Fetch the repository's bias commits
            return _githubService.Get_Repo_Bias(requestData);
        }

        /// <summary>
        /// Method to fetch stats for each collaborator in a repository
        /// </summary>
        /// <param name="requestData">Data required to calculate repository stats</param>
        /// <returns>List of stats for each collaborator</returns>
        [HttpPost]
        [Route("get/repository/stats")]
        public async Task<Repo_Stat_Result> Get_Repository_Stats([FromBody]Repo_Stat_Request requestData)
        {
            // Get stats for each collaborator
            return await _githubService.Get_Repository_Stats(requestData);
        }

        /// <summary>
        /// Method to get the initial commits of a repository
        /// </summary>
        /// <param name="userName">The owner of the target repository</param>
        /// <param name="repoName">The name of the target repository</param>
        /// <returns>List of initial commits</returns>
        [HttpGet]
        [Route("get/initial/commits/{userName}/{repoName}")]
        public async Task<List<Repo_Commit>> Get_Initial_Commits(string userName, string repoName)
        {
            // Get the list of commits
            return await _githubService.Get_Repo_Initial_Commits(userName, repoName);
        }

        //! END Section: Methods
    }
}