using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Types;
using Honours_Project.Models;
using Honours_Project.Services;
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

        private readonly IGraphQLService _graphQLService;

        /// <summary>
        /// Constructor of the controller that handles dependency injection of required services
        /// </summary>
        public GitHubController(IGitHubService githubService, IGraphQLService graphQLService)
        {
            _githubService = githubService;
            _graphQLService = graphQLService;
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

        [HttpPost]
        [Route("get/repo/bias")]
        public Task<Repo_Bias_Result> Get_Repo_Bias([FromBody]Repo_Stat_Request requestData)
        {
            return _githubService.Get_Repo_Bias(requestData);
        }

        [HttpPost]
        [Route("get/repository/stats")]
        public async Task<Repo_Stat_Result> Get_Repository_Stats([FromBody]Repo_Stat_Request requestData)
        {
            return await _githubService.Get_Repository_Stats(requestData);
        }

        [HttpGet]
        [Route("get/initial/commits/{userName}/{repoName}")]
        public async Task<List<Repo_Commit>> Get_Initial_Commits(string userName, string repoName)
        {
            return await _githubService.Get_Repo_Initial_Commits(userName, repoName);
        }

        /*[HttpGet]
        [Route("get/repository/commits/{userName}/{repoName}/{pageNumber}")]
        public async Task<Repo_Commit_Result> Get_Repository_Stats(string userName, string repoName, int pageNumber)
        {
            return await _githubService.Get_Repository_Commits(userName, repoName, pageNumber);
        }*/

        [HttpGet]
        [Route("get/graphql/commits")]
        public async Task<List<Node>> Get_GraphQL_Commits()
        {
            var user = "rtaggart16";
            var repo = "tmpst";
            var branch = "master";

            bool allCommitsFetched = false;

            var pass = 1;

            List<Node> nodes = new List<Node>();

            var after = "";

            do
            {
                var query = "";

                if(pass != 1)
                {
                    query = "query { repository(name: \"" + repo + "\", owner: \"" + user + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100, after: \"" + after + "\") { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl } name email date } changedFiles additions deletions } } } } } } } }";
                }
                else
                {
                    query = "query { repository(name: \"" + repo + "\", owner: \"" + user + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100) { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl } name email date } changedFiles additions deletions } } } } } } } }";
                }

                var response = await _graphQLService.Perform_GraphQL_Request(query, GitHub_Model_Types.GraphQL_Repository_Result);

                var parsedResponse = (GraphQLRepositoryResult)response;

                pass++;

                if(parsedResponse.Errors.Count() == 0)
                {
                    nodes.AddRange(parsedResponse.RepositoryInfo.Repository.Ref.Target.History.Edges.Select(x => x.Node));

                    if(parsedResponse.RepositoryInfo.Repository.Ref.Target.History.PageInfo.HasNextPage)
                    {
                        after = parsedResponse.RepositoryInfo.Repository.Ref.Target.History.PageInfo.EndCursor;
                    }
                    else
                    {
                        allCommitsFetched = true;
                    }
                }
                else
                {
                    break;
                }

            } while (allCommitsFetched == false);

            return nodes;
        }
    }
}