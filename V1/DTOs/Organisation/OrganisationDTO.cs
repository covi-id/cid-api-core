using CoviIDApiCore.Models.Database;

namespace CoviIDApiCore.V1.DTOs.Organisation
{
    public class OrganisationDTO
    {
        public string Id { get; set; }
        public int? Balance { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }

        public OrganisationDTO()
        {

        }

        public OrganisationDTO(Models.Database.Organisation organisation, OrganisationAccessLog organisationAccessLog, int total, int balance)
        {
            Id = organisation.Id.ToString();
            Balance = balance;
            Name = organisation.Name;
            Total = total;
        }
    }
}