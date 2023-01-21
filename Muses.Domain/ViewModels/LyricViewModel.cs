using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muses.Domain.ViewModels
{
    public class GetLyricViewModel
    {
        [Required]
        [Display(Name = "ArtistName")]
        public string ArtistName { get; set; } = string.Empty;
        [Required]
        [Display(Name = "SongName")]
        public string SongName { get; set; } = string.Empty;
    }
}
