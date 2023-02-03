using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muses.Domain.Models
{
    public class User
    {
        public User()
        {
            Role = new Role();
        }

        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Navigations

        public Role Role { get; set; }
        public List<Song> Songs { get; set; }
    }
}
