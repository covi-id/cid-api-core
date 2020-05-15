using System;
using System.Net;
using System.Threading.Tasks;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.System;
using CoviIDApiCore.V1.Interfaces.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoviIDApiCore.V1.Controllers
{
    [ApiController]
    [Route("api/sms")]
    public class SmsController : Controller
    {
        private readonly ISmsService _smsService;

        public SmsController(ISmsService smsService)
        {
            _smsService = smsService;
        }

        [HttpPost("balance_job")]
        public IActionResult BalanceJob()
        {
            return StatusCode(StatusCodes.Status200OK,
                _smsService.CreateBalanceJob());
        }

        [HttpGet("balance")]
        public async Task<IActionResult> CheckBalance()
        {
            return StatusCode(StatusCodes.Status200OK,
                await _smsService.VerifyBalance());
        }
    }
}