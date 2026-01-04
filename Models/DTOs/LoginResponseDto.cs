using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Models.DTOs
{
    public sealed class LoginResponseDto
    {
        public string Token { get; set; } = "";
        public DateTimeOffset ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public int PrivilegeLevel { get; set; }
    }
}
