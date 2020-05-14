using System.Net;
using System.Threading.Tasks;
using CoviIDApiCore.V1.DTOs.System;
using CoviIDApiCore.V1.DTOs.Wallet;
using CoviIDApiCore.V1.Interfaces.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CoviIDApiCore.V1.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/wallets")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        /// <summary>
        /// Creates a new wallet, followed by and OTP request
        /// </summary>
        /// <param name="walletParameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{sessionId}")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequest walletParameters, string sessionId = null)
        {
            var response = await _walletService.CreateWalletAndOtp(walletParameters, sessionId);

            return Ok(new Response(response, HttpStatusCode.OK));
        }

        /// <summary>
        /// Retrieves the wallet and covid19 test status
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{walletId}/status")]
        public async Task<IActionResult> GetWalletStatus(string walletId, [FromBody] WalletStatusRequest payload)
        {
            var response = await _walletService.GetWalletStatus(walletId, payload.Key);

            return Ok(new Response(response, HttpStatusCode.OK));
        }
    }
}