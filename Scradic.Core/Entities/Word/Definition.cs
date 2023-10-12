using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scradic.Core.Entities
{
    public class Definition
    {
        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? Translation { get; set; }
        public int WordId { get; set; }

        [ForeignKey(nameof(WordId))]
        public Word? Word { get; set; }
    }
}
