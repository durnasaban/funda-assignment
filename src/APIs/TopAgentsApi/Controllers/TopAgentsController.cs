using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

namespace TopAgentsApi.Controllers
{
    using Repositories;
    using Responses;
    using Entities;

    [ApiController]
    [Route("/api/top-agents")]
    public class TopAgentsController : ControllerBase
    {
        private readonly ITopAgentsRepository _repository;

        public TopAgentsController(ITopAgentsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{key}")]
        [ProducesResponseType(typeof(IEnumerable<TopAgentDetailResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<TopAgentDetailResponse>>> GetTopAgents(string key)
        {
            var topAgents = await _repository.GetTopAgents(key);
            return Ok(ToTopAgentDetailResponse(topAgents));
        }

        private IEnumerable<TopAgentDetailResponse> ToTopAgentDetailResponse(IEnumerable<TopAgentDetail> topAgentDetails) =>
            topAgentDetails
                .OrderByDescending(ta => ta.ObjectCount)
                .ThenBy(ta => ta.AgentName)
                .Select(ta => new TopAgentDetailResponse
                {
                    AgentName = ta.AgentName,
                    ObjectCount = ta.ObjectCount
                });
    }
}
