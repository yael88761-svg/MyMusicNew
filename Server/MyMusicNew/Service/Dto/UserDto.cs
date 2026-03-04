using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class UserDto
    {

        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required,EmailAddress]
        public string Email { get; set; }

    }
}
