﻿using System.Threading.Tasks;
using CoviIDApiCore.Models.Database;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Organisation;
using CoviIDApiCore.V1.DTOs.System;

namespace CoviIDApiCore.V1.Interfaces.Services
{
    public interface IOrganisationService
    {
        Task CreateAsync(CreateOrganisationRequest payload);
        Task<Response> GetAsync(string id);
        Task<int> UpdateCountAsync(UpdateCountRequest payload, ScanType scanType);
    }
}