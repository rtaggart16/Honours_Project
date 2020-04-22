/*
    Name: Ross Taggart
    ID: S1828840
*/

using Honours_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honours_Project.Services
{
    /// <summary>
    /// Interface that contains method definitions for calculating contribution scores data
    /// </summary>
    public interface IStatisticsService
    {
        Contribution_Result CalculateBasicCommitContributionScore(Contribution_Request request);

        decimal Calculate_Addition_Score(int userAdditions, int repoAdditions, int authorTotal);

        decimal Calculate_Deletion_Score(int userDeletions, int repoDeletions, int authorTotal);

        decimal Calculate_Commit_Score(int userCommits, int repoCommits, int authorTotal);
    }

    /// <summary>
    /// Class that contains method bodies for fetching REST data
    /// </summary>
    public class StatisticsService : IStatisticsService
    {
        //! Section: Globals

        /// <summary>
        /// Decimal that contains the weighting associated with the commit score
        /// </summary>
        private readonly decimal COMMIT_PERCENTAGE_FACTOR = 50M;

        /// <summary>
        /// Decimal that contains the weighting associated with the addition score
        /// </summary>
        private readonly decimal ADDITION_PERCENTAGE_FACTOR = 25M;

        /// <summary>
        /// Decimal that contains the weighting associated with the deletion score
        /// </summary>
        private readonly decimal DELETION_PERCENTAGE_FACTOR = 25M;

        //! END Section: Globals

        //! SECTION: Methods

        /// <summary>
        /// Method for fetching the three scores associated with user's contribution and assigning a contribution score
        /// </summary>
        /// <param name="request">Data required for contribution assignment</param>
        /// <returns>A collaborator's contribution</returns>
        public Contribution_Result CalculateBasicCommitContributionScore(Contribution_Request request)
        {
            // Initialise the values
            Contribution_Result result = new Contribution_Result
            {
                Author_Id = request.Author_Id,
                Score = new Score_Component()
                {
                    Commit_Score = Calculate_Commit_Score(request.User.Commit_Total, request.Repo.Commit_Total, request.Author_Total),
                    Addition_Score = Calculate_Addition_Score(request.User.Addition_Total, request.Repo.Addition_Total, request.Author_Total),
                    Deletion_Score = Calculate_Deletion_Score(request.User.Deletion_Total, request.Repo.Deletion_Total, request.Author_Total)
                }
            };

            // Set the value of the contribution score to the sum of the three sub-scores
            result.Score.Contribution_Score = (result.Score.Commit_Score + result.Score.Addition_Score + result.Score.Deletion_Score);

            // Return the scores
            return result;
        }

        /// <summary>
        /// Method to calculate an individual's commit score
        /// </summary>
        /// <param name="userCommits">The total number of commits performed by the collaborator</param>
        /// <param name="repoCommits">The total number of commits in the repository</param>
        /// <param name="authorTotal">The total number of collaborators in the repository</param>
        /// <returns>Collaborator's commit score</returns>
        public decimal Calculate_Commit_Score(int userCommits, int repoCommits, int authorTotal)
        {
            // If repository or user commits are less than 0, return 0
            if(repoCommits < 0 || userCommits < 0)
            {
                return 0;
            }
            else
            {
                // If there are no commits in the repository, divide the commit weight equally in respect to the number of collaborators
                if (repoCommits == 0)
                {
                    return (COMMIT_PERCENTAGE_FACTOR / (decimal)authorTotal);
                }

                // Calculate the commit score of the collaborator by calculating the percentage of commits that are theirs and multiplying by commit weighting
                return ((decimal)userCommits / (decimal)repoCommits) * COMMIT_PERCENTAGE_FACTOR;
            }
        }

        /// <summary>
        /// Method to calculate an individual's addition score
        /// </summary>
        /// <param name="userAdditions">The total number of additions performed by the collaborator</param>
        /// <param name="repoAdditions">The total number of additions in the repository</param>
        /// <param name="authorTotal">The total number of collaborators in the repository</param>
        /// <returns>Collaborator's addition score</returns>
        public decimal Calculate_Addition_Score(int userAdditions, int repoAdditions, int authorTotal)
        {
            // If repository or user additions are less than 0, return 0
            if (repoAdditions < 0 || userAdditions < 0)
            {
                return 0;
            }
            else
            {
                // If there are no additions in the repository, divide the addition weight equally in respect to the number of collaborators
                if (repoAdditions == 0)
                {
                    return (ADDITION_PERCENTAGE_FACTOR / (decimal)authorTotal);
                }

                // Calculate the addition score of the collaborator by calculating the percentage of additions that are theirs and multiplying by addition weighting
                return ((decimal)userAdditions / (decimal)repoAdditions) * ADDITION_PERCENTAGE_FACTOR;
            }
        }

        /// <summary>
        /// Method to calculate an individual's deletion score
        /// </summary>
        /// <param name="userDeletions">The total number of deletions performed by the collaborator</param>
        /// <param name="repoDeletions">The total number of deletions in the repository</param>
        /// <param name="authorTotal">The total number of collaborators in the repository</param>
        /// <returns>Collaborator's deletion score</returns>
        public decimal Calculate_Deletion_Score(int userDeletions, int repoDeletions, int authorTotal)
        {
            // If repository or user additions are less than 0, return 0
            if (repoDeletions < 0 || userDeletions < 0)
            {
                return 0;
            }
            else
            {
                // If there are no deletions in the repository, divide the deletion weight equally in respect to the number of collaborators
                if (repoDeletions == 0)
                {
                    return (DELETION_PERCENTAGE_FACTOR / (decimal)authorTotal);
                }

                // Calculate the deletion score of the collaborator by calculating the percentage of deletions that are theirs and multiplying by deletion weighting
                return ((decimal)userDeletions / (decimal)repoDeletions) * DELETION_PERCENTAGE_FACTOR;
            }
        }

        //! END SECTION: Methods
    }
}
