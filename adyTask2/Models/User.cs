using System;
using System.Collections.Generic;

namespace adyTask2.Models
{
    public partial class User
    {
        public User()
        {
            DirectFromUser = new HashSet<Direct>();
            DirectToUser = new HashSet<Direct>();
            MLog = new HashSet<MLog>();
            Meeting = new HashSet<Meeting>();
            MeetingLine = new HashSet<MeetingLine>();
            MlDetails = new HashSet<MlDetails>();
            Permission = new HashSet<Permission>();
            Reports = new HashSet<Reports>();
            Revision = new HashSet<Revision>();
            UserRole = new HashSet<UserRole>();
        }

        public int Id { get; set; }
        public int PersonId { get; set; }
        public string CurrentEmployeeFlag { get; set; }
        public string NationalIdentifier { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Gender { get; set; }
        public int? PositionId { get; set; }
        public int? OrganizationId { get; set; }
        public string OrganizationFullName { get; set; }
        public string OrganizationShortName { get; set; }
        public int? JobId { get; set; }
        public string JobName { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string JobCategoryCode { get; set; }
        public string JobCategoryName { get; set; }
        public string JobSubcategoryCode { get; set; }
        public string JobSubcategoryName { get; set; }
        public string Password { get; set; }
        public byte? Active { get; set; }
        public bool? IsActiveDirectory { get; set; }

        public virtual ICollection<Direct> DirectFromUser { get; set; }
        public virtual ICollection<Direct> DirectToUser { get; set; }
        public virtual ICollection<MLog> MLog { get; set; }
        public virtual ICollection<Meeting> Meeting { get; set; }
        public virtual ICollection<MeetingLine> MeetingLine { get; set; }
        public virtual ICollection<MlDetails> MlDetails { get; set; }
        public virtual ICollection<Permission> Permission { get; set; }
        public virtual ICollection<Reports> Reports { get; set; }
        public virtual ICollection<Revision> Revision { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
