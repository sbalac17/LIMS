using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace LIMS.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Lab> Labs { get; set; }
        public DbSet<LabMember> LabMembers { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Reagent> Reagents { get; set; }
        public DbSet<UsedReagent> UsedReagents { get; set; }
        public DbSet<Sample> Samples { get; set; }
        public DbSet<LabSample> LabSamples { get; set; }
        public DbSet<LabSampleComment> LabSampleComments { get; set; }
        public DbSet<Test> Tests { get; set; }
    }

    public class ApplicationUser : IdentityUser, IEquatable<ApplicationUser>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType = DefaultAuthenticationTypes.ApplicationCookie)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public bool Equals(ApplicationUser other)
        {
            if (ReferenceEquals(other, null)) return false;
            return string.Equals(UserName, other.UserName, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ApplicationUser)obj);
        }

        public override int GetHashCode()
        {
            return UserName.GetHashCode();
        }

        public static bool operator ==(ApplicationUser left, ApplicationUser right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ApplicationUser left, ApplicationUser right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// The labs this user is participating in.
        /// </summary>
        public virtual ICollection<LabMember> LabMemberships { get; set; }
    }

    [ModelBinder(typeof(LabModelBinder))]
    public class Lab
    {
        public long LabId { get; set; }

        [Display(Name = "College")]
        [Required]
        public string CollegeName { get; set; }

        [Display(Name = "Course Code")]
        [Required]
        public string CourseCode { get; set; }

        [Display(Name = "Week #")]
        [Required]
        public int WeekNumber { get; set; }

        [Display(Name = "Test Code")]
        public string TestId { get; set; }

        [JsonIgnore]
        public Test Test { get; set; }

        [Display(Name = "Location")]
        [Required]
        public string Location { get; set; }
        
        /// <summary>
        /// The users participating in this lab.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<LabMember> Members { get; set; }

        public bool UserIsLabManager(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return UserIsLabManager(user.Id);
        }
        
        public bool UserIsLabManager(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            return Members.Any(lm => lm.IsLabManager && lm.UserId == userId);
        }

        public bool UserIsMember(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return UserIsMember(user.Id);
        }
        
        public bool UserIsMember(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            return Members.Any(lm => lm.UserId == userId);
        }
    }

    public class LabMember : IUserWithLabManager
    {
        [Required]
        [Key, Column(Order = 0)]
        [JsonIgnore]
        public long LabId { get; set; }
        
        [JsonIgnore]
        public Lab Lab { get; set; }

        [Required]
        [Key, Column(Order = 1)]
        public string UserId { get; set; }

        [Display(Name = "Username")]
        public string UserName => User.UserName;

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        public bool IsLabManager { get; set; }
        
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
        [JsonIgnore]
        public DateTimeOffset? LastOpened { get; set; }

        [JsonProperty(nameof(LastOpened))]
        [NotMapped]
        public DateWithTime? LastOpenedJson
        {
            get => LastOpened;
            set => LastOpened = value;
        }
    }

    public class LogEntry
    {
        [JsonIgnore]
        public long LogEntryId { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }
        
        [Display(Name = "Date")]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
        [JsonIgnore]
        public DateTimeOffset Date { get; set; }

        [JsonProperty(nameof(Date))]
        [NotMapped]
        public DateWithTime DateJson
        {
            get => Date;
            set => Date = value;
        }
    }
    
    [ModelBinder(typeof(ReagentModelBinder))]
    public class Reagent
    {
        public long ReagentId { get; set; }
        
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }
        
        [Display(Name = "Quantity")]
        [Required]
        public int Quantity { get; set; }
        
        [Display(Name = "Added")]
        [Required]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = Date.Placeholder, ApplyFormatInEditMode = true)]
        [JsonIgnore]
        public DateTimeOffset AddedDate { get; set; }

        [JsonProperty(nameof(AddedDate))]
        [NotMapped]
        public DateWithTime AddedDateJson
        {
            get => AddedDate;
            set => AddedDate = value;
        }
        
        [Display(Name = "Expires")]
        [Required]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = Date.Placeholder, ApplyFormatInEditMode = true)]
        [JsonIgnore]
        public DateTimeOffset ExpiryDate { get; set; }

        [JsonProperty(nameof(ExpiryDate))]
        [NotMapped]
        public DateWithTime ExpiryDateJson
        {
            get => ExpiryDate;
            set => ExpiryDate = value;
        }

        [Display(Name = "Manufacturer Code")]
        [Required]
        public string ManufacturerCode { get; set; }
    }

    public class UsedReagent
    {
        public long UsedReagentId { get; set; }

        [Required]
        public long LabId { get; set; }
        
        [Required]
        public long ReagentId { get; set; }
        
        [JsonIgnore]
        public Lab Lab { get; set; }

        [JsonIgnore]
        public Reagent Reagent { get; set; }

        [Required]
        public int Quantity { get; set; }
        
        [Display(Name = "Use Date")]
        [Required]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
        [JsonIgnore]
        public DateTimeOffset UsedDate { get; set; }

        [JsonProperty(nameof(UsedDate))]
        [NotMapped]
        public DateWithTime UsedDateJson
        {
            get => UsedDate;
            set => UsedDate = value;
        }
    }

    [ModelBinder(typeof(SampleModelBinder))]
    public class Sample
    {
        public long SampleId { get; set; }
        
        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Taken")]
        [Required]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
        [JsonIgnore]
        public DateTimeOffset AddedDate { get; set; }

        [JsonProperty(nameof(AddedDate))]
        [NotMapped]
        public DateWithTime AddedDateJson
        {
            get => AddedDate;
            set => AddedDate = value;
        }
        
        // TestId isn't required to fix SQL error ("may cause cycles or multiple cascade paths")
        [Display(Name = "Test Code")]
        public string TestId { get; set; }

        [Display(Name = "Test")]
        [JsonIgnore]
        public Test Test { get; set; }
    }

    public enum LabSampleStatus
    {
        InProgress, Approved, Rejected
    }

    public class LabSample
    {
        [Required]
        [Key, Column(Order = 0)]
        public long LabId { get; set; }

        [JsonIgnore]
        public Lab Lab { get; set; }

        [Required]
        [Key, Column(Order = 1)]
        [Index(IsUnique = true)]
        public long SampleId { get; set; }

        [JsonIgnore]
        public Sample Sample { get; set; }
        
        [Display(Name = "Assigned At")]
        [Required]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
        [JsonIgnore]
        public DateTimeOffset AssignedDate { get; set; }

        [JsonProperty(nameof(AssignedDate))]
        [NotMapped]
        public DateWithTime AssignedDateJson
        {
            get => AssignedDate;
            set => AssignedDate = value;
        }

        [Display(Name = "Notes")]
        [StringLength(10000)]
        public string Notes { get; set; }

        [Required]
        public LabSampleStatus Status { get; set; }
    }

    public class LabSampleComment
    {
        [JsonIgnore]
        public long LabSampleCommentId { get; set; }

        [Required]
        [JsonIgnore]
        public long LabId { get; set; }

        [Required]
        [JsonIgnore]
        public long SampleId { get; set; }

        [Required]
        [JsonIgnore]
        public LabSample LabSample { get; set; }

        [Required]
        [JsonIgnore]
        public string UserId { get; set; }

        [Required]
        [JsonIgnore]
        public ApplicationUser User { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
        [JsonIgnore]
        public DateTimeOffset Date {get; set; }

        [JsonProperty(nameof(Date))]
        [NotMapped]
        public DateWithTime DateJson
        {
            get => Date;
            set => Date = value;
        }

        public string Message { get; set; }

        public LabSampleStatus? NewStatus {get; set; }
    }

    [ModelBinder(typeof(TestModelBinder))]
    public class Test
    {
        [Display(Name = "Test Code")]
        public string TestId { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }
    }
}
