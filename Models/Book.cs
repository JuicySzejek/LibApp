using System.ComponentModel.DataAnnotations;

namespace LibApp.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string Author { get; set; }
        public Genre Genre { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        public byte GenreId { get; set; }
        public DateTime DateAdded { get; set; }

        [DataType(DataType.Date,ErrorMessage ="Date type is required.")]
        public DateTime? ReleaseDate { get; set; }

        [Required(ErrorMessage = "Number in Stock is required")]
        [Range(1,20,ErrorMessage ="Number is out of Range 1-20")]
        public int NumberInStock { get; set; }
    }
}
