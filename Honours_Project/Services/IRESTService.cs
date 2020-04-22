/*
    Name: Ross Taggart
    ID: S1828840
*/

using Honours_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Honours_Project.Services
{
    /// <summary>
    /// Interface that contains method definitions for fetching REST data
    /// </summary>
    public interface IRESTService
    {
        /// <summary>
        /// Method to perform a GET request using the specified URL, before parsing the response as a given data type
        /// </summary>
        /// <param name="url">The URL to send the request to</param>
        /// <param name="objectType">Enum that specifies the type of object to parse the response into</param>
        /// <returns>Reponse data formatted in the type specified</returns>
        Task<object> Perform_REST_GET_Request(string url, GitHub_Model_Types objectType);
    }

    /// <summary>
    /// Class that contains method bodies for fetching REST data
    /// </summary>
    public class RESTService : IRESTService
    {
        //! Section: Methods

        /// <summary>
        /// Method to perform a GET request using the specified URL, before parsing the response as a given data type
        /// </summary>
        /// <param name="url">The URL to send the request to</param>
        /// <param name="objectType">Enum that specifies the type of object to parse the response into</param>
        /// <returns>Reponse data formatted in the type specified</returns>
        public async Task<object> Perform_REST_GET_Request(string url, GitHub_Model_Types objectType)
        {
            // Wrap in try-catch to handle unexpected errors
            try
            {
                // Create a new HttpClient to handle HTTP communication
                using (var client = new HttpClient())
                {
                    // Add a "User-Agent" header to identify the request
                    client.DefaultRequestHeaders.Add("User-Agent", "request");

                    // Submit the GET request to the target URL and await the response
                    var response = await client.GetAsync(url);

                    // Convert the data to the target type and return
                    return await Convert_Response_Data(response, objectType);
                }
            }
            catch (Exception ex)
            {
                // Should never be hit
                return new object();
            }
        }

        /// <summary>
        /// Method to parse the result of a HTTP request into a specified object type
        /// </summary>
        /// <param name="response">The HTTP data to parse</param>
        /// <param name="objectType">The type of object to parse the data into</param>
        /// <returns>Parsed response data</returns>
        private async Task<object> Convert_Response_Data(HttpResponseMessage response, GitHub_Model_Types objectType)
        {
            // Create a generic object to return the data as (caller method can handle appropriate casting)
            object returnObject = new object();

            // Get the string representation of the body content
            string content = await response.Content.ReadAsStringAsync();

            // Check the type of data to parse into
            switch (objectType)
            {
                // If the target type is a Repo_List_Result object 
                case GitHub_Model_Types.Repo_List_Result:
                    // Wrap in try-catch to handle unexpected errors
                    try
                    {
                        // Convert the string into a list of the Simple_Repo_Info class
                        var repoListResult = JsonConvert.DeserializeObject<List<Simple_Repo_Info>>(content);

                        // Create a new object that includes the data and the original status code and message
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
                        // Create a new object that indicates something went wrong
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

                // If the target type is a Repo_Stat_Result object 
                case GitHub_Model_Types.Repo_Stat_Result:
                    try
                    {
                        // Convert the string into a list of the Repo_Stat_Info class
                        var repoStatResult = JsonConvert.DeserializeObject<List<Repo_Stat_Info>>(content);

                        // Create a new object that includes the data and the original status code and message
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
                        // Create a new object that indicates something went wrong
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

            // Return the parsed data
            return returnObject;
        }

        //! END Section: Methods
    }
}
