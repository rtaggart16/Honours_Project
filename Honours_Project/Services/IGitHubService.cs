using Honours_Project.Models;
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
    }

    public class GitHubService: IGitHubService
    {
        public async Task<Repo_List_Result> Get_User_Repositories(string userName)
        {
            using (var client = new HttpClient())
            {
                var url = "https://api.github.com/users/" + userName + "/repos";

                try
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "request");

                    var response = await client.GetAsync(url);

                    string content = await response.Content.ReadAsStringAsync();

                    content = content.Replace(@"\", "");

                    List<Simple_Repo_Info> result = JsonConvert.DeserializeObject<List<Simple_Repo_Info>>(content);

                    return new Repo_List_Result()
                    {
                        Repos = result,
                        Status = new Status()
                        {
                            Status_Code = (int)response.StatusCode,
                            Message = response.ReasonPhrase
                        }
                    };
                }
                catch (Exception ex)
                {
                    return new Repo_List_Result()
                    {
                        Repos = new List<Simple_Repo_Info>(),
                        Status = new Status()
                        {
                            Status_Code = 500,
                            Message = "Error ocurred"
                        }
                    };
                }
            }
        }

        public async Task<Repo_Stat_Result> Get_Repository_Stats(string userName, string repoName, DateTime? start, DateTime? end)
        {
            using (var client = new HttpClient())
            {
                var url = "https://api.github.com/repos/" + userName + "/" + repoName + "/stats/contributors";

                try
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "request");

                    var response = await client.GetAsync(url);

                    string content = await response.Content.ReadAsStringAsync();

                    content = content.Replace(@"\", "");

                    List<Repo_Stat_Info> result = JsonConvert.DeserializeObject<List<Repo_Stat_Info>>(content);

                    if(start.HasValue && end.HasValue)
                    {     
                        if(start.Value.Date <= end.Value.Date)
                        {
                            // If the start and the end is the same value, change the end to a week from the start
                            if (start.Value.Date == end.Value.Date)
                            {
                                end = start.Value.AddDays(7);
                            }

                            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                            var fileteredWeeks = new List<Repo_Stat_Info>();

                            foreach (var authorStat in result)
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

                    return new Repo_Stat_Result()
                    {
                        Stats = result,
                        Status = new Status()
                        {
                            Status_Code = (int)response.StatusCode,
                            Message = response.ReasonPhrase
                        }
                    };
                }
                catch (Exception ex)
                {
                    return new Repo_Stat_Result()
                    {
                        Stats = new List<Repo_Stat_Info>(),
                        Status = new Status()
                        {
                            Message = "An Error Occurred",
                            Status_Code = 500
                        }
                    };
                }
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
    }
}
