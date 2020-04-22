/*
    Name: Ross Taggart
    ID: S1828840
*/

using Honours_Project.Models;
using Honours_Project.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Honours_Project
{
    /// <summary>
    /// Interface that contains method definitions for fetching and manipulating GitHub data
    /// </summary>
    public interface IGitHubService
    {
        /// <summary>
        /// Method to fetch a user's public repositories
        /// </summary>
        /// <param name="userName">The target username</param>
        /// <returns></returns>
        Task<Repo_List_Result> Get_User_Repositories(string userName);

        /// <summary>
        /// Method to fetch statistics for a given repository
        /// </summary>
        /// <param name="requestData">Required data for analysis</param>
        /// <returns>Repository statistics</returns>
        Task<Repo_Stat_Result> Get_Repository_Stats(Repo_Stat_Request requestData);

        /// <summary>
        /// Method to fetch the initial commits of a repository
        /// </summary>
        /// <param name="userName">The owner of the target repository</param>
        /// <param name="repoName">The name of the target repository</param>
        /// <returns>Initial commits of a repository</returns>
        Task<List<Repo_Commit>> Get_Repo_Initial_Commits(string userName, string repoName);

        /// <summary>
        /// Method to identify commits that could cause potential bias
        /// </summary>
        /// <param name="requestData">Data required for analysis</param>
        /// <returns>Bias commits</returns>
        Task<Repo_Bias_Result> Get_Repo_Bias(Repo_Stat_Request requestData);
    }

    /// <summary>
    /// Class that contains method bodies for fetching and manipulating GitHub data
    /// </summary>
    public class GitHubService : IGitHubService
    {
        //! Section: Globals

        /// <summary>
        /// GitHub API configuration
        /// </summary>
        private readonly API_Config _config;

        /// <summary>
        /// Service to perform basic REST request
        /// </summary>
        private readonly IRESTService _restService;

        /// <summary>
        /// Service to perform GraphQL requests
        /// </summary>
        private readonly IGraphQLService _graphQLService;

        //! END section: Globals

        //! Section: Methods

        /// <summary>
        /// Constructor of the service that handles dependency injection of required services
        /// </summary>
        /// <param name="config">GitHub API configuration</param>
        /// <param name="restService">Service to perform basic REST request</param>
        /// <param name="graphQLService">Service to perform GraphQL requests</param>
        public GitHubService(IOptions<API_Config> config, IRESTService restService, IGraphQLService graphQLService)
        {
            _config = config.Value;
            _restService = restService;
            _graphQLService = graphQLService;
        }

        /// <summary>
        /// Method to fetch a user's public repositories
        /// </summary>
        /// <param name="userName">The target username</param>
        /// <returns>A user's public repositories</returns>
        public async Task<Repo_List_Result> Get_User_Repositories(string userName)
        {
            // Format the final url
            var url = "https://api.github.com/users/" + userName + "/repos";

            // Perform the GET request with the URL
            var returnObj = await _restService.Perform_REST_GET_Request(url, GitHub_Model_Types.Repo_List_Result);

            // Return the data
            return (Repo_List_Result)returnObj;
        }

        /// <summary>
        /// Method to fetch statistics for a given repository
        /// </summary>
        /// <param name="requestData">Required data for analysis</param>
        /// <returns>Repository statistics</returns>
        public async Task<Repo_Stat_Result> Get_Repository_Stats(Repo_Stat_Request requestData)
        {
            List<Repo_Stat_Info> stats = new List<Repo_Stat_Info>();

            List<Repo_Commit> allCommits = new List<Repo_Commit>();
            
            // Flag to signify if all of a repository's commits have been fetched
            bool allCommitsFetched = false;

            // Variable to track how many fetches have been performed
            var pass = 1;

            // Create list to store the GraphQL "commits"
            List<Node> nodes = new List<Node>();

            // Variable to store the ID of the last commit. Currently set to empty as nothing has been processed
            var after = "";

            // Variable to specify that only commits from the master branch are being considered
            var branch = "master";

            do
            {
                // Vaariable to store the query to be used. Must be populated by conditional statement
                var query = "";

                if (pass != 1)
                {
                    // Set the query to include the after variable so that only commits after this ID are fetched (avoids duplicates)
                    query = "query { repository(name: \"" + requestData.Repo_Name + "\", owner: \"" + requestData.User_Name + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100, after: \"" + after + "\") { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl id } name email date } changedFiles additions deletions } } } } } } } }";
                }
                else
                {
                    // Set the query to not include the after variable as this is the first fetch
                    query = "query { repository(name: \"" + requestData.Repo_Name + "\", owner: \"" + requestData.User_Name + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100) { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl id } name email date } changedFiles additions deletions } } } } } } } }";
                }

                // Get the response for the given GraphQL query
                var response = await _graphQLService.Perform_GraphQL_Request(query, GitHub_Model_Types.GraphQL_Repository_Result);

                // Parse the response
                var parsedResponse = (GraphQLRepositoryResult)response;

                // Increment the number of passes
                pass++;

                // If no error were encountered
                if (parsedResponse.Errors.Count() == 0)
                {
                    // Add the processed commits to the global list
                    nodes.AddRange(parsedResponse.RepositoryInfo.Repository.Ref.Target.History.Edges.Select(x => x.Node));

                    // If more needs to be processed
                    if (parsedResponse.RepositoryInfo.Repository.Ref.Target.History.PageInfo.HasNextPage)
                    {
                        // Populate the after variable with the last ID encountered
                        after = parsedResponse.RepositoryInfo.Repository.Ref.Target.History.PageInfo.EndCursor;
                    }
                    else
                    {
                        // Update the flag so that the loop will be broken. All commits have been fetched
                        allCommitsFetched = true;
                    }
                }
                else
                {
                    break;
                }

            } while (allCommitsFetched == false);

            // Iterate through each commit returned
            foreach (var node in nodes)
            {
                // Add the nodes as a Repo_Commit object for easier processing
                allCommits.Add(new Repo_Commit()
                {
                    Sha = node.Oid,
                    Author = new Author_Info()
                    {
                        Login = node.Author.User.Login,
                        Avatar_Url = node.Author.User.AvatarUrl,
                        Id = node.Author.User.Id
                    },
                    Commit = new Commit()
                    {
                        Message = node.Message,
                        Committer = new Commiter()
                        {
                            Date = node.Author.Date,
                            Email = node.Author.Email,
                            Message = node.Message,
                            Name = node.Author.Name
                        }
                    },
                    Stats = new Commit_Stats()
                    {
                        Additions = node.Additions,
                        Deletions = node.Deletions,
                        Total = (node.Additions + node.Deletions),
                        Changed_Files = node.ChangedFiles
                    }
                });
            }

            // Iterate through each unique author identified
            foreach(var author in allCommits.Select(x => x.Author.Login).Distinct())
            {
                // Get the author's avatar URL and their ID
                var authorUrl = allCommits.FirstOrDefault(x => x.Author.Login == author).Author.Avatar_Url;
                var authorId = allCommits.FirstOrDefault(x => x.Author.Login == author).Author.Id;

                // Fetch all commits by the current author within the time range
                var authorCommits = allCommits.Where(x => x.Author.Login == author && !requestData.Restricted_Commits.Contains(x.Sha) && x.Commit.Committer.Date.Date >= requestData.Start.Date && x.Commit.Committer.Date.Date <= requestData.End.Date).ToList();

                // Create variable to track if the author is below the specified commit threshold
                var belowThreshold = false;

                // Check if the author is below the commit threshold
                if(authorCommits.Count() < requestData.Commit_Threshold)
                {
                    belowThreshold = true;
                }

                // Add the author's statistical information to the stats list
                stats.Add(new Repo_Stat_Info()
                {
                    Author = new Author_Info()
                    {
                        Login = author,
                        Avatar_Url = authorUrl,
                        Id = authorId
                    },
                    Commits = authorCommits,
                    Total = authorCommits.Count(),
                    Additions = authorCommits.Select(x => x.Stats.Additions).Sum(),
                    Deletions = authorCommits.Select(x => x.Stats.Deletions).Sum(),
                    Below_Threshold = belowThreshold
                });
            }

            // Return the stats result
            return new Repo_Stat_Result()
            {
                Stats = stats,
                Status = new Status()
                {
                    Status_Code = 200,
                    Message = "OK"
                }
            };
        }

        /// <summary>
        /// Method to fetch the initial commits of a repository
        /// </summary>
        /// <param name="userName">The owner of the target repository</param>
        /// <param name="repoName">The name of the target repository</param>
        /// <returns>Initial commits of a repository</returns>
        public async Task<List<Repo_Commit>> Get_Repo_Initial_Commits(string userName, string repoName)
        {
            // Create a variable to store the initial commits
            List<Repo_Commit> initCommits = new List<Repo_Commit>();

            // Flag to signify if all of a repository's commits have been fetched
            bool allCommitsFetched = false;

            // Variable to track how many fetches have been performed
            var pass = 1;

            // Create list to store the GraphQL "commits"
            List<Node> nodes = new List<Node>();

            // Variable to store the ID of the last commit. Currently set to empty as nothing has been processed
            var after = "";

            // Variable to specify that only commits from the master branch are being considered
            var branch = "master";

            do
            {
                // Vaariable to store the query to be used. Must be populated by conditional statement
                var query = "";

                if (pass != 1)
                {
                    // Set the query to include the after variable so that only commits after this ID are fetched (avoids duplicates)
                    query = "query { repository(name: \"" + repoName + "\", owner: \"" + userName + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100, after: \"" + after + "\") { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl id } name email date } changedFiles additions deletions } } } } } } } }";
                }
                else
                {
                    // Set the query to not include the after variable as this is the first fetch
                    query = "query { repository(name: \"" + repoName + "\", owner: \"" + userName + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100) { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl id } name email date } changedFiles additions deletions } } } } } } } }";
                }

                // Get the response for the given GraphQL query
                var response = await _graphQLService.Perform_GraphQL_Request(query, GitHub_Model_Types.GraphQL_Repository_Result);

                // Parse the response
                var parsedResponse = (GraphQLRepositoryResult)response;

                // Increment the number of passes
                pass++;

                // If no error were encountered
                if (parsedResponse.Errors.Count() == 0)
                {
                    // Add the processed commits to the global list
                    nodes.AddRange(parsedResponse.RepositoryInfo.Repository.Ref.Target.History.Edges.Select(x => x.Node));

                    // If more needs to be processed
                    if (parsedResponse.RepositoryInfo.Repository.Ref.Target.History.PageInfo.HasNextPage)
                    {
                        // Populate the after variable with the last ID encountered
                        after = parsedResponse.RepositoryInfo.Repository.Ref.Target.History.PageInfo.EndCursor;
                    }
                    else
                    {
                        // Update the flag so that the loop will be broken. All commits have been fetched
                        allCommitsFetched = true;
                    }
                }
                else
                {
                    break;
                }

            } while (allCommitsFetched == false);

            // If any commits were found
            if (nodes.Count() > 0)
            {
                // Order the commits by date
                nodes = nodes.OrderBy(x => x.Author.Date).ToList();

                // Set the number of commits to check as 4 (results in checking first 5 commits, indexes 0-4)
                var checkCount = 4;

                // If the number of commits found was less than 5, set the number to check as the number of commits found to avoid errors
                if (nodes.Count() < 5)
                {
                    checkCount = (nodes.Count() - 1);
                }

                // Iterate through the first few commits of the repository
                for (int i = 0; i <= checkCount; i++)
                {
                    var parsedCommit = new Repo_Commit()
                    {
                        Sha = nodes[i].Oid,
                        Author = new Author_Info()
                        {
                            Login = nodes[i].Author.User.Login,
                            Avatar_Url = nodes[i].Author.User.AvatarUrl
                        },
                        Commit = new Commit()
                        {
                            Message = nodes[i].Message,
                            Committer = new Commiter()
                            {
                                Date = nodes[i].Author.Date,
                                Email = nodes[i].Author.Email,
                                Message = nodes[i].Message,
                                Name = nodes[i].Author.Name
                            }
                        },
                        Stats = new Commit_Stats()
                        {
                            Additions = nodes[i].Additions,
                            Deletions = nodes[i].Deletions,
                            Total = (nodes[i].Additions + nodes[i].Deletions),
                            Changed_Files = nodes[i].ChangedFiles
                        }
                    };

                    initCommits.Add(parsedCommit);
                }
            }

            // Return the formatted commits
            return initCommits;
        }

        /// <summary>
        /// Method to identify commits that could cause potential bias
        /// </summary>
        /// <param name="requestData">Data required for analysis</param>
        /// <returns>Bias commits</returns>
        public async Task<Repo_Bias_Result> Get_Repo_Bias(Repo_Stat_Request requestData)
        {
            // Create variable to store all commits of the repository
            List<Repo_Commit> allCommits = new List<Repo_Commit>();

            // Create a variable to store the result of the bias analysis
            Repo_Bias_Result biasResult = new Repo_Bias_Result()
            {
                GitHub_Commits = new List<Repo_Commit>(),
                Mass_Addition_Commits = new List<Repo_Commit>(),
                Mass_Deletion_Commits = new List<Repo_Commit>()
            };

            // Flag to signify if all of a repository's commits have been fetched
            bool allCommitsFetched = false;

            // Variable to track how many fetches have been performed
            var pass = 1;

            // Create list to store the GraphQL "commits"
            List<Node> nodes = new List<Node>();

            // Variable to store the ID of the last commit. Currently set to empty as nothing has been processed
            var after = "";

            // Variable to specify that only commits from the master branch are being considered
            var branch = "master";

            do
            {
                // Vaariable to store the query to be used. Must be populated by conditional statement
                var query = "";

                if (pass != 1)
                {
                    // Set the query to include the after variable so that only commits after this ID are fetched (avoids duplicates)
                    query = "query { repository(name: \"" + requestData.Repo_Name + "\", owner: \"" + requestData.User_Name + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100, after: \"" + after + "\") { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl id } name email date } changedFiles additions deletions } } } } } } } }";
                }
                else
                {
                    // Set the query to not include the after variable as this is the first fetch
                    query = "query { repository(name: \"" + requestData.Repo_Name + "\", owner: \"" + requestData.User_Name + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100) { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl id } name email date } changedFiles additions deletions } } } } } } } }";
                }

                // Get the response for the given GraphQL query
                var response = await _graphQLService.Perform_GraphQL_Request(query, GitHub_Model_Types.GraphQL_Repository_Result);

                // Parse the response
                var parsedResponse = (GraphQLRepositoryResult)response;

                // Increment the number of passes
                pass++;

                // If no error were encountered
                if (parsedResponse.Errors.Count() == 0)
                {
                    // Add the processed commits to the global list
                    nodes.AddRange(parsedResponse.RepositoryInfo.Repository.Ref.Target.History.Edges.Select(x => x.Node));

                    // If more needs to be processed
                    if (parsedResponse.RepositoryInfo.Repository.Ref.Target.History.PageInfo.HasNextPage)
                    {
                        // Populate the after variable with the last ID encountered
                        after = parsedResponse.RepositoryInfo.Repository.Ref.Target.History.PageInfo.EndCursor;
                    }
                    else
                    {
                        // Update the flag so that the loop will be broken. All commits have been fetched
                        allCommitsFetched = true;
                    }
                }
                else
                {
                    break;
                }

            } while (allCommitsFetched == false);

            // If any commits were found that are within date range and are not already restricted from processing
            if (nodes.Where(x => x.Author.Date.Date >= requestData.Start.Date && x.Author.Date.Date <= requestData.End.Date && !requestData.Restricted_Commits.Contains(x.Oid)).Count() > 0)
            {
                // Iterate through each node and add them to the allCommits variable
                foreach (var node in nodes.Where(x => x.Author.Date.Date >= requestData.Start.Date && x.Author.Date.Date <= requestData.End.Date && !requestData.Restricted_Commits.Contains(x.Oid)))
                {
                    allCommits.Add(new Repo_Commit()
                    {
                        Sha = node.Oid,
                        Author = new Author_Info()
                        {
                            Login = node.Author.User.Login,
                            Avatar_Url = node.Author.User.AvatarUrl
                        },
                        Commit = new Commit()
                        {
                            Message = node.Message,
                            Committer = new Commiter()
                            {
                                Date = node.Author.Date,
                                Email = node.Author.Email,
                                Message = node.Message,
                                Name = node.Author.Name
                            }
                        },
                        Stats = new Commit_Stats()
                        {
                            Additions = node.Additions,
                            Deletions = node.Deletions,
                            Total = (node.Additions + node.Deletions),
                            Changed_Files = node.ChangedFiles
                        }
                    });
                }

                if (allCommits.Count() > 0)
                {
                    // Order the commits by date
                    allCommits = allCommits.OrderBy(x => x.Commit.Committer.Date).ToList();
                    
                    // Iterate through each commit
                    foreach (var analysedCommit in allCommits)
                    {
                        // GitHub Commit
                        if (analysedCommit.Commit.Message.ToLower().Contains("git"))
                        {
                            biasResult.GitHub_Commits.Add(analysedCommit);
                        }

                        // Mass Addition
                        else if (analysedCommit.Stats.Additions > requestData.Addition_Threshold)
                        {
                            biasResult.Mass_Addition_Commits.Add(analysedCommit);
                        }

                        // Mass Deletion
                        else if (analysedCommit.Stats.Deletions > requestData.Deletion_Threshold)
                        {
                            biasResult.Mass_Deletion_Commits.Add(analysedCommit);
                        }
                    }
                }

                // Signify that the repository has commits
                biasResult.Has_Commits = true;
            }
            else
            {
                biasResult.Has_Commits = false;
            }
            
            // Return the result of the bias analysis
            return biasResult;
        }

        //! END Section: Methods
    }
}
