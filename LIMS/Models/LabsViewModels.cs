using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMS.Models
{
    public class LabsSearchViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Query { get; set; }

        public List<LabsSearchResult> Results { get; set; }
    }

    public class LabsSearchResult
    {
        public long Id { get; set; }

        public bool IsMember { get; set; }

        [Display(Name = "College")]
        public string CollegeName { get; set; }

        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Display(Name = "Lab Manager")]
        public string FacultyName { get; set; }

        [Display(Name = "Week #")]
        public int WeekNumber { get; set; }

        [Display(Name = "Test Code")]
        public string TestCode { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }
    }

    public class LabsDetailsViewModel
    {
        public Lab Lab { get; set; }

        public bool IsLabManager { get; set; }
    }

    public class LabsCreateViewModel
    {
        public LabsCreateViewModel()
        {
            CollegeName = "Centennial College";
            Location = "Progress - D3-14";
            WeekNumber = 1;
        }

        [Display(Name = "College")]
        [Required]
        [StringLength(100)]
        public string CollegeName { get; set; }

        [Display(Name = "Course Code")]
        [Required]
        [StringLength(15)]
        public string CourseCode { get; set; }

        [Display(Name = "Week #")]
        [Required]
        [Range(0, 1000)]
        public int WeekNumber { get; set; }

        [Display(Name = "Test Code")]
        [Required]
        [StringLength(20, MinimumLength = 3)]
        [RegularExpression("^[A-Z0-9]*$", ErrorMessage = "Test Code may only contain A-Z and 0-9.")]
        public string TestCode { get; set; }

        [Display(Name = "Location")]
        [Required]
        [StringLength(100)]
        public string Location { get; set; }
    }

    public class LabsMembersViewModel
    {
        public Lab Lab { get; set; }

        public bool IsLabManager { get; set; }

        public List<Result> Members { get; set; }
        
        public class Result : IUserWithLabManager
        {
            public ApplicationUser User { get; set; }

            public bool IsLabManager { get; set; }

            [Display(Name = "Last Active")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm:ss tt}", ApplyFormatInEditMode = true)]
            public DateTimeOffset? LastActive { get; set; }
        }
    }

    public class LabsAddMemberViewModel
    {
        public Lab Lab { get; set; }
        
        public string Query { get; set; }

        public List<Result> Results { get; set; }

        public class Result : IUserWithLabManager
        {
            public ApplicationUser User { get; set; }

            public bool IsMember { get; set; }

            public bool IsLabManager { get; set; }
        }
    }

    public class LabsRemoveMemberViewModel
    {
        public Lab Lab { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }

        public string UserId { get; set; }
    }

    public class LabsEditViewModel : LabsCreateViewModel
    {
        public LabsEditViewModel()
        {
        }

        public LabsEditViewModel(Lab lab)
        {
            Lab = lab;
            CollegeName = lab.CollegeName;
            CourseCode = lab.CourseCode;
            WeekNumber = lab.WeekNumber;
            TestCode = lab.TestId;
            Location = lab.Location;
        }

        public Lab Lab { get; set; }
    }

    public class LabsReagentsViewModel
    {
        public Lab Lab { get; set; }

        public bool IsLabManager { get; set; }

        public List<UsedReagent> UsedReagents { get; set; }
    }

    public class LabsAddReagentViewModel
    {
        public Lab Lab { get; set; }
        
        public string Query { get; set; }

        public List<Reagent> Results { get; set; }
    }

    public class LabsConfirmAddReagentViewModel
    {
        public Lab Lab { get; set; }

        public Reagent Reagent { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Quantity to Use")]
        public int Quantity { get; set; }
    }

    public class LabsRemoveReagentViewModel
    {
        public Lab Lab { get; set; }
        
        public Reagent Reagent {get; set; }

        public long UsedReagentId { get; set; }

        [Display(Name = "Return Quantity")]
        [Required]
        [Range(0, 1000)]
        public int ReturnQuantity { get; set; }
    }

    public class LabsSamplesViewModel
    {
        public Lab Lab { get; set; }

        public string Query { get; set; }

        public List<LabSample> LabSamples { get; set; }

        public bool IsLabManager { get; set; }
    }

    public class LabsAddSampleViewModel
    {
        public Lab Lab { get; set; }
        
        public string Query { get; set; }

        public List<Sample> Results { get; set; }
    }

    public class LabsSampleDetailsViewModel
    {
        public LabSample LabSample { get; set; }

        public List<Result> Comments { get; set; }

        public bool IsLabManager { get; set; }

        [Display(Name = "Comment")]
        [StringLength(1000, MinimumLength = 0)]
        public string Message { get; set; }

        [Required]
        [Range(0, 3)]
        public int SelectedButton { get; set; }

        public class Result : IUserWithLabManager
        {
            public ApplicationUser User { get; set; }

            public LabSampleComment Comment { get; set; }

            public bool IsLabManager { get; set; }
        }
    }

    public class LabsEditSampleViewModel
    {
        public Lab Lab { get; set; }

        public LabSample LabSample { get; set; }

        [Display(Name = "Notes")]
        [Required]
        [StringLength(100000)]
        public string Notes { get; set; }
    }

    public class LabsRemoveSampleViewModel
    {
        public Lab Lab { get; set; }

        public LabSample LabSample { get; set; }
    }

    public class LabsReportViewModel
    {
        public Lab Lab { get; set; }

        public List<MemberResult> Members { get; set; }

        public List<SampleResult> LabSamples { get; set; }

        public List<ReagentResult> UsedReagents { get; set; }

        public class MemberResult : IUserWithLabManager
        {
            public ApplicationUser User { get; set; }

            public bool IsLabManager { get; set; }
        }

        public class SampleResult
        {
            public Sample Sample { get; set; }

            [Display(Name = "Status")]
            public LabSampleStatus Status { get; set; }
            
            [Display(Name = "Assigned")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm:ss tt}", ApplyFormatInEditMode = true)]
            public DateTimeOffset AssignedDate { get; set; }
        }

        public class ReagentResult
        {
            public Reagent Reagent { get; set; }

            [Display(Name = "Quantity")]
            public int Quantity { get; set; }
        }
    }
}
