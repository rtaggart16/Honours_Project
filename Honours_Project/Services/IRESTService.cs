using Honours_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Honours_Project.Services
{
    public interface IRESTService
    {
        Task<object> Perform_REST_GET_Request(string url, GitHub_Model_Types objectType);
    }

    public class RESTService : IRESTService
    {
        public async Task<object> Perform_REST_GET_Request(string url, GitHub_Model_Types objectType)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "request");

                    var response = await client.GetAsync(url);

                    return await Convert_Response_Data(response, objectType);
                }
            }
            catch (Exception ex)
            {
                // Should never be hit
                return new object();
            }
        }

        private async Task<object> Convert_Response_Data(HttpResponseMessage response, GitHub_Model_Types objectType)
        {
            object returnObject = new object();

            string content = await response.Content.ReadAsStringAsync();

            switch (objectType)
            {
                case GitHub_Model_Types.Repo_List_Result:
                    try
                    {
                        var repoListResult = JsonConvert.DeserializeObject<List<Simple_Repo_Info>>(content);

                        returnObject = new Repo_List_Result()
                        {
                            Repos = repoListResult,
                            Status = new Status()
                            {
                                Status_Code = (int)response.StatusCode,
                                Message = response.ReasonPhrase
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        returnObject = new Repo_List_Result()
                        {
                            Repos = new List<Simple_Repo_Info>(),
                            Status = new Status()
                            {
                                Status_Code = (int)response.StatusCode,
                                Message = response.ReasonPhrase
                            }
                        };
                    }
                    break;

                case GitHub_Model_Types.Repo_Stat_Result:
                    try
                    {
                        var repoStatResult = JsonConvert.DeserializeObject<List<Repo_Stat_Info>>(content);

                        returnObject = new Repo_Stat_Result()
                        {
                            Stats = repoStatResult,
                            Status = new Status()
                            {
                                Status_Code = (int)response.StatusCode,
                                Message = response.ReasonPhrase
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        returnObject = new Repo_Stat_Result()
                        {
                            Stats = new List<Repo_Stat_Info>(),
                            Status = new Status()
                            {
                                Status_Code = (int)response.StatusCode,
                                Message = response.ReasonPhrase
                            }
                        };
                    }
                    break;
            }

            return returnObject;
        }
    }
}
