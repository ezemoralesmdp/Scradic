using System.ComponentModel.DataAnnotations;

namespace Scradic.Core.Entities
{
    public class PDFInfo
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string FolderPath { get; set; }
        public long Size { get; set; }
        public int TotalWords { get; set; }
        public DateTime FileCreationDate { get; set; }
    }
}