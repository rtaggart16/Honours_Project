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
    }
}