using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honours_Project.Services
{
    public interface IStatisticsService
    {
    }

    public class StatisticsService : IStatisticsService
    {
        private readonly decimal COMMIT_PERCENTAGE_FACTOR = 50M;
        private readonly decimal ADDITION_PERCENTAGE_FACTOR = 25M;
        private readonly decimal DELETION_PERCENTAGE_FACTOR = 25M;

        //! SECTION: Top-level Calculation


        //! END SECTION: Top-level Calculations


        //! SECTION: Sub-level Calculations

        /// <summary>
        /// Method to calculate an individual's commit score
        /// </summary>
        /// <param name="userCommits"></param>
        /// <param name="repoCommits"></param>
        /// <param name="authorTotal"></param>
        /// <returns></returns>
        public decimal Calculate_Commit_Score(int userCommits, int repoCommits, int authorTotal)
        {
            if (repoCommits == 0)
            {
                return (COMMIT_PERCENTAGE_FACTOR / (decimal)authorTotal);
            }

            return ((decimal)userCommits / (decimal)repoCommits) * COMMIT_PERCENTAGE_FACTOR;
        }

        /// <summary>
        /// Method to calculate an individual's addition score
        /// </summary>
        /// <param name="userAdditions"></param>
        /// <param name="repoAdditions"></param>
        /// <param name="authorTotal"></param>
        /// <returns></returns>
        public decimal Calculate_Addition_Score(int userAdditions, int repoAdditions, int authorTotal)
        {
            // If there are no additions in the repo, provide an equal distribution to all collaborators
            if (repoAdditions == 0)
            {
                return (ADDITION_PERCENTAGE_FACTOR / (decimal)authorTotal);
            }

            return ((decimal)userAdditions / (decimal)repoAdditions) * ADDITION_PERCENTAGE_FACTOR;
        }

        /// <summary>
        /// Method to calculate an individual's deletion score
        /// </summary>
        /// <param name="userDeletions"></param>
        /// <param name="repoDeletions"></param>
        /// <param name="authorTotal"></param>
        /// <returns></returns>
        public decimal Calculate_Deletion_Score(int userDeletions, int repoDeletions, int authorTotal)
        {
            if (userDeletions == 0)
            {
                return (DELETION_PERCENTAGE_FACTOR / (decimal)authorTotal);
            }

            return ((decimal)userDeletions / (decimal)repoDeletions) * DELETION_PERCENTAGE_FACTOR;
        }

        //! END SECTION: Sub-level Calculations
    }
}
