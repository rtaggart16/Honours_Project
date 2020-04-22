/*
    Name: Ross Taggart
    ID: S1828840
*/

using System;
using System.Collections.Generic;

namespace Honours_Project.Models
{
    //! Section: Contents

    /*
     * - Top-level models
     *      - Repo_List_Result
     *      - Repo_Stat_Result
     *      - Repo_Commit_Result
     *      - Repo_Bias_Result
     *      - Repo_Stat_Request
     * - Sub-level models
     *      - Simple_Repo_Info
     *      - Repo_Stat_Info
     *      - Week
     *      - Author_Info
     *      - Repo_Commit
     *      - Commit
     *      - Committer
    */

    //! END Section: Contents


    //! Section: Top-level models

    /// <summary>
    /// Class that contains the result of a repository fetch for a user
    /// </summary>
    public class Repo_List_Result
    {
        /// <summary>
        /// Formatted list of a user's repos
        /// </summary>
        public List<Simple_Repo_Info> Repos { get; set; }

        /// <summary>
        /// Provides a shorthand summary of the response
        /// </summary>
        public Status Status { get; set; }
    }

    /// <summary>
    /// Class that contains the result of a repo stat fetch
    /// </summary>
    public class Repo_Stat_Result
    {
        /// <summary>
        /// List of stats for each collaborator in the repo
        /// </summary>
        public List<Repo_Stat_Info> Stats { get; set; }

        /// <summary>
        /// Provides a shorthand summary of the response
        /// </summary>
        public Status Status { get; set; }
    }

    /// <summary>
    /// Class that contains the result of the repo commits fetch
    /// </summary>
    public class Repo_Commit_Result
    {
        /// <summary>
        /// List of commits for the specified page of the repository
        /// </summary>
        public List<Repo_Commit> Commits { get; set; }

        /// <summary>
        /// Provides a shorthand summary of the response
        /// </summary>
        public Status Status { get; set; }
    }

    /// <summary>
    /// Class that contains the result of a bias check
    /// </summary>
    public class Repo_Bias_Result
    {
        /// <summary>
        /// List of GitHub related commits that could cause bias
        /// </summary>
        public List<Repo_Commit> GitHub_Commits { get; set; }

        /// <summary>
        /// List of commits that exceed the user's addition threshold that could cause bias
        /// </summary>
        public List<Repo_Commit> Mass_Addition_Commits { get; set; }

        /// <summary>
        /// List of commits that exceed the user's deletion threshold that could cause bias
        /// </summary>
        public List<Repo_Commit> Mass_Deletion_Commits { get; set; }

        /// <summary>
        /// Boolean flag to track if a repository has any commits
        /// </summary>
        public bool Has_Commits { get; set; }
    }

    /// <summary>
    /// Class that contains the options for fetching statistics for a repository
    /// </summary>
    public class Repo_Stat_Request
    {
        /// <summary>
        /// The name of the owner of the target repository
        /// </summary>
        public string User_Name { get; set; }

        /// <summary>
        /// The name of the target repository
        /// </summary>
        public string Repo_Name { get; set; }

        /// <summary>
        /// The threshold for addition bias
        /// </summary>
        public int Addition_Threshold { get; set; }

        /// <summary>
        /// The threshold for deletion bias
        /// </summary>
        public int Deletion_Threshold { get; set; }

        /// <summary>
        /// The threshold for blow-commit threshold
        /// </summary>
        public int Commit_Threshold { get; set; }

        /// <summary>
        /// The date to start analysis
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// The date to end analysis
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// List of SHAs of the restricted commits
        /// </summary>
        public List<string> Restricted_Commits { get; set; }
    }

    //! END Section: Top-level models


    //! Section: Sub-level models

    /// <summary>
    /// Class that stores the basic information about a repository required for further queries
    /// </summary>
    public class Simple_Repo_Info
    {
        /// <summary>
        /// The numerical identifier of the repository. Specified by GitHub
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The shorthand name of the repository such as "Test_Repository"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The full name of the repository, including owner, such as "rtaggart16/Test_Repository"
        /// </summary>
        public string Full_Name { get; set; }

        /// <summary>
        /// Whether or not the repository is publicly accessible
        /// </summary>
        public bool Private { get; set; }
    }

    /// <summary>
    /// Class that contains the stats of a collaborator to a repository
    /// </summary>
    public class Repo_Stat_Info
    {
        /// <summary>
        /// The total number of commits a user has performed
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// The total number of additions a user has performed
        /// </summary>
        public int Additions { get; set; }

        /// <summary>
        /// The total number of deletions a user has performed
        /// </summary>
        public int Deletions { get; set; }

