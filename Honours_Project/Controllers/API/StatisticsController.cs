using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honours_Project.Models;
using Honours_Project.Services;
using Microsoft.AspNetCore.Mvc;

namespace Honours_Project.Controllers
{
    [Route("api/statistics")]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpPost]
        [Route("get/contribution/score")]
        public Contribution_Result Get_Contribution_Score([FromBody] Contribution_Request request)
        {
            return _statisticsService.CalculateBasicCommitContributionScore(request);
        }

        [HttpPost]
        [Route("get/addition/score")]
        public decimal Get_Addition_Score([FromBody] Contribution_Request request)
        {
            return _statisticsService.Calculate_Addition_Score(request.User.Addition_Total, request.Repo.Addition_Total, request.Author_Total);
        }

        [HttpPost]
        [Route("get/deletion/score")]
        public decimal Get_Deletion_Score([FromBody] Contribution_Request request)
        {
            return _statisticsService.Calculate_Deletion_Score(request.User.Deletion_Total, request.Repo.Deletion_Total, request.Author_Total);
        }

        [HttpPost]
        [Route("get/commit/score")]
        public decimal Get_Commit_Score([FromBody] Contribution_Request request)
        {
            return _statisticsService.Calculate_Commit_Score(request.User.Commit_Total, request.Repo.Commit_Total, request.Author_Total);
        }
    }
}