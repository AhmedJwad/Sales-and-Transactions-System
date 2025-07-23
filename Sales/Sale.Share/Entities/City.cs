using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class City
    {
        public int Id { get; set; }
        [Display(Name = "State")]
        [MaxLength(100, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "the field{0}is required")]
        public string Name { get; set; } = null!;
        public int StateId { get; set; }       
        public State? State { get; set; }      
        public ICollection<User>? Users { get; set; }
    }
}
