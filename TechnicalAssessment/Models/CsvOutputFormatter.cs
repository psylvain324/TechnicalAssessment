using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace TechnicalAssessment.Models
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            if (typeof(Transaction).IsAssignableFrom(type) || typeof(IEnumerable<Transaction>).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }
            return false;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            var logger = serviceProvider.GetService(typeof(ILogger<CsvOutputFormatter>)) as ILogger;

            var response = context.HttpContext.Response;
            var buffer = new StringBuilder();
            if (context.Object is IEnumerable<Transaction>)
            {
                foreach (Transaction transaction in context.Object as IEnumerable<Transaction>)
                {
                    FormatCsvFile(buffer, transaction, logger);
                }
            }
            else
            {
                var transaction = context.Object as Transaction;
                FormatCsvFile(buffer, transaction, logger);
            }
            await response.WriteAsync(buffer.ToString());
        }

        private static void FormatCsvFile(StringBuilder buffer, Transaction transaction, ILogger logger)
        {
            buffer.AppendLine(
                transaction.TransactionId + ", " +
                transaction.Amount + ", " +
                transaction.CurrencyCode + ", " +
                transaction.TransactionDate + ", " +
                transaction.Status + "\r\n");
            logger.LogInformation("Writing Csv File for Transaction ID: {TransactionId}", transaction.TransactionId);
        }
    }
}
