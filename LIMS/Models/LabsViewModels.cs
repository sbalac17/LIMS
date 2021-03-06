﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LIMS.Models.Api;
using Newtonsoft.Json;

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
        public long LabId { get; set; }

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
        public string TestId { get; set; }

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
        public string TestId { get; set; }

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
            public string UserId { get; set; }
            
            [Display(Name = "Username")]
            public string UserName { get; set; }

            public bool IsLabManager { get; set; }

            [Display(Name = "Last Active")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
            [JsonIgnore]
            public DateTimeOffset? LastActive { get; set; }

            [JsonProperty(nameof(LastActive))]
            public DateWithTime? LastActiveJson
            {
                get => LastActive;
                set => LastActive = value;
            }
        }
    }

    public class LabsAddMemberViewModel
    {
        public Lab Lab { get; set; }
        
        public string Query { get; set; }

        public List<Result> Results { get; set; }

        public class Result : IUserWithLabManager
        {
            public string UserId { get; set; }

            [Display(Name = "Username")]
            public string UserName { get; set; }

            public bool IsMember { get; set; }

            public bool IsLabManager { get; set; }
        }
    }

    public class LabsRemoveMemberViewModel
    {
        public Lab Lab { get; set; }

        [Display(Name = "Username")]
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
            TestId = lab.TestId;
            Location = lab.Location;
        }

        public Lab Lab { get; set; }
    }

    public class LabsReagentsViewModel
    {
        public Lab Lab { get; set; }

        public bool IsLabManager { get; set; }

        public List<Result> UsedReagents { get; set; }

        public class Result
        {
            public long UsedReagentId { get; set; }

            public long ReagentId { get; set; }
            
            [Display(Name = "Name")]
            public string ReagentName { get; set; }
            
            [Display(Name = "Manufacturer Code")]
            public string ReagentManufacturerCode { get; set; }
            
            [Display(Name = "Expires")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = Date.Placeholder, ApplyFormatInEditMode = true)]
            [JsonIgnore]
            public DateTimeOffset ReagentExpiryDate { get; set; }
            
            [JsonProperty(nameof(ReagentExpiryDate))]
            public Date ReagentExpiryDateJson
            {
                get => ReagentExpiryDate;
                set => ReagentExpiryDate = value;
            }

            [Display(Name = "Use Date")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
            [JsonIgnore]
            public DateTimeOffset UsedDate { get; set; }

            [JsonProperty(nameof(UsedDate))]
            public DateWithTime UsedDateJson
            {
                get => UsedDate;
                set => UsedDate = value;
            }
            
            [Display(Name = "Quantity")]
            public int Quantity { get; set; }
        }
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

        public List<Result> LabSamples { get; set; }

        public bool IsLabManager { get; set; }

        public class Result
        {
            [JsonIgnore]
            public long LabId { get; set; }
            
            public long SampleId { get; set; }

            [Display(Name = "Test Code")]
            public string SampleTestId { get; set; }
            
            [Display(Name = "Description")]
            public string SampleDescription { get; set; }
            
            [Display(Name = "Taken")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
            [JsonIgnore]
            public DateTimeOffset SampleAddedDate { get; set; }

            [JsonProperty(nameof(SampleAddedDate))]
            public DateWithTime UsedDateJson
            {
                get => SampleAddedDate;
                set => SampleAddedDate = value;
            }
            
            [Display(Name = "Assigned At")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
            [JsonIgnore]
            public DateTimeOffset AssignedDate { get; set; }

            [JsonProperty(nameof(AssignedDate))]
            public DateWithTime AssignedDateJson
            {
                get => AssignedDate;
                set => AssignedDate = value;
            }
            
            [Display(Name = "Status")]
            public LabSampleStatus Status { get; set; }
        }
    }

    public class LabsAddSampleViewModel
    {
        public Lab Lab { get; set; }
        
        public string Query { get; set; }

        public List<Sample> Results { get; set; }
    }

    public class LabsSampleDetailsViewModel : Api.LabsApiCommentModel
    {
        public LabSample LabSample { get; set; }

        public Sample Sample => LabSample.Sample;

        public List<Result> Comments { get; set; }

        public bool IsLabManager { get; set; }

        [Required]
        [Range(0, 3)]
        [JsonIgnore]
        public int SelectedButton
        {
            get => RequestedStatus.HasValue ? (int)RequestedStatus.Value + 1 : 0;
            set => RequestedStatus = value != 0 ? (LabSampleStatus?)(value - 1) : null;
        }

        public class Result : IUserWithLabManager
        {
            public string UserId { get; set; }
            
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [JsonIgnore]
            public LabSampleComment Comment { get; set; }

            [JsonIgnore]
            public DateTimeOffset Date => Comment.Date;

            [JsonProperty(nameof(Date))]
            public DateWithTime DateJson => Comment.Date;

            public string Message => Comment.Message;
            public LabSampleStatus? NewStatus => Comment.NewStatus;

            public bool IsLabManager { get; set; }
        }
    }

    public class LabsEditSampleViewModel : LabsApiEditSampleModel
    {
        public Lab Lab { get; set; }

        public LabSample LabSample { get; set; }
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
            public string UserId { get; set; }

            [Display(Name = "Username")]
            public string UserName { get; set; }

            public bool IsLabManager { get; set; }
        }

        public class SampleResult
        {
            public Sample Sample { get; set; }

            [Display(Name = "Status")]
            public LabSampleStatus Status { get; set; }
            
            [Display(Name = "Assigned")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
            public DateTimeOffset AssignedDate { get; set; }

            [JsonProperty(nameof(AssignedDate))]
            public DateWithTime AssignedDateJson
            {
                get => AssignedDate;
                set => AssignedDate = value;
            }
        }

        public class ReagentResult
        {
            public Reagent Reagent { get; set; }

            [Display(Name = "Quantity")]
            public int Quantity { get; set; }
        }
    }
}
