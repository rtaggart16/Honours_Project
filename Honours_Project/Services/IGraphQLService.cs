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
    public interface IGraphQLService
    {
        Task<object> Perform_GraphQL_Request(string query, GitHub_Model_Types objectType);
    }

    public class GraphQLService : IGraphQLService
    {
        private readonly API_Config _config;

        public GraphQLService(IOptions<API_Config> config)
        {
            _config = config.Value;
        }

        public async Task<object> Perform_GraphQL_Request(string query, GitHub_Model_Types objectType)
        {
            var commitData = new GraphQLRequest
            {
                Query = query
            };

            var client = new GraphQLClient("https://api.github.com/graphql");

            client.DefaultRequestHeaders.Add("User-Agent", "request");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.Access_Token);

            var response = await client.PostAsync(commitData);

            return Convert_GraphQL_Response_Data(response, objectType);
        }

        private object Convert_GraphQL_Response_Data(GraphQLResponse response, GitHub_Model_Types objectType)
        {
            object returnObject = new object();

            var data = (object)response.Data;

            var dataString = data.ToString();

            switch(objectType)
            {
                case GitHub_Model_Types.GraphQL_Repository_Result:
                    if(response.Errors != null)
                    {
                        returnObject = new GraphQLRepositoryResult()
                        {
                            RepositoryInfo = null,
                            Errors = response.Errors.ToList()
                        };
                    }
                    else
                    {
                        try
                        {
                            GraphQLRepositoryInfo repoInfo = JsonConvert.DeserializeObject<GraphQLRepositoryInfo>(dataString);

                            returnObject = new GraphQLRepositoryResult()
                            {
                                RepositoryInfo = repoInfo,
                                Errors = new List<GraphQLError>()
                            };
                        }
                        catch (Exception ex)
                        {
                            returnObject = new GraphQLRepositoryResult()
                            {
                                RepositoryInfo = null,
                                Errors = new List<GraphQLError>() { new GraphQLError() { Message = "The response could not be parsed" } }
                            };
                        }
                    }
                    break;
            }

            return returnObject;
        }
    }
}
