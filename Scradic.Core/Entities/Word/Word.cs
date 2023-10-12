using System.ComponentModel.DataAnnotations;

namespace Scradic.Core.Entities
{
    public class Word
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? GramaticalCategory { get; set; }
        public string? AnotherSuggestion { get; set; }
        public List<Definition>? Definitions { get; set; }
        public List<Example>? Examples { get; set; }
        public bool Pdf { get; set; }
        public int Hits { get; set; } = 1;
        public DateTime InsertDate { get; set; } = DateTime.Now;

        public Word()
        {
            Definitions = new List<Definition>();
            Examples = new List<Example>();
        }
    }
}
