/*
    Name: Ross Taggart
    ID: S1828840
*/

using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Honours_Project.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honours_Project.Services
{
    /// <summary>
    /// Interface that contains method definitions for fetching GraphQL data
    /// </summary>
    public interface IGraphQLService
    {
        /// <summary>
        /// Method to perform a GraphQL request for a given query, before formatting the data into a given type
        /// </summary>
        /// <param name="query">The query to execute</param>
        /// <param name="objectType">Enum that specifies the target parse type</param>
        /// <returns>GraphQLResponse formatted in the type specified</returns>
        Task<object> Perform_GraphQL_Request(string query, GitHub_Model_Types objectType);
    }

    /// <summary>
    /// Class that contains method bodies for fetching GraphQL data
    /// </summary>
    public class GraphQLService : IGraphQLService
    {
        //! Section Globals

        /// <summary>
        /// GitHub API configuration
        /// </summary>
        private readonly API_Config _config;

        //! END Section: Globals

        //! Section: Methods

        /// <summary>
        /// Constructor of the service that handles dependency injection of API secrets
        /// </summary>
        /// <param name="config">GitHub API configuration</param>
        public GraphQLService(IOptions<API_Config> config)
        {
            _config = config.Value;
        }

        /// <summary>
        /// Method to perform a GraphQL request for a given query, before formatting the data into a given type
        /// </summary>
        /// <param name="query">The query to execute</param>
        /// <param name="objectType">Enum that specifies the target parse type</param>
        /// <returns>GraphQLResponse formatted in the type specified</returns>
        public async Task<object> Perform_GraphQL_Request(string query, GitHub_Model_Types objectType)
        {
            // Create a new GraphQL request with the target query
            var commitData = new GraphQLRequest
            {
                Query = query
            };

            // Create a new GraphQL client that refernces the GitHub API v4 endpoint as a base URL
            var client = new GraphQLClient("https://api.github.com/graphql");

            // Add a "User-Agent" header to identify the request
            client.DefaultRequestHeaders.Add("User-Agent", "request");

            // Add the GitHub API token included in secrets as a Bearer authorisation token. Required to authorise the request
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.Access_Token);

            // Get the request response
            var response = await client.PostAsync(commitData);

            // Convert the data to the target type and return
            return Convert_GraphQL_Response_Data(response, objectType);
        }

        /// <summary>
        /// Method to parse the result of a GraphQL request into a specified object type
        /// </summary>
        /// <param name="response">The GraphQL response</param>
        /// <param name="objectType">The type of object to parse the data into</param>
        /// <returns>Parsed response data</returns>
        private object Convert_GraphQL_Response_Data(GraphQLResponse response, GitHub_Model_Types objectType)
        {
            // Create a generic object to return the data as (caller method can handle appropriate casting)
            object returnObject = new object();

            // Convert the GraphQL to a generic object
            var data = (object)response.Data;

            // Convert to string
            var dataString = data.ToString();

            // Check the type of object to parse the data into
            switch(objectType)
            {
                // If the target type is a Repository_Result object 
                case GitHub_Model_Types.GraphQL_Repository_Result:
                    // If there were any errors returned from the request
                    if(response.Errors != null)
                    {
                        // Return new object that contains no data and details the encountered errors
                        returnObject = new GraphQLRepositoryResult()
                        {
                            RepositoryInfo = null,
                            Errors = response.Errors.ToList()
                        };
                    }
                    else
                    {
                        // Wrap in try-catch to handle unexpected errors
                        try
                        {
                            // Convert the string into a GraphQLRepositoryInfo class
                            GraphQLRepositoryInfo repoInfo = JsonConvert.DeserializeObject<GraphQLRepositoryInfo>(dataString);

                            // Create a new object that includes the parsed data, and no errors
                            returnObject = new GraphQLRepositoryResult()
                            {
                                RepositoryInfo = repoInfo,
                                Errors = new List<GraphQLError>()
                            };
                        }
                        catch (Exception ex)
                        {
                            // Create a new object that specifies the data could not be parsed as expected
                            returnObject = new GraphQLRepositoryResult()
                            {
                                RepositoryInfo = null,
                                Errors = new List<GraphQLError>() { new GraphQLError() { Message = "The response could not be parsed" } }
                            };
                        }
                    }
                    break;
            }

            // Return the parsed object
            return returnObject;
        }

        //! END Section: Methods
    }
}
