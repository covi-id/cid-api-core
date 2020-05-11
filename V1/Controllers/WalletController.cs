using System.Net;
using System.Threading.Tasks;
using CoviIDApiCore.V1.DTOs.Credentials;
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

        [HttpPost]
        public async Task<IActionResult> CreateWallet(CreateWalletRequest walletParameters)
        {
            var response = await _walletService.CreateWallet(walletParameters);

            return Ok(new Response(response, HttpStatusCode.OK));
        }

        [HttpPost]
        [Route("{walletId}/status")]
        public async Task<IActionResult> GetWalletStatus(Guid walletId, [FromBody] string key)
        {
            var response = await _walletService.GetWalletStatus(walletId, key);

            return Ok(new Response(response, HttpStatusCode.OK));
        }

        [HttpPost]
        [Route("coviid")]
        public async Task<IActionResult> CreateCoviIdWallet([FromBody] CoviIdWalletParameters coviIdWalletParameters)
        {
            var response = await _walletService.CreateCoviIdWallet(coviIdWalletParameters);

            return Ok(new Response(response, HttpStatusCode.OK));
        }

    }
}