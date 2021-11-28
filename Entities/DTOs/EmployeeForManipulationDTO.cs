using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs
{
    public abstract class EmployeeForManipulationDTO
    {
        [Required(ErrorMessage = "Employee name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string Name { get; set; }

        [Range(18, 200, ErrorMessage = "Age is required and it should be in range from 18 to 200")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Position is a required field.")]
        [MaxLength(20, ErrorMessage = "Maximum length for the Position is 20 characters")]
        public string Position { get; set; }
    }
}
