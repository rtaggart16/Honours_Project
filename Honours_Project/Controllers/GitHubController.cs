using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honours_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Honours_Project.Controllers
{
    /// <summary>
    /// The API controller that handles GitHub-related requests to the system. Base URL: "api/github"
    /// </summary>
    [Route("api/github")]
    public class GitHubController : Controller
    {
        private readonly IGitHubService _githubService;

        /// <summary>
        /// Constructor of the controller that handles dependency injection of required services
        /// </summary>
        public GitHubController(IGitHubService githubService)
        {
            _githubService = githubService;
        }

        /// <summary>
        /// Method that handles requests for a specific user's repos. Returns a list of Repo_List_Result
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/users/repositories/{userName}")]
        public async Task<Repo_List_Result> Get_User_Repositories(string userName)
        {
            return await _githubService.Get_User_Repositories(userName);
        }

        [HttpGet]
        [Route("get/repository/stats/{userName}/{repoName}/{start}/{end}")]
        public async Task<Repo_Stat_Result> Get_Repository_Stats(string userName, string repoName, DateTime? start, DateTime? end)
        {
            return await _githubService.Get_Repository_Stats(userName, repoName, start, end);
        }

        [HttpGet]
        [Route("get/repository/commits/{userName}/{repoName}/{pageNumber}")]
        public async Task<Repo_Commit_Result> Get_Repository_Stats(string userName, string repoName, int pageNumber)
        {
            return await _githubService.Get_Repository_Commits(userName, repoName, pageNumber);
        }
    }
}