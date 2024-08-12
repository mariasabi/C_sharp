using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Models
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       // [Column("Id")]
        public int Id { get; set; }
    //    [Column("Name")]
        public string? Name { get; set; }
     //   [Column("Type")]
        public string? Type { get; set; }
     //   [Column("Quantity")]
        public int? Quantity { get; set; }
      //  [Column("Price")]
        public decimal? Price { get; set; }
    }
}
