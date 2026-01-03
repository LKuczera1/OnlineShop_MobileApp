using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Models
{
    public class Product
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Stock { get; set; }
        public string? PictureName { get; set; } = string.Empty;
        public string? ThumbnailName { get; set; } = string.Empty;

        public ImageSource? ImageSource { get; set; } = null;
        public ImageSource? ThumbnailSource { get; set; } = null;
    }
}
