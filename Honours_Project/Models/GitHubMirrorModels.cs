using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honours_Project.Models
{
    //! Section: Contents

    /*
     * - Top-level models
     *      - Repo_List_Result
     * - Sub-level models
     *      - Simple_Repo_Info
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

    //! END Section: Sub-level models
}
