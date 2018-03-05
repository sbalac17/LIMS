using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMS.Models
{
    public class SamplesSearchViewModel
    {
        public string Query { get; set; }

        public List<Result> Results { get; set; }

        public class Result
        {
            public Sample Sample { get; set; }

            // can be null
            [Display(Name = "Assigned Lab")]
            public LabSample LabSample { get; set; }
        }
    }

    public class SamplesDetailsViewModel
    {
        public Sample Sample { get; set; }
    }

    public class SamplesCreateViewModel
    {
        [Display(Name = "Test Code")]
        [Required]
        public string TestCode { get; set; }

        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Taken")]
        [Required]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateWithTime AddedDate { get; set; }
    }

    public class SamplesEditViewModel : SamplesCreateViewModel
    {
        public Sample Sample { get; set; }

        public SamplesEditViewModel()
        {
        }

        public SamplesEditViewModel(Sample sample)
        {
            Sample = sample;
            TestCode = sample.TestId;
            Description = sample.Description;
            AddedDate = sample.AddedDate;
        }
    }
}
