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
    public interface IGitHubService
    {
        Task<Repo_List_Result> Get_User_Repositories(string userName);

        Task<Repo_Stat_Result> Get_Repository_Stats(Repo_Stat_Request requestData);

        Task<Repo_Commit_Result> Get_Repository_Commits(string userName, string repoName, int pageNumber);

        Task<List<Repo_Commit>> Get_Repo_Initial_Commits(string userName, string repoName);

        Task<Repo_Bias_Result> Get_Repo_Bias(Repo_Stat_Request requestData);
    }

    public class GitHubService : IGitHubService
    {
        private readonly API_Config _config;
        private readonly IRESTService _restService;
        private readonly IGraphQLService _graphQLService;

        public GitHubService(IOptions<API_Config> config, IRESTService restService, IGraphQLService graphQLService)
        {
            _config = config.Value;
            _restService = restService;
            _graphQLService = graphQLService;
        }

        public async Task<Repo_List_Result> Get_User_Repositories(string userName)
        {
            var url = "https://api.github.com/users/" + userName + "/repos";

            var returnObj = await _restService.Perform_REST_GET_Request(url, GitHub_Model_Types.Repo_List_Result);

            return (Repo_List_Result)returnObj;
        }

        /*public async Task<Repo_Stat_Result> Get_Repository_Stats(string userName, string repoName, DateTime? start, DateTime? end)
        {
            var url = "https://api.github.com/repos/" + userName + "/" + repoName + "/stats/contributors";

            var returnObj = await _restService.Perform_REST_GET_Request(url, GitHub_Model_Types.Repo_Stat_Result);

            var parsedObj = (Repo_Stat_Result)returnObj;

            if (start.HasValue && end.HasValue && parsedObj.Status.Status_Code == 200)
            {
                if (start.Value.Date <= end.Value.Date)
                {
                    // If the start and the end is the same value, change the end to a week from the start
                    if (start.Value.Date == end.Value.Date)
                    {
                        end = start.Value.AddDays(7);
                    }

                    DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                    var fileteredWeeks = new List<Repo_Stat_Info>();

                    foreach (var authorStat in parsedObj.Stats)
                    {
                        var authorWeeks = new List<Week>();

                        var authorTotal = 0;

                        foreach (var week in authorStat.Weeks)
                        {
                            var unixW = double.Parse(week.W);

                            var dateVal = dt.AddSeconds(unixW);

                            if (dateVal >= start && dateVal < end)
                            {
                                authorWeeks.Add(week);

                                authorTotal += week.C;
                            }
                        }

                        authorStat.Weeks = authorWeeks;
                        authorStat.Total = authorTotal;
                    }

                    return new Repo_Stat_Result()
                    {
                        Stats = parsedObj.Stats,
                        Status = parsedObj.Status
                    };
                }
                else
                {
                    return new Repo_Stat_Result()
                    {
                        Stats = new List<Repo_Stat_Info>(),
                        Status = new Status()
                        {
                            Status_Code = 500,
                            Message = "Start can't be greater than end"
                        }
                    };
                }
            }
            else
            {
                return parsedObj;
            }
        }*/

        public async Task<Repo_Stat_Result> Get_Repository_Stats(Repo_Stat_Request requestData)
        {
            List<Repo_Stat_Info> stats = new List<Repo_Stat_Info>();

            List<Repo_Commit> allCommits = new List<Repo_Commit>();

            // 1. Get all commits for repo

            bool allCommitsFetched = false;

            var pass = 1;

            List<Node> nodes = new List<Node>();

            var after = "";

            var branch = "master";

            do
            {
                var query = "";

                if (pass != 1)
                {
                    query = "query { repository(name: \"" + requestData.Repo_Name + "\", owner: \"" + requestData.User_Name + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100, after: \"" + after + "\") { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl id } name email date } changedFiles additions deletions } } } } } } } }";
                }
                else
                {
                    query = "query { repository(name: \"" + requestData.Repo_Name + "\", owner: \"" + requestData.User_Name + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100) { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl id } name email date } changedFiles additions deletions } } } } } } } }";
                }

                var response = await _graphQLService.Perform_GraphQL_Request(query, GitHub_Model_Types.GraphQL_Repository_Result);

                var parsedResponse = (GraphQLRepositoryResult)response;

                pass++;

                if (parsedResponse.Errors.Count() == 0)
                {
                    nodes.AddRange(parsedResponse.RepositoryInfo.Repository.Ref.Target.History.Edges.Select(x => x.Node));

                    if (parsedResponse.RepositoryInfo.Repository.Ref.Target.History.PageInfo.HasNextPage)
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

            foreach (var node in nodes)
            {
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

            foreach(var author in allCommits.Select(x => x.Author.Login).Distinct())
            {
                var authorUrl = allCommits.FirstOrDefault(x => x.Author.Login == author).Author.Avatar_Url;
                var authorId = allCommits.FirstOrDefault(x => x.Author.Login == author).Author.Id;

                var authorCommits = allCommits.Where(x => x.Author.Login == author && !requestData.Restricted_Commits.Contains(x.Sha) && x.Commit.Committer.Date.Date >= requestData.Start.Date && x.Commit.Committer.Date.Date <= requestData.End.Date).ToList();

                var belowThreshold = false;

                if(authorCommits.Count() < requestData.Commit_Threshold)
                {
                    belowThreshold = true;
                }

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

        public async Task<Repo_Commit_Result> Get_Repository_Commits(string userName, string repoName, int pageNumber)
        {
            using (var client = new HttpClient())
            {
                var url = "https://api.github.com/repos/" + userName + "/" + repoName + "/commits?page=" + pageNumber;

                try
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "request");

                    var response = await client.GetAsync(url);

                    string content = await response.Content.ReadAsStringAsync();

                    content = content.Replace(@"\", "");

                    List<Repo_Commit> commits = JsonConvert.DeserializeObject<List<Repo_Commit>>(content);

                    return new Repo_Commit_Result()
                    {
                        Commits = commits,
                        Status = new Status()
                        {
                            Status_Code = (int)response.StatusCode,
                            Message = response.ReasonPhrase
                        }
                    };
                }
                catch (Exception ex)
                {
                    return new Repo_Commit_Result()
                    {
                        Commits = new List<Repo_Commit>(),
                        Status = new Status()
                        {
                            Message = "An Error Occurred",
                            Status_Code = 500
                        }
                    };
                }
            }
        }

        public async Task<List<Repo_Commit>> Get_Repo_Initial_Commits(string userName, string repoName)
        {
            List<Repo_Commit> initCommits = new List<Repo_Commit>();

            bool allCommitsFetched = false;

            var pass = 1;

            List<Node> nodes = new List<Node>();

            var after = "";

            var branch = "master";

            do
            {
                var query = "";

                if (pass != 1)
                {
                    query = "query { repository(name: \"" + repoName + "\", owner: \"" + userName + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100, after: \"" + after + "\") { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl } name email date } changedFiles additions deletions } } } } } } } }";
                }
                else
                {
                    query = "query { repository(name: \"" + repoName + "\", owner: \"" + userName + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100) { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl } name email date } changedFiles additions deletions } } } } } } } }";
                }

                var response = await _graphQLService.Perform_GraphQL_Request(query, GitHub_Model_Types.GraphQL_Repository_Result);

                var parsedResponse = (GraphQLRepositoryResult)response;

                pass++;

                if (parsedResponse.Errors.Count() == 0 && parsedResponse.RepositoryInfo.Repository.Ref != null)
                {
                    nodes.AddRange(parsedResponse.RepositoryInfo.Repository.Ref.Target.History.Edges.Select(x => x.Node));

                    if (parsedResponse.RepositoryInfo.Repository.Ref.Target.History.PageInfo.HasNextPage)
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

            if(nodes.Count() > 0)
            {
                nodes = nodes.OrderBy(x => x.Author.Date).ToList();

                var checkCount = 4;

                if (nodes.Count() < 5)
                {
                    checkCount = (nodes.Count() - 1);
                }

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

            return initCommits;
        }

        public async Task<Repo_Bias_Result> Get_Repo_Bias(Repo_Stat_Request requestData)
        {
            /*
             * Tasks:
             *      1. Get all commits for repo
             *      2. Find initial commits
             *      3. Find mass additions
             *      4. Find mass deletions
            */

            List<Repo_Commit> allCommits = new List<Repo_Commit>();

            List<Repo_Commit> biasCommits = new List<Repo_Commit>();

            Repo_Bias_Result biasResult = new Repo_Bias_Result()
            {
                GitHub_Commits = new List<Repo_Commit>(),
                Mass_Addition_Commits = new List<Repo_Commit>(),
                Mass_Deletion_Commits = new List<Repo_Commit>()
            };

            // 1. Get all commits for repo

            bool allCommitsFetched = false;

            var pass = 1;

            List<Node> nodes = new List<Node>();

            var after = "";

            var branch = "master";

            do
            {
                var query = "";

                if (pass != 1)
                {
                    query = "query { repository(name: \"" + requestData.Repo_Name + "\", owner: \"" + requestData.User_Name + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100, after: \"" + after + "\") { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl } name email date } changedFiles additions deletions } } } } } } } }";
                }
                else
                {
                    query = "query { repository(name: \"" + requestData.Repo_Name + "\", owner: \"" + requestData.User_Name + "\") { ref(qualifiedName: \"" + branch + "\") { target { ... on Commit { id history(first: 100) { pageInfo { hasNextPage, endCursor } edges { node { messageHeadline oid message author { user { login avatarUrl } name email date } changedFiles additions deletions } } } } } } } }";
                }

                var response = await _graphQLService.Perform_GraphQL_Request(query, GitHub_Model_Types.GraphQL_Repository_Result);

                var parsedResponse = (GraphQLRepositoryResult)response;

                pass++;

                if (parsedResponse.Errors.Count() == 0 && parsedResponse.RepositoryInfo.Repository.Ref != null)
                {
                    nodes.AddRange(parsedResponse.RepositoryInfo.Repository.Ref.Target.History.Edges.Select(x => x.Node));

                    if (parsedResponse.RepositoryInfo.Repository.Ref.Target.History.PageInfo.HasNextPage)
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

            if(nodes.Where(x => x.Author.Date.Date >= requestData.Start.Date && x.Author.Date.Date <= requestData.End.Date && !requestData.Restricted_Commits.Contains(x.Oid)).Count() > 0)
            {
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
                    allCommits = allCommits.OrderBy(x => x.Commit.Committer.Date).ToList();

                    // Check first 5 commits for initial bias

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

                biasResult.Has_Commits = true;
            }
            else
            {
                biasResult.Has_Commits = false;
            }
            
            return biasResult;
        }
    }
}
