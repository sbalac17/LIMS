using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace LIMS.Models
{
    public class RoleViewModel
    {
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class EditUserViewModel
    {
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        
        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}
