using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIMS.Models
{
    public class ReagentsSearchViewModel
    {
        public string Query { get; set; }

        public List<Reagent> Results { get; set; }
    }

    public class ReagentsCreateViewModel
    {
        [Display(Name = "Name")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Display(Name = "Quantity")]
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Display(Name = "Expires")]
        [Required]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = DateWithTime.PlaceholderDate, ApplyFormatInEditMode = true)]
        public Date ExpiryDate { get; set; }
        
        [Display(Name = "Manufacturer Code")]
        [Required]
        [StringLength(100)]
        public string ManufacturerCode { get; set; }
    }

    public class ReagentsEditViewModel : ReagentsCreateViewModel
    {
        public ReagentsEditViewModel()
        {
        }

        public ReagentsEditViewModel(Reagent reagent)
        {
            Reagent = reagent;
            Name = reagent.Name;
            Quantity = reagent.Quantity;
            ExpiryDate = reagent.ExpiryDate;
            ManufacturerCode = reagent.ManufacturerCode;
        }

        public Reagent Reagent { get; set; }
    }
}