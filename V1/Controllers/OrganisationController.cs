using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Organisation;
using CoviIDApiCore.V1.Interfaces.Services;
using Hangfire;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoviIDApiCore.V1.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/")]
    [ApiController]
    public class OrganisationController : Controller
    {
        private readonly IOrganisationService _organisationService;

        public OrganisationController(IOrganisationService organisationService)
        {
            _organisationService = organisationService;
        }

        [HttpPost("organisations")]
        public IActionResult CreateOrganisation([FromBody] CreateOrganisationRequest payload)
        {
            BackgroundJob.Enqueue(() => _organisationService.CreateAsync(payload));

            return new OkResult();
        }

        [HttpPut("organisation/subtract/{id}")]
        public async Task<IActionResult> Subtract(string id)
        {
            var payload = new UpdateCountRequest()
            {
                Latitude = 0, Longitude = 0,
            };

            return StatusCode(StatusCodes.Status200OK,
                await _organisationService.UpdateCountAsync(id, payload, ScanType.CheckOut));
        }

        [HttpGet("organisation/{id}")]
        public async Task<IActionResult> GetOrganisationOld(string id)
        {
            var resp = await _organisationService.GetAsync(id);

            return StatusCode(resp.Meta.Code, resp);
        }

        [HttpGet("organisations/{id}")]
        public async Task<IActionResult> GetOrganisation(string id)
        {
            var resp = await _organisationService.GetAsync(id);

            return StatusCode(resp.Meta.Code, resp);
        }

        [HttpPost("organisations/{id}/check_in")]
        public async Task<IActionResult> CheckIn(string id, [FromBody] UpdateCountRequest payload)
        {
            return StatusCode(StatusCodes.Status200OK,
                await _organisationService.UpdateCountAsync(id, payload, ScanType.CheckIn));
        }

        [HttpPost("organisations/{id}/check_out")]
        public async Task<IActionResult> CheckOut(string id, [FromBody] UpdateCountRequest payload)
        {
            return StatusCode(StatusCodes.Status200OK,
                await _organisationService.UpdateCountAsync(id, payload, ScanType.CheckOut));
        }
        
        [HttpPost("organisations/{id}/denied")]
        public async Task<IActionResult> AccessDenied(string id, [FromBody] UpdateCountRequest payload)
        {
            return StatusCode(StatusCodes.Status200OK,
                await _organisationService.UpdateCountAsync(id, payload, ScanType.Denied));
        }

        /// <summary>
        /// Creates a mobile entry for a user and checks them in.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost("organisations/{organisationId}/mobile_check_in")]
        public async Task<IActionResult> MobileCheckIn(string organisationId, [FromBody] MobileUpdateCountRequest payload)
        {
            var response = await _organisationService.MobileCheckIn(organisationId, payload);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost("organisations/{organisationId}/mobile_check_out")]
        public async Task<IActionResult> MobileCheckOut(string organisationId, [FromBody] MobileUpdateCountRequest payload)
        {
            var response = await _organisationService.MobileCheckOut(organisationId, payload);
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}