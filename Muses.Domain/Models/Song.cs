using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muses.Domain.Models
{
    public class Song
    {
        public Song()
        {
            User = new User();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string SongName { get; set; } = string.Empty;
        public string Transcript { get; set; } = string.Empty;

        // Navigations

        public User User { get; set; }
    }
}
