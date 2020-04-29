using System.Collections.Generic;
using static CoviIDApiCore.V1.Constants.DefinitionConstants;

namespace CoviIDApiCore.Models.AppSettings
{
    /// <summary>
    /// Stores the streetcred ID's related to the specified Tenant.
    /// </summary>
    public class StreetcredDefinitions
    {
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public Dictionary<Schema, string> DefinitionIds { get; set; }
        public Dictionary<Schema, string> SchemaIds { get; set; }
    }
}
