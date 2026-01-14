using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Models.DTOs
{
    public class UserDataDto
    {
        public string UserName {  get; set; } = string.Empty;
        public string? Password { get; set; } = null;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber {  get; set; } = string.Empty;
        public string City {  get; set; } = string.Empty;
    }
}