        /// <summary>
        /// Boolean flag to track if a user is below the commit threshold specified
        /// </summary>
        public bool Below_Threshold { get; set; }

        /// <summary>
        /// List of all the commits a user has performed
        /// </summary>
        public List<Repo_Commit> Commits { get; set; }

        /// <summary>
        /// Object that contains details about a collaborator
        /// </summary>
        public Author_Info Author { get; set; }
    }

    /// <summary>
    /// Class that contains weekly stats for a collaborator to a repo
    /// </summary>
    public class Week
    {
        /// <summary>
        /// EPOCH representation of the week beginning the stats are taken from
        /// </summary>
        public string W { get; set; }

        /// <summary>
        /// The number of additions a collaborator contributed
        /// </summary>
        public int A { get; set; }

        /// <summary>
        /// The number of deletions a collaborator contributed
        /// </summary>
        public int D { get; set; }

        /// <summary>
        /// The number of commits a collaborator contributed
        /// </summary>
        public int C { get; set; }
    }

    /// <summary>
    /// Class that stores information about the author/collaborator of a repository
    /// </summary>
    public class Author_Info
    {
        /// <summary>
        /// The author's username e.g. "rtaggart16"
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// The numerical identifier of the author. Specified by GitHub
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The identifier of the Node. Specified by GitHub
        /// </summary>
        public string Node_Id { get; set; }

        /// <summary>
        /// The URL of the author's avatar image
        /// </summary>
        public string Avatar_Url { get; set; }

        /// <summary>
        /// The URL of the author's publicly accessible account page
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// A truncated version of the author's publicly accessible account page
        /// </summary>
        public string Html_Url { get; set; }

        /// <summary>
        /// The URL of the page that displays an author's followers
        /// </summary>
        public string Followers_Url { get; set; }

        /// <summary>
        /// The URL of the page that displays who an author is following
        /// </summary>
        public string Following_Url { get; set; }

        /// <summary>
        /// The URL of the page that displays an author's gists
        /// </summary>
        public string Gists_Url { get; set; }

        /// <summary>
        /// The URL of the page that displays all of the repos that an author has starred
        /// </summary>
        public string Starred_url { get; set; }

        /// <summary>
        /// The URL of the page that displays all of the repos that an author subscribes to updates from
        /// </summary>
        public string Subscriptions_Url { get; set; }

        /// <summary>
        /// The URL of the page that displays information about an author's organisation (if they belong to one)
        /// </summary>
        public string Organizations_Url { get; set; }

        /// <summary>
        /// The URL of the page that displays an author's repos
        /// </summary>
        public string Repos_Url { get; set; }

        /// <summary>
        /// The URL of the page that displays an author's events
        /// </summary>
        public string Events_Url { get; set; }

        /// <summary>
        /// The URL of the page that displays an author's received events
        /// </summary>
        public string Received_Events_Url { get; set; }

        /// <summary>
        /// Textual representation of an author's type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Flag of whether an author is an admin
        /// </summary>
        public bool Site_Admin { get; set; }
    }

    /// <summary>
    /// Class that contains the most important information about every commit in a repo
    /// </summary>
    public class Repo_Commit
    {
        /// <summary>
        /// The unique identifier of the commit
        /// </summary>
        public string Sha { get; set; }

        /// <summary>
        /// Top-level information about a commit
        /// </summary>
        public Commit Commit { get; set; }

        /// <summary>
        /// Information about the author of the repo the commit belongs to
        /// </summary>
        public Author_Info Author { get; set; }

        /// <summary>
        /// The stats of the commit
        /// </summary>
        public Commit_Stats Stats { get; set; }
    }

    /// <summary>
    /// Class that contains top-level information about a commit
    /// </summary>
    public class Commit
    {
        /// <summary>
        /// Information about the collaborator who made the commit
        /// </summary>
        public Commiter Committer { get; set; }

        /// <summary>
        /// The message given by the collaborator
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Class that contains the additions and deletions of a commit
    /// </summary>
    public class Commit_Stats
    {
        /// <summary>
        /// The total changes made by the commit
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// The additions made by the commit
        /// </summary>
        public int Additions { get; set; }

        /// <summary>
        /// The deletions made by the commit
        /// </summary>
        public int Deletions { get; set; }

        public int Changed_Files { get; set; }
    }

    /// <summary>
    /// Class that contains information about the collaborator that made the commit
    /// </summary>
    public class Commiter
    {
        /// <summary>
        /// The username of the collaborator i.e. "rtaggart16"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The email of the collaborator
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The date the collaborator made the commit
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The message the collaborator gave the commit
        /// </summary>
        public string Message { get; set; }
    }

    //! END Section: Sub-level models
}
