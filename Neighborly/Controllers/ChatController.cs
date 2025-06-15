using Microsoft.AspNetCore.Mvc;
using Neighborly.Data;
using Neighborly.Models.DBModels;
using Neighborly.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Neighborly.Controllers
{
    public class ChatController : Controller
    {
        private readonly NeighborlyContext _context;

        public ChatController(NeighborlyContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var chatsQuery = _context.Chats
                .Where(c => c.User1Id == userId || c.User2Id == userId);

            var chatList = chatsQuery
                .Select(c => new ChatListItemViewModel
                {
                    Id = c.ChatId,
                    ListingTitle = _context.Listings.Where(l => l.ListingId == c.ListingId).Select(l => l.Title).FirstOrDefault(),
                    IsActive = c.ClosedAt == null,
                    Participant = (c.User1Id == userId ? _context.Users.FirstOrDefault(u => u.UserId == c.User2Id) : _context.Users.FirstOrDefault(u => u.UserId == c.User1Id))!,
                    LastMessage = _context.Messages.Where(m => m.ChatId == c.ChatId)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => new MessagePreview
                        {
                            Text = m.Content,
                            Timestamp = m.SentAt,
                            IsRead = m.ReadAt != null
                        }).FirstOrDefault()
                }).ToList();

            ChatViewModel? active = null;
            if (id != null)
            {
                var chatEntity = chatsQuery.FirstOrDefault(c => c.ChatId == id);
                if (chatEntity != null)
                {
                    var otherUser = chatEntity.User1Id == userId ? chatEntity.User2Id : chatEntity.User1Id;
                    var user = _context.Users.FirstOrDefault(u => u.UserId == otherUser);
                    var messages = _context.Messages
                        .Where(m => m.ChatId == chatEntity.ChatId)
                        .OrderBy(m => m.SentAt)
                        .Select(m => new MessageViewModel
                        {
                            Id = m.MessageId,
                            Text = m.Content,
                            Timestamp = m.SentAt,
                            IsFromMe = m.SenderId == userId,
                            IsRead = m.ReadAt != null
                        }).ToList();

                    active = new ChatViewModel
                    {
                        Id = chatEntity.ChatId,
                        ListingTitle = _context.Listings.Where(l => l.ListingId == chatEntity.ListingId).Select(l => l.Title).FirstOrDefault(),
                        IsActive = chatEntity.ClosedAt == null,
                        Participant = user!,
                        Messages = messages
                    };

                    // mark unread messages as read
                    var unread = _context.Messages
                        .Where(m => m.ChatId == chatEntity.ChatId && m.SenderId != userId && m.ReadAt == null)
                        .ToList();
                    if (unread.Any())
                    {
                        foreach (var m in unread)
                        {
                            m.ReadAt = DateTime.UtcNow;
                        }
                        _context.SaveChanges();
                    }
                }
            }
            var categories = _context.Categories.ToList();
            var model = new MessagesIndexViewModel
            {
                Chats = chatList,
                ActiveChat = active,
                RatingCategories = categories
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult SendMessage(int chatId, string text)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                return RedirectToAction(nameof(Index), new { id = chatId });
            }

            var message = new Messages
            {
                ChatId = chatId,
                SenderId = userId,
                Content = text,
                SentAt = DateTime.UtcNow
            };
            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index), new { id = chatId });
        }

        [HttpPost]
        public IActionResult StartFromListing(int listingId, string text)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return RedirectToAction("Details", "Listings", new { id = listingId });
            }

            var listing = _context.Listings.FirstOrDefault(l => l.ListingId == listingId);
            if (listing == null)
            {
                return NotFound();
            }

            if (listing.UserId == userId)
            {
                // User cannot message themselves
                return RedirectToAction("Details", "Listings", new { id = listingId });
            }

            var chat = _context.Chats.FirstOrDefault(c => c.ListingId == listingId &&
                ((c.User1Id == userId && c.User2Id == listing.UserId) ||
                 (c.User1Id == listing.UserId && c.User2Id == userId)));

            if (chat == null)
            {
                chat = new Chats
                {
                    ListingId = listingId,
                    User1Id = userId,
                    User2Id = listing.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Chats.Add(chat);
                _context.SaveChanges();
            }

            var message = new Messages
            {
                ChatId = chat.ChatId,
                SenderId = userId,
                Content = text,
                SentAt = DateTime.UtcNow
            };
            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new { id = chat.ChatId });
        }

        [HttpPost]
        public IActionResult CloseChat(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var chat = _context.Chats.FirstOrDefault(c => c.ChatId == id &&
                (c.User1Id == userId || c.User2Id == userId));
            if (chat == null)
            {
                return NotFound();
            }

            if (chat.ClosedAt == null)
            {
                chat.ClosedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index), new { id });
        }

        [HttpPost]
        public IActionResult RateChat(int chatId, int score, int? categoryId, string? comment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized();
            }

            var chat = _context.Chats.FirstOrDefault(c => c.ChatId == chatId &&
                (c.User1Id == userId || c.User2Id == userId));
            if (chat == null || chat.ClosedAt == null)
            {
                return NotFound();
            }

            var rateeId = chat.User1Id == userId ? chat.User2Id : chat.User1Id;

            var rating = new User_ratings
            {
                RaterId = userId.Value,
                RateeId = rateeId ?? 0,
                ListingId = chat.ListingId ?? 0,
                Score = score,
                Comment = comment ?? string.Empty,
                CategoryId = categoryId,
                CreatedAt = DateTime.UtcNow
            };

            _context.User_Ratings.Add(rating);

            var ratee = _context.Users.FirstOrDefault(u => u.UserId == rateeId);
            if (ratee != null)
            {
                ratee.RatingAvg = (ratee.RatingAvg * ratee.RatingCount + score) /
                    (ratee.RatingCount + 1);
                ratee.RatingCount += 1;
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new { id = chatId });
        }
    }
}