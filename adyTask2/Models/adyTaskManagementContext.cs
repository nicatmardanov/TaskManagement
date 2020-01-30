using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace adyTask2.Models
{
    public partial class adyTaskManagementContext : DbContext
    {
        public adyTaskManagementContext()
        {
        }

        public adyTaskManagementContext(DbContextOptions<adyTaskManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Direct> Direct { get; set; }
        public virtual DbSet<MDepartment> MDepartment { get; set; }
        public virtual DbSet<MLog> MLog { get; set; }
        public virtual DbSet<Mail> Mail { get; set; }
        public virtual DbSet<Meeting> Meeting { get; set; }
        public virtual DbSet<MeetingLine> MeetingLine { get; set; }
        public virtual DbSet<MeetingType> MeetingType { get; set; }
        public virtual DbSet<MlDetails> MlDetails { get; set; }
        public virtual DbSet<MlType> MlType { get; set; }
        public virtual DbSet<Operations> Operations { get; set; }
        public virtual DbSet<OtherParticipiants> OtherParticipiants { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<PermissionPages> PermissionPages { get; set; }
        public virtual DbSet<Place> Place { get; set; }
        public virtual DbSet<Reports> Reports { get; set; }
        public virtual DbSet<Revision> Revision { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Tags> Tags { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserOld> UserOld { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.


                //optionsBuilder.UseSqlServer("Server=DESKTOP-UTUBGGC\\SQLEXPRESS;Database=adyTaskManagement;Trusted_Connection=True;");
                optionsBuilder.UseSqlServer(@"Data Source=192.168.5.17;Initial Catalog=adyTaskManagement;User ID=meeting;Password=12345@ady;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");


                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.LazyLoadOnDisposedContextWarning));

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.ToTable("attachment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Path).HasColumnName("path");

                entity.Property(e => e.RefId).HasColumnName("ref_id");

                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("country");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(70);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Direct>(entity =>
            {
                entity.ToTable("direct");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("create_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.FinishDate)
                    .HasColumnName("finish_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.FromUserId).HasColumnName("fromUser_id");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.MlId).HasColumnName("ml_id");

                entity.Property(e => e.ToUserId).HasColumnName("toUser_id");

                entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.DirectFromUser)
                    .HasPrincipalKey(p => p.PersonId)
                    .HasForeignKey(d => d.FromUserId)
                    .HasConstraintName("FK_direct_userr");

                entity.HasOne(d => d.Ml)
                    .WithMany(p => p.Direct)
                    .HasForeignKey(d => d.MlId)
                    .HasConstraintName("FK_direct_meeting_line");

                entity.HasOne(d => d.ToUser)
                    .WithMany(p => p.DirectToUser)
                    .HasPrincipalKey(p => p.PersonId)
                    .HasForeignKey(d => d.ToUserId)
                    .HasConstraintName("FK_direct_user11");
            });

            modelBuilder.Entity<MDepartment>(entity =>
            {
                entity.ToTable("m_department");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.RefId).HasColumnName("ref_id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.MDepartment)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_meeting_department_department");
            });

            modelBuilder.Entity<MLog>(entity =>
            {
                entity.ToTable("m_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AdditionalInformation)
                    .HasColumnName("additional_information")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate)
                    .HasColumnName("create_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IpAdress).HasColumnName("ipAdress");

                entity.Property(e => e.OperationId).HasColumnName("operation_id");

                entity.Property(e => e.ReadDate)
                    .HasColumnName("read_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.RefId).HasColumnName("ref_id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.MLog)
                    .HasForeignKey(d => d.OperationId)
                    .HasConstraintName("FK_m_log_operations");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MLog)
                    .HasPrincipalKey(p => p.PersonId)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_m_logg_userr");
            });

            modelBuilder.Entity<Mail>(entity =>
            {
                entity.ToTable("mail");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Cc)
                    .HasColumnName("cc")
                    .IsUnicode(false);

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("create_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ErrorText).HasColumnName("error_text");

                entity.Property(e => e.Message).HasColumnName("message");

                entity.Property(e => e.RefId).HasColumnName("ref_id");

                entity.Property(e => e.SendDate)
                    .HasColumnName("send_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .IsUnicode(false);

                entity.Property(e => e.ToUser)
                    .HasColumnName("toUser")
                    .IsUnicode(false);

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.Ref)
                    .WithMany(p => p.Mail)
                    .HasForeignKey(d => d.RefId)
                    .HasConstraintName("FK_mail_meeting");
            });

            modelBuilder.Entity<Meeting>(entity =>
            {
                entity.ToTable("meeting");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("create_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.CreatorId).HasColumnName("creator_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.FinishDate)
                    .HasColumnName("finish_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.FollowerUser)
                    .HasColumnName("followerUser")
                    .IsUnicode(false);

                entity.Property(e => e.InformedUser).HasColumnName("informedUser");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsPublished).HasColumnName("isPublished");

                entity.Property(e => e.MeetingDate)
                    .HasColumnName("meeting_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.MeetingType).HasColumnName("meeting_type");

                entity.Property(e => e.OtherParticipiants)
                    .HasColumnName("other_participiants")
                    .IsUnicode(false);

                entity.Property(e => e.OwnerUser)
                    .HasColumnName("ownerUser")
                    .IsUnicode(false);

                entity.Property(e => e.Participiants)
                    .HasColumnName("participiants")
                    .IsUnicode(false);

                entity.Property(e => e.Place).HasColumnName("place");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.HasOne(d => d.MeetingTypeNavigation)
                    .WithMany(p => p.Meeting)
                    .HasForeignKey(d => d.MeetingType)
                    .HasConstraintName("FK_meeting_meeting_type");

                entity.HasOne(d => d.PlaceNavigation)
                    .WithMany(p => p.Meeting)
                    .HasForeignKey(d => d.Place)
                    .HasConstraintName("FK_meeting_place");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Meeting)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_meeting_status");
            });

            modelBuilder.Entity<MeetingLine>(entity =>
            {
                entity.ToTable("meeting_line");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("create_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.CreatorId).HasColumnName("creator_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.FinishTime)
                    .HasColumnName("finish_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.FollowerEmail)
                    .HasColumnName("follower_email")
                    .IsUnicode(false);

                entity.Property(e => e.IdentifierEmail)
                    .HasColumnName("identifier_email")
                    .IsUnicode(false);

                entity.Property(e => e.InformedUserEmail)
                    .HasColumnName("informedUser_email")
                    .IsUnicode(false);

                entity.Property(e => e.IsDirected).HasColumnName("isDirected");

                entity.Property(e => e.IsPublished).HasColumnName("isPublished");

                entity.Property(e => e.IsRevised).HasColumnName("isRevised");

                entity.Property(e => e.MeetingId).HasColumnName("meeting_id");

                entity.Property(e => e.MlType).HasColumnName("ml_type");

                entity.Property(e => e.ReadDate)
                    .HasColumnName("read_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ResponsibleEmail)
                    .HasColumnName("responsible_email")
                    .IsUnicode(false);

                entity.Property(e => e.StartTime)
                    .HasColumnName("start_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.MeetingLine)
                    .HasPrincipalKey(p => p.PersonId)
                    .HasForeignKey(d => d.CreatorId)
                    .HasConstraintName("FK_meeting_line_userr");

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.MeetingLine)
                    .HasForeignKey(d => d.MeetingId)
                    .HasConstraintName("FK_meeting_line_meeting");

                entity.HasOne(d => d.MlTypeNavigation)
                    .WithMany(p => p.MeetingLine)
                    .HasForeignKey(d => d.MlType)
                    .HasConstraintName("FK_meeting_line_ml_type");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MeetingLine)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_meeting_line_status");
            });

            modelBuilder.Entity<MeetingType>(entity =>
            {
                entity.ToTable("meeting_type");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<MlDetails>(entity =>
            {
                entity.ToTable("ml_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Completion)
                    .HasColumnName("completion")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate)
                    .HasColumnName("create_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.MlId).HasColumnName("ml_id");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Ml)
                    .WithMany(p => p.MlDetails)
                    .HasForeignKey(d => d.MlId)
                    .HasConstraintName("FK_ml_details_meeting_line1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MlDetails)
                    .HasPrincipalKey(p => p.PersonId)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ml_details_userr");
            });

            modelBuilder.Entity<MlType>(entity =>
            {
                entity.ToTable("ml_type");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Operations>(entity =>
            {
                entity.ToTable("operations");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Operation)
                    .HasColumnName("operation")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<OtherParticipiants>(entity =>
            {
                entity.ToTable("other_participiants");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Company)
                    .HasColumnName("company")
                    .HasMaxLength(100);

                entity.Property(e => e.CountryId).HasColumnName("country_id");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Position)
                    .HasColumnName("position")
                    .HasMaxLength(200);

                entity.Property(e => e.Profession)
                    .HasColumnName("profession")
                    .HasMaxLength(100);

                entity.Property(e => e.Surname)
                    .HasColumnName("surname")
                    .HasMaxLength(100);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.OtherParticipiants)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_other_participiants_country");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permission");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Department)
                    .HasColumnName("department")
                    .IsUnicode(false);

                entity.Property(e => e.Mtype)
                    .HasColumnName("mtype")
                    .IsUnicode(false);

                entity.Property(e => e.Page)
                    .HasColumnName("page")
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Permission)
                    .HasPrincipalKey(p => p.PersonId)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_permission_userr");
            });

            modelBuilder.Entity<PermissionPages>(entity =>
            {
                entity.ToTable("permission_pages");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PageName)
                    .HasColumnName("page_name")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Place>(entity =>
            {
                entity.ToTable("place");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Reports>(entity =>
            {
                entity.ToTable("reports");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("create_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ReportName)
                    .HasColumnName("report_name")
                    .HasMaxLength(500);

                entity.Property(e => e.ReportString)
                    .HasColumnName("report_string")
                    .HasMaxLength(500);

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reports)
                    .HasPrincipalKey(p => p.PersonId)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_reports_user");
            });

            modelBuilder.Entity<Revision>(entity =>
            {
                entity.ToTable("revision");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("create_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.MlId).HasColumnName("ml_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Ml)
                    .WithMany(p => p.Revision)
                    .HasForeignKey(d => d.MlId)
                    .HasConstraintName("FK_Revision_meeting_line");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Revision)
                    .HasPrincipalKey(p => p.PersonId)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Revision_userr");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasColumnName("role_name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("status");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasColumnName("status_name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Tags>(entity =>
            {
                entity.ToTable("tags");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.RefId).HasColumnName("ref_id");

                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.PersonId)
                    .HasName("IX_user")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Active).HasColumnName("ACTIVE");

                entity.Property(e => e.CurrentEmployeeFlag)
                    .IsRequired()
                    .HasColumnName("CURRENT_EMPLOYEE_FLAG")
                    .HasMaxLength(1);

                entity.Property(e => e.EmailAddress).HasColumnName("EMAIL_ADDRESS");

                entity.Property(e => e.FirstName).HasColumnName("FIRST_NAME");

                entity.Property(e => e.FullName).HasColumnName("FULL_NAME");

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasColumnName("GENDER")
                    .HasMaxLength(1);

                entity.Property(e => e.IsActiveDirectory).HasColumnName("IS_ACTIVE_DIRECTORY");

                entity.Property(e => e.JobCategoryCode).HasColumnName("JOB_CATEGORY_CODE");

                entity.Property(e => e.JobCategoryName).HasColumnName("JOB_CATEGORY_NAME");

                entity.Property(e => e.JobId).HasColumnName("JOB_ID");

                entity.Property(e => e.JobName).HasColumnName("JOB_NAME");

                entity.Property(e => e.JobSubcategoryCode).HasColumnName("JOB_SUBCATEGORY_CODE");

                entity.Property(e => e.JobSubcategoryName).HasColumnName("JOB_SUBCATEGORY_NAME");

                entity.Property(e => e.LastName).HasColumnName("LAST_NAME");

                entity.Property(e => e.MobilePhoneNumber).HasColumnName("MOBILE_PHONE_NUMBER");

                entity.Property(e => e.NationalIdentifier).HasColumnName("NATIONAL_IDENTIFIER");

                entity.Property(e => e.OrganizationFullName).HasColumnName("ORGANIZATION_FULL_NAME");

                entity.Property(e => e.OrganizationId).HasColumnName("ORGANIZATION_ID");

                entity.Property(e => e.OrganizationShortName).HasColumnName("ORGANIZATION_SHORT_NAME");

                entity.Property(e => e.Password)
                    .HasColumnName("PASSWORD")
                    .HasMaxLength(100);

                entity.Property(e => e.Patronymic).HasColumnName("PATRONYMIC");

                entity.Property(e => e.PersonId).HasColumnName("PERSON_ID");

                entity.Property(e => e.PositionId).HasColumnName("POSITION_ID");
            });

            modelBuilder.Entity<UserOld>(entity =>
            {
                entity.ToTable("user_old");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Position).HasColumnName("position");

                entity.Property(e => e.Surname)
                    .HasColumnName("surname")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_role_role1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRole)
                    .HasPrincipalKey(p => p.PersonId)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_role_userr");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
