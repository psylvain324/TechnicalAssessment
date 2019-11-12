using System;
using System.ComponentModel.DataAnnotations;

namespace TechnicalAssessment.Models
{
    public class UploadFile
    {
        [Key]
        public int FileId { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "Extension Type")]
        public ExtensionTypes ExtensionType { get; set; }

        [Display(Name = "Size")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Size { get; set; }

        [Display(Name = "Upload Date")]
        [DataType(DataType.DateTime)]
        public DateTime UploadDate { get; set; }
    }

    public enum ExtensionTypes
    {
        CSV = 0,
        XML = 1
    }
}
