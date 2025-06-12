namespace Neighborly.Models.DBModels
{
    public class DataGenerator
    {
        public static List<User> GenerateUsers(int count)
        {
            var users = new List<User>();
            var random = new Random();

            for (int i = 1; i <= count; i++)
            {
                users.Add(new User
                {
                    FirstName = $"First{i}",
                    LastName = $"Last{i}",
                    Email = $"user{i}@example.com",
                    PasswordHash = $"hash{i}",
                    AvatarUrl = $"https://example.com/avatar/{i}.png",
                    RatingAvg = (float)(random.NextDouble() * 5.0),
                    RatingCount = random.Next(0, 100),
                    IsAdmin = i % 50 == 0, // co 50. admin
                    IsActive = i % 10 != 0, // co 10. nieaktywny
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 365)),
                    LastLogin = DateTime.UtcNow.AddMinutes(-random.Next(0, 100000))
                });
            }

            return users;
        }
    }
}
