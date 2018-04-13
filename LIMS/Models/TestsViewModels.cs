using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMS.Models
{
    public class TestsSearchViewModel
    {
        public string Query { get; set; }

        public List<Result> Results { get; set; }

        public class Result
        {
            [Display(Name = "Test Code")]
            public string TestId { get; set; }

            [Display(Name = "Name")]
            public string Name { get; set; }
        }
    }

    public class TestsCreateViewModel
    {
        [Display(Name = "Test Code")]
        [Required]
        [StringLength(20, MinimumLength = 3)]
        [RegularExpression("^[A-Z0-9]*$", ErrorMessage = "Test Code may only contain A-Z and 0-9.")]
        public string TestId { get; set; }
        
        [Display(Name = "Name")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required]
        [StringLength(10000)]
        public string Description { get; set; }
    }

    public class TestsEditViewModel
    {
        public TestsEditViewModel()
        {
        }

        public TestsEditViewModel(Test test)
        {
            TestId = test.TestId;
            Name = test.Name;
            Description = test.Description;
        }

        [Display(Name = "Test Code")]
        [StringLength(20, MinimumLength = 3)]
        [RegularExpression("^[A-Z0-9]*$", ErrorMessage = "Test Code may only contain A-Z and 0-9.")]
        public string TestId { get; }

        [Display(Name = "Name")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required]
        [StringLength(10000)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }

    public class TestsDeleteViewModel
    {
        public Test Test { get; set; }

        public bool CannotDelete { get; set; }
    }

    public class TestsAddSampleViewModel
    {
        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Taken")]
        [Required]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = DateWithTime.Placeholder, ApplyFormatInEditMode = true)]
        public DateWithTime AddedDate { get; set; }
    }
}
