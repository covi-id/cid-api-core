using System.Net;
using System.Threading.Tasks;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Authentication;
using CoviIDApiCore.V1.DTOs.System;
using CoviIDApiCore.V1.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoviIDApiCore.V1.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : Controller
    {
        private readonly IOtpService _otpService;

        public AuthenticationController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("otp")]
        public async Task<IActionResult> ConfirmOtp([FromBody] RequestOtpConfirmation payload)
        {
            return StatusCode(StatusCodes.Status200OK,
                new Response(await _otpService.ConfirmOtpAsync(payload, Request.Headers["Authorization"]),true, HttpStatusCode.OK, Messages.Misc_Success));
        }

        [HttpPost("otp/resend")]
        public async Task<IActionResult> ResendOtp([FromBody] RequestResendOtp payload)
        {
            return StatusCode(StatusCodes.Status200OK,
                new Response(await _otpService.ResendOtpAsync(payload, Request.Headers["Authorization"]),true, HttpStatusCode.OK, Messages.Misc_Success));
        }

        [HttpPost("otp/delete_wallet")]
        public async Task<IActionResult> ConfirmDeleteWallet(OtpDeleteWalletRequest request, [FromHeader(Name ="Authorization")] string authToken)
        {
            await _otpService.ConfirmDeleteWallet(request, authToken);
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}