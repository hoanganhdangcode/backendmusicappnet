using Microsoft.EntityFrameworkCore;
using Net.MusicApp.Data;
using Net.MusicApp.DTOs;
using Net.MusicApp.Entities;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Net.MusicApp.Services
{
    public class AuthService
    {
        private readonly MusicAppDBContext _db;

        public AuthService(MusicAppDBContext dbContext)
        {
            _db = dbContext;
        }
        public async  Task<User?> getUserbyemail (string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

       





    }
}
