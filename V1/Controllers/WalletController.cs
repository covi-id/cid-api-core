using System.Net;
using System.Threading.Tasks;
using CoviIDApiCore.V1.DTOs.System;
using CoviIDApiCore.V1.DTOs.Wallet;
using CoviIDApiCore.V1.Interfaces.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Route("{sessionId}")]
        public async Task<IActionResult> CreateWalletWithSession([FromBody] CreateWalletRequest walletParameters, string sessionId)
        {
            var response = await _walletService.CreateWalletAndOtp(walletParameters, sessionId);

            return Ok(new Response(response, HttpStatusCode.OK));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequest walletParameters)
        {
            var response = await _walletService.CreateWalletAndOtp(walletParameters, null);

            return Ok(new Response(response, HttpStatusCode.OK));
        }

        [HttpPost]
        [Route("{walletId}/status")]
        public async Task<IActionResult> GetWalletStatus(string walletId, [FromBody] WalletStatusRequest payload)
        {
            var response = await _walletService.GetWalletStatus(walletId, payload.Key);

            return Ok(new Response(response, HttpStatusCode.OK));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteWalletAndOtpRequest([FromBody] string walletId)
        {
            await _walletService.DeleteWallet(walletId);
            return Ok(new Response(response, HttpStatusCode.OK));
        }
    }
}