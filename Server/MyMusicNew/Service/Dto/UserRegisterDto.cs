using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class UserRegisterDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6), RegularExpression(@"^(?=.*[0-9])(?=.*[a-zA-Z]).{6,20}$")]
        public string Password { get; set; } = string.Empty;

    }
}
