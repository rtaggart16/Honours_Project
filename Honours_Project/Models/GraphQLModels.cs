/*
    Name: Ross Taggart
    ID: S1828840
*/

using System;
using System.Collections.Generic;

namespace Honours_Project.Models
{
    //! Section: Models

    /// <summary>
    /// Class that contains information about the repository fetch
    /// </summary>
    public class GraphQLRepositoryResult
    {
        /// <summary>
        /// Information specifically related to the repository
        /// </summary>
        public GraphQLRepositoryInfo RepositoryInfo { get; set; }

        /// <summary>
        /// Any errors encountered during the fetch
        /// </summary>
        public List<GraphQL.Common.Response.GraphQLError> Errors { get; set; }
    }

    /// <summary>
    /// Class that contains information about the fetched repository
    /// </summary>
    public class GraphQLRepositoryInfo
    {
        /// <summary>
        /// The repository
        /// </summary>
        public GraphQLRepository Repository { get; set; }
    }

    /// <summary>
    /// Class that contains information about a repository reference
    /// </summary>
    public class GraphQLRepository
    {
        /// <summary>
        /// Repository reference
        /// </summary>
        public Ref Ref { get; set; }
    }

    /// <summary>
    /// Class that contains information about the target repository
    /// </summary>
    public class Ref
    {
        /// <summary>
        /// Repository reference
        /// </summary>
        public Target Target { get; set; }
    }

    /// <summary>
    /// Class that contains information about the history of the repository
    /// </summary>
    public class Target
    {
        /// <summary>
        /// ID of the repository
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// History of the repository
        /// </summary>
        public History History { get; set; }
    }

    /// <summary>
    /// Class that contains information about the history of the repository
    /// </summary>
    public class History
    {
        /// <summary>
        /// Information about the current page of results
        /// </summary>
        public Page_Info PageInfo { get; set; }

        /// <summary>
        /// List of commits
        /// </summary>
        public List<Edge> Edges { get; set; }
    }

    /// <summary>
    /// Class that contains information about the current page of results
    /// </summary>
    public class Page_Info
    {
        /// <summary>
        /// Boolean flag that signifies if there is another page to fetch
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// ID of the last fetched commit
        /// </summary>
        public string EndCursor { get; set; }
    }

    /// <summary>
    /// Class that contains information about a commit
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Commit identifier
        /// </summary>
        public Node Node { get; set; }
    }

    /// <summary>
    /// Class that contains information about a commit
    /// </summary>
    public class Node
    {
        /// <summary>
        /// The displayed headline of the commit
        /// </summary>
        public string MessageHeadline { get; set; }

        /// <summary>
        /// The ID of the commit
        /// </summary>
        public string Oid { get; set; }

        /// <summary>
        /// The full message of the commit
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Information on the author of the commit
        /// </summary>
        public Author Author { get; set; }

        /// <summary>
        /// Total number of changed files in commit
        /// </summary>
        public int ChangedFiles { get; set; }

        /// <summary>
        /// Total number of additions in commit
        /// </summary>
        public int Additions { get; set; }

        /// <summary>
        /// Total number of deletions in commit
        /// </summary>
        public int Deletions { get; set; }
    }

    /// <summary>
    /// Class that holds information about the author of a commit
    /// </summary>
    public class Author
    {
        /// <summary>
        /// User information
        /// </summary>
        public Commit_Author_Info User { get; set; }

        /// <summary>
        /// Display name of the author
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email of the author
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Date of the commit
        /// </summary>
        public DateTime Date { get; set; }
    }

    /// <summary>
    /// Class that contains streamlined information about a commit author
    /// </summary>
    public class Commit_Author_Info
    {
        /// <summary>
        /// Username of the author
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// URL of the user's profile picture
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// ID of the user
        /// </summary>
        public string Id { get; set; }
    }

    //! END Section: Models
}
