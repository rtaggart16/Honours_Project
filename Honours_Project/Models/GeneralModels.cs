/*
    Name: Ross Taggart
    ID: S1828840
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honours_Project.Models
{
    //! Section: Contents

    /*
     * - General
     *      - Status
     * - Enums
     *      - GitHub_Model_Types
    */

    //! END Section: Contents

    //! General

    /// <summary>
    /// Provides a shortened overview of the web response of a request
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Textual description of the response
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Response code of the request e.g. 200, 404, 500 etc...
        /// </summary>
        public int Status_Code { get; set; }
    }

    //! END Section: General

    //! Section: Enums

    /// <summary>
    /// Enum that tracks the various models of the application
    /// </summary>
    public enum GitHub_Model_Types
    {
        Repo_List_Result = 0,
        Repo_Stat_Result = 1,
        Repo_Commit_Result = 2,
        GraphQL_Repository_Result = 3
    }

    //! END Section: Enums
}
