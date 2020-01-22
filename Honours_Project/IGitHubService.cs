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
        //Task<Repo_List_Result> Get_User_Repositories(string userName);
        Task<string> Get_User_Repositories(string userName);
    }

    public class GitHubService: IGitHubService
    {
        /*public async Task<Repo_List_Result> Get_User_Repositories(string userName)
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
        }*/

        public async Task<string> Get_User_Repositories(string userName)
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

                    return content;
                }
                catch (Exception ex)
                {
                    return "An error occurred";
                }
            }
        }
    }
}
