using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoviIDApiCore.V1.Controllers
{
    [ApiController]
    [Route("api/sms")]
    public class SmsController : Controller
    {
        public SmsController()
        {
        }

        [HttpPost("balance_job")]
        public async Task<IActionResult> BalanceJob()
        {

        }
    }
}