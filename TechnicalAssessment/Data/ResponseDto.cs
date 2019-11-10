using System;
namespace TechnicalAssessment.Data
{
    public class ResponseDto<T>
    {
        public bool Result { get; set; }
        public T Date { get; set; }
        public String ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
    }
}