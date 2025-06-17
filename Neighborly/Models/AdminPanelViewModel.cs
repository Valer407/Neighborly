using System;
using System.Collections.Generic;
using Neighborly.Models.DBModels;

namespace Neighborly.Models
{
    public class AdminPanelViewModel
    {
        public List<AdminListingItem> Listings { get; set; } = new();
        public List<AdminUserItem> Users { get; set; } = new();
        public List<Categories> Categories { get; set; } = new();
        public List<AdminReportItem> Reports { get; set; } = new();
    }

    public class AdminListingItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }

    public class AdminUserItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime Joined { get; set; }
    }

    public class AdminReportItem
    {
        public int Id { get; set; }
        public string ListingTitle { get; set; }
        public string ReporterName { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }
}