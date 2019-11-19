using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using CsvHelper;
using log4net;
using Microsoft.AspNetCore.Http;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Services.Interfaces;

namespace TechnicalAssessment.Services
{
    public class CustomerService : IServiceUpload
    {
        private DatabaseContext databaseContext;
        private readonly IFormatProvider formatProvider;
        private readonly ILog logger;

        public CustomerService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
            formatProvider = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.ThreeLetterISOLanguageName);
            logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void UploadCsv(IFormFile file)
        {
            if (file != null)
            {
                try
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    using (var csv = new CsvReader(reader))
                    {
                        var customers = new List<Customer>();
                        csv.Read();
                        csv.ReadHeader();
                        while (csv.Read())
                        {
                            var customer = new Customer
                            {
                                CustomerId = csv.GetField<int>("CustomerId"),
                                CustomerName = csv.GetField<string>("CustomerName"),
                                Email = csv.GetField<string>("Email"),
                                MobileNumber = csv.GetField<string>("MobileNumber")
                            };

                            databaseContext.Customers.Add(customer);
                        }
                    }
                }
                catch (ReaderException e)
                {
                    logger.Error(e.InnerException);
                }
            }

            databaseContext.SaveChanges();
        }

        public void UploadXml(IFormFile file)
        {
            XmlDocument doc = new XmlDocument();
            if (file != null)
            {
                doc.Load(file.OpenReadStream());
                try
                {
                    XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Customers/Customer");

                    foreach (XmlNode node in nodes)
                    {
                        Customer customer = new Customer
                        {
                            CustomerId = int.Parse(node.Attributes["id"].Value, formatProvider),
                            CustomerName = node.SelectSingleNode("CustomerName").InnerText,
                            Email = node.SelectSingleNode("Email").InnerText,
                            MobileNumber = node.SelectSingleNode("MobileNumber").InnerText
                        };

                        databaseContext.Customers.Add(customer);
                    }
                }
                catch (XmlException e)
                {
                    logger.Error(e.InnerException);
                }
            }

            databaseContext.SaveChanges();
        }
    }
}
