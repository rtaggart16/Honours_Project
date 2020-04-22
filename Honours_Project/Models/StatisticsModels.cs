/*
    Name: Ross Taggart
    ID: S1828840
*/

using System;

namespace Honours_Project.Models
{
    //! Section: Contents

    /*
     * - Top-level models
     *      - Contribution_Request
     *      - Contributuon_Result
     * - Sub-level models
     *      - Repo_Total
     *      - Score_Component
    */

    //! END Section: Contents


    //! Section: Top-level models

    /// <summary>
    /// Class that contains options for the contribution request
    /// </summary>
    public class Contribution_Request
    {
        /// <summary>
        /// ID of the author
        /// </summary>
        public string Author_Id { get; set; }

        /// <summary>
        /// Commit, Additions and Deletions totals for the total repo
        /// </summary>
        public Repo_Total Repo { get; set; }

        /// <summary>
        /// Commit, Additions and Deletions totals for the user
        /// </summary>
        public Repo_Total User { get; set; }

        /// <summary>
        /// The total number of collaborators in a repo
        /// </summary>
        public int Author_Total { get; set; }

        /// <summary>
        /// Start of the range
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// End of the range
        /// </summary>
        public DateTime? End { get; set; }
    }

    /// <summary>
    /// Class that contains the result of a contribution request
    /// </summary>
    public class Contribution_Result
    {
        /// <summary>
        /// The score information for the collaborator
        /// </summary>
        public Score_Component Score { get; set; }

        /// <summary>
        /// The ID of the author the contribution was calculated for
        /// </summary>
        public string Author_Id { get; set; }
    }

    //! END Section: Top-level models


    //! Section: Sub-level models

    /// <summary>
    /// Class that contains commit, addition and deletion totals
    /// </summary>
    public class Repo_Total
    {
        /// <summary>
        /// Total number of commits
        /// </summary>
        public int Commit_Total { get; set; }

        /// <summary>
        /// Total number of additions
        /// </summary>
        public int Addition_Total { get; set; }

        /// <summary>
        /// Total number of deletions
        /// </summary>
        public int Deletion_Total { get; set; }
    }

    /// <summary>
    /// Class that contains information about the calculated score
    /// </summary>
    public class Score_Component
    {
        /// <summary>
        /// Calculated commit score
        /// </summary>
        public decimal Commit_Score { get; set; }

        /// <summary>
        /// Calculated addition score
        /// </summary>
        public decimal Addition_Score { get; set; }

        /// <summary>
        /// Calculated deletion score
        /// </summary>
        public decimal Deletion_Score { get; set; }

        /// <summary>
        /// Calculation contribution score
        /// </summary>
        public decimal Contribution_Score { get; set; }
    }

    //! END Section: Sub-level models
}
