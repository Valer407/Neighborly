﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace Neighborly.Models.DBModels
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [MaxLength(255)]
        public string AvatarUrl { get; set; }

        public int? CityId { get; set; }

        public int? DistrictId { get; set; }

        [ForeignKey("CityId")]
        public Cities? City { get; set; }

        [ForeignKey("DistrictId")]
        public Distircts? District { get; set; }

        public string? About { get; set; }

        [Range(0, 5)]
        public float RatingAvg { get; set; } = 0;

        public int RatingCount { get; set; } = 0;

        public bool IsAdmin { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public ICollection<Listings> Listings { get; set; }
    }


}
