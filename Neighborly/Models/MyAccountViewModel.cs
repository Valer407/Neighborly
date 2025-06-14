using System.Collections.Generic;

namespace Neighborly.Models
{
    public class MyAccountViewModel
    {
        public List<ListingViewModel> Listings { get; set; }
        public List<ListingViewModel> Favorites { get; set; }
    }
}