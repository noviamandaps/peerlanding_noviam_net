using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqUpdateUserDto
    {
        [Required(ErrorMessage = "name is required")]
        [MaxLength(30, ErrorMessage = "name cannot exceed 30 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "role is required")]
        [MaxLength(30, ErrorMessage = "role cannot be exceed 30 characters")]
        public string Role { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "balance must be a positive value")]
        public decimal? Balance { get; set; }
    }
}
