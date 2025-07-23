using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class Country
    {
        public int Id { get; set; }
        [Display(Name = "Country")]
        [MaxLength(100, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "the field{0}is required")]
        public string Name { get; set; } = null!;
        [Display(Name = "State")]
        [JsonIgnore]
        public ICollection<State>? states { get; set; }      
        public int StatesNumber => states == null || states.Count == 0 ? 0 : states.Count;
    }
}
