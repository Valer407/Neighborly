using System;
using System.Collections.Generic;
using Neighborly.Models.DBModels;

namespace Neighborly.Models
{
    public class MessagesIndexViewModel
    {
        public List<ChatListItemViewModel> Chats { get; set; } = new();
        public ChatViewModel? ActiveChat { get; set; }
    }

    public class ChatListItemViewModel
    {
        public int Id { get; set; }
        public User Participant { get; set; }
        public string? ListingTitle { get; set; }
        public bool IsActive { get; set; }
        public MessagePreview? LastMessage { get; set; }
    }

    public class ChatViewModel
    {
        public int Id { get; set; }
        public User Participant { get; set; }
        public string? ListingTitle { get; set; }
        public bool IsActive { get; set; }
        public List<MessageViewModel> Messages { get; set; } = new();
    }

    public class MessagePreview
    {
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public List<Categories> RatingCategories { get; set; } = new();
    }

    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsFromMe { get; set; }
        public bool IsRead { get; set; }
    }
}