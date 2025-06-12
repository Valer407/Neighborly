using System;
using System.Collections.Generic;
using Neighborly.Models.DBModels;

namespace Neighborly.Models
{
    public class ListingsIndexViewModel
    {
        public List<Categories> Categories { get; set; }
        public List<ListingCardViewModel> Listings { get; set; }
    }

    public class ListingCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; }
        public LocationViewModel Location { get; set; }
        public ListingCardUserViewModel User { get; set; }
    }

    public class ListingCardUserViewModel
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public float Rating { get; set; }
    }
}