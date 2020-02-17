using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public class Contribution_Request
    {
        public string Author_Id { get; set; }

        public Repo_Total Repo { get; set; }

        public Repo_Total User { get; set; }

        public int Author_Total { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }
    }

    public class Contribution_Result
    {
        public Score_Component Score { get; set; }

        public string Author_Id { get; set; }
    }

    //! END Section: Top-level models


    //! Section: Sub-level models

    public class Repo_Total
    {
        public int Commit_Total { get; set; }

        public int Addition_Total { get; set; }

        public int Deletion_Total { get; set; }
    }

    public class Score_Component
    {
        public decimal Commit_Score { get; set; }

        public decimal Addition_Score { get; set; }

        public decimal Deletion_Score { get; set; }

        public decimal Contribution_Score { get; set; }
    }

    //! END Section: Sub-level models
}
