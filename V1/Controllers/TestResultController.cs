using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoviIDApiCore.V1.DTOs.System;
using CoviIDApiCore.V1.DTOs.WalletTestResult;
using CoviIDApiCore.V1.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoviIDApiCore.V1.Controllers
{
    [Route("api/test_result")]
    [ApiController]
    public class TestResultController : ControllerBase
    {
        private readonly ITestResultService _testResultService;
        public TestResultController(ITestResultService testResultService)
        {
            _testResultService = testResultService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTestResult([FromBody] TestResultRequest testResultRequest)
        {
            await _testResultService.AddTestResult(testResultRequest);

            return Ok(new Response(true, HttpStatusCode.OK));
        }
    }
}