using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqRegisterUserDto
    {
        [Required(ErrorMessage = "name is required")]
        [MaxLength(30, ErrorMessage ="name cannot exceed 30 char")]
        public string Name { get; set; }

        [Required(ErrorMessage ="email is required")]
        [MaxLength(50, ErrorMessage = "email cannot exceed 50 char")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "password is required")]
        [MinLength(8, ErrorMessage = "password must be 8 char long")]
        [MaxLength(50, ErrorMessage = "password cannot exceed 50 char")]
        public string Password { get; set; }


        [Required(ErrorMessage = "role is required")]
        [MaxLength(30, ErrorMessage = "role cannot exceed 50 char")]
        public string Role { get; set; }


        [Range (0, double.MaxValue, ErrorMessage = "Balance must be a positive value")]
        public decimal? Balance { get; set; }

    }
}
