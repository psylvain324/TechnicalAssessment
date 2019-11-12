using System;
using System.ComponentModel.DataAnnotations;

namespace TechnicalAssessment.Models
{
    public class UploadFile
    {
        public int FileId { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "Extension Type")]
        public ExtensionTypes ExtensionType { get; set; }

        [Display(Name = "Size (bytes)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Size { get; set; }

        [Display(Name = "Uploaded (UTC)")]
        [DataType(DataType.DateTime)]
        public DateTime UploadDT { get; set; }
    }

    public enum ExtensionTypes
    {
        CSV = 0,
        XML = 1
    }
}
