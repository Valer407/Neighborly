using System;

namespace Neighborly.Models
{
    public class ListingDetailsViewModel
    {
        public ListingDetails Listing { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class ListingDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; }
        public LocationViewModel Location { get; set; }
        public ListingCardUserViewModel User { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}