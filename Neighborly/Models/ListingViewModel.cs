using System;

namespace Neighborly.Models
{
    public class ListingViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public LocationViewModel Location { get; set; }
    }

    public class LocationViewModel
    {
        public string City { get; set; }
        public string District { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}