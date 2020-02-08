using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Types;
using Honours_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        [Route("get/repo/bias/{userName}/{repoName}/{additionThreshold}/{deletionThreshold}")]
        public Task<List<Repo_Commit>> Get_Repo_Bias(string userName, string repoName, int additionThreshold, int deletionThreshold)
        {
            return _githubService.Get_Repo_Bias(userName, repoName, additionThreshold, deletionThreshold);
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

        [HttpGet]
        [Route("get/graphql/commits")]
        public async Task<string> Get_GraphQL_Commits()
        {
            var user = "rtaggart16";
            var repo = "tmpst";
            var branch = "master";

            var commitData = new GraphQLRequest
            {
                Query = "query { repository(name: \"" + repo + "\", owner: \"" + user + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100) { pageInfo { hasNextPage } edges { node { messageHeadline oid message author { name email date } changedFiles additions deletions } } } } } } } }"                
            };

            var client = new GraphQLClient("https://api.github.com/graphql");

            client.DefaultRequestHeaders.Add("User-Agent", "request");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "9807762057a12f61863ba358612c70fb090f8253");

            var response = await client.PostAsync(commitData);

            var data = (object)response.Data;

            var dataString = data.ToString();

            GraphQLRepositoryInfo repoInfo = JsonConvert.DeserializeObject<GraphQLRepositoryInfo>(dataString);

            string content = await response.Data;

            return content;
        }
    }
}