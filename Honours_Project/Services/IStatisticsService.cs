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
        private readonly decimal ADDITION_PERCENTAGE_FACTOR = 50M;

        //! SECTION: Top-level Calculation


        //! END SECTION: Top-level Calculations


        //! SECTION: Sub-level Calculations
        
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

        //! END SECTION: Sub-level Calculations
    }
}
