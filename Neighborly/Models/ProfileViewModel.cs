using System;
using System.Collections.Generic;

namespace Neighborly.Models
{
    public class ProfileViewModel
    {
        public ProfileUserViewModel User { get; set; }
        public List<ListingCardViewModel> Listings { get; set; }
        public List<ProfileReviewViewModel> Reviews { get; set; }
    }

    public class ProfileUserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public bool Verified { get; set; }
        public float Rating { get; set; }
        public DateTime MemberSince { get; set; }
    }

    public class ProfileReviewViewModel
    {
        public ReviewAuthorViewModel Author { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }

    public class ReviewAuthorViewModel
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
    }
}