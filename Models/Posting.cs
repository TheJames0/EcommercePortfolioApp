using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models
{
    public class Posting
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] ImageData { get; set; }
        public byte[] ImageThumbnail { get; set; }
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public int Price { get; set; }
        public DateTime Postdt { get; set; }

    }
}
