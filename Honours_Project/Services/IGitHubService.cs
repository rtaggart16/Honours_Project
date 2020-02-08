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

        Task<Repo_Stat_Result> Get_Repository_Stats(string userName, string repoName, DateTime? start, DateTime? end);

        Task<Repo_Commit_Result> Get_Repository_Commits(string userName, string repoName, int pageNumber);

        Task<List<Repo_Commit>> Get_Repo_Bias(string userName, string repoName, int additionThreshold, int deletionThreshold);
    }

    public class GitHubService : IGitHubService
    {
        private readonly API_Config _config;
        private readonly IRESTService _restService;

        public GitHubService(IOptions<API_Config> config, IRESTService restService)
        {
            _config = config.Value;
            _restService = restService;
        }

        public async Task<Repo_List_Result> Get_User_Repositories(string userName)
        {
            var url = "https://api.github.com/users/" + userName + "/repos";

            var returnObj = await _restService.Perform_REST_GET_Request(url, GitHub_Model_Types.Repo_List_Result);

            return (Repo_List_Result)returnObj;
        }

        public async Task<Repo_Stat_Result> Get_Repository_Stats(string userName, string repoName, DateTime? start, DateTime? end)
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

        public async Task<List<Repo_Commit>> Get_Repo_Bias(string userName, string repoName, int additionThreshold, int deletionThreshold)
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

            // 1. Get all commits for repo

            bool commitsFetched = false;

            for (int i = 1; commitsFetched == false; i++)
            {
                var pageCommits = await Get_Repository_Commits(userName, repoName, i);

                if (pageCommits.Commits.Count() > 0)
                {
                    allCommits.AddRange(pageCommits.Commits);
                }
                else
                {
                    commitsFetched = true;
                }
            }

            if (allCommits.Count() > 0)
            {
                allCommits.OrderBy(x => x.Commit.Committer.Date);

                // Check first 5 commits for initial bias

                for (int i = 0; i <= 4; i++)
                {
                    using (var client = new HttpClient())
                    {
                        var url = "https://api.github.com/repos/" + userName + "/" + repoName + "/commits/" + allCommits[i].Sha;

                        client.DefaultRequestHeaders.Add("User-Agent", "request");

                        var response = await client.GetAsync(url);

                        string content = await response.Content.ReadAsStringAsync();

                        content = content.Replace(@"\", "");

                        Repo_Commit commit = JsonConvert.DeserializeObject<Repo_Commit>(content);

                        // Check if the commit is a default git one

                        if (commit.Commit.Message == "Add .gitignore and .gitattributes.")
                        {
                            biasCommits.Add(commit);
                        }

                        // Check if the commit exceeds a large amount of additions

                        if (commit.Stats.Additions > additionThreshold)
                        {
                            biasCommits.Add(commit);
                        }

                        // Check if the commit exceeds a large amount of deletions

                        if (commit.Stats.Deletions > deletionThreshold)
                        {
                            biasCommits.Add(commit);
                        }
                    }
                }
            }

            return biasCommits;
        }
    }
}
