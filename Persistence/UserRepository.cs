using System.Linq;
using PodNoms.Api.Models;
using PodNoms.Api.Persistence;
using PodNoms.Api.Utils;

namespace PodNoms.Api.Persistence {
    public class UserRepository : IUserRepository {
        private readonly PodnomsDbContext _context;

        public UserRepository(PodnomsDbContext context) {
            _context = context;
        }

        public User Get(int id) {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public User Get(string email) {
            return _context.Users.FirstOrDefault(u => u.EmailAddress == email);
        }

        public User AddOrUpdate(User entry) {
            if (entry.Id != 0) {
                _context.Users.Attach(entry);
            } else {
                _context.Users.Add(entry);

            }
            _context.SaveChanges();
            return entry;
        }

        public User UpdateRegistration(string email, string name, string sid, string providerId, string profileImage) {
            var user = _context.Users.FirstOrDefault(u => u.EmailAddress == email);

            if (user == null) {
                user = new User();
                user.EmailAddress = email;
                _context.Users.Add(user);
                _context.SaveChanges();
            }

            user.Sid = sid;
            user.FullName = string.IsNullOrEmpty(user.FullName) ? name : user.FullName;
            user.ProfileImage = profileImage;
            user.ProfileImage = profileImage;

            _context.Users.Attach(user);

            if (string.IsNullOrEmpty(user.ApiKey))
                UpdateApiKey(user.EmailAddress);
            _context.SaveChanges();

            return user;
        }

        public string UpdateApiKey(string email) {
            var newKey = "";
            var user = _context.Users.FirstOrDefault(u => u.EmailAddress == email);
            if (user != null) {
                do {
                    newKey = Randomisers.RandomString(16);
                } while (_context.Users.FirstOrDefault(u => u.ApiKey == newKey) != null);
            }
            user.ApiKey = newKey;
            _context.SaveChanges();
            return newKey;
        }
    }
}