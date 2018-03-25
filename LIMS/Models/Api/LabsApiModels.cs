using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace LIMS.Models.Api
{
    public class LabsApiCommentModel : IValidatableObject
    {
        [Display(Name = "Comment")]
        [StringLength(1000, MinimumLength = 0)]
        [JsonIgnore]
        public string Message { get; set; }

        [JsonIgnore]
        public LabSampleStatus? RequestedStatus { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!RequestedStatus.HasValue && string.IsNullOrWhiteSpace(Message))
            {
                yield return new ValidationResult("Comment is required.", new[] { nameof(Message) });
            }
        }
    }

    public class LabsApiEditSampleModel
    {
        [Display(Name = "Notes")]
        [Required]
        [StringLength(100000)]
        public string Notes { get; set; }
    }
}
