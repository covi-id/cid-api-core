using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.DTOs.Organisation;
using CoviIDApiCore.V1.DTOs.System;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IOrganisationService
    {
        Task CreateAsync(CreateOrganisationRequest payload);
        Task<Response> GetAsync(string id);
        Task<Response> UpdateCountAsync(string id, UpdateCountRequest payload, ScanType scanType);
        Task<Response> MobileCheckIn(string organisationId, MobileUpdateCountRequest payload);
        Task<Response> MobileCheckOut(string organisationId, MobileUpdateCountRequest payload);
    }
}