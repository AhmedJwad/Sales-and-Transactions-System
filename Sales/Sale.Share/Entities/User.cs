using Microsoft.AspNetCore.Identity;
using Sale.Share.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
   public class User: IdentityUser
    {
        [Display(Name = "First Name")]
        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last Name")]
        [MaxLength(50)]
        [Required]
        public string LastName { get; set; } = null!;

        [Display(Name = "Address")]
        [MaxLength(200)]
        [Required]
        public string Address { get; set; } = null!;

        [Display(Name = "Photo")]
        public string? Photo { get; set; }

        [DefaultValue("+964")]
        [Display(Name = "Country Code")]
        [MaxLength(5)]
        [Required]
        public string CountryCode { get; set; } = null!;

        [Display(Name = "Type of user")]
        public UserType UserType { get; set; }
        [JsonIgnore]
        public City? City { get; set; }

        [Display(Name = "City")]
        [Range(1, int.MaxValue)]
        public int CityId { get; set; }

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        [Display(Name = "User")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
