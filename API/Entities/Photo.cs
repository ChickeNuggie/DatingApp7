using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")] // create and specify table name. It will set relational table between AppUser and Photo
    public class Photo
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsMain { get; set; }

        public string PublicId { get; set; } //store photos in uniqe public ID

        //To fully define relationship in the table, ensure AppUserId and its AppUser property is included,
        //It ensures that AppUserId property is not nullable (not able to add photo to database without corresponding related user object to store)
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}