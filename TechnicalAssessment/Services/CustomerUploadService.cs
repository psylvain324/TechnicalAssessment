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
    public class CustomerUploadService : IServiceUpload<Customer>
    {
        private DatabaseContext databaseContext;
        private readonly IFormatProvider formatProvider;
        private readonly ILog logger;

        public CustomerUploadService(DatabaseContext databaseContext)
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
                    doc.Load(file.OpenReadStream());
                    XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Customers/Customer");
                    var customers = ParseXmlNodes(nodes);
                    foreach (Customer customer in customers)
                    {
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

        public List<Customer> ParseXmlNodes(XmlNodeList xmlNodes)
        {
            List<Customer> customers = new List<Customer>();
            foreach (XmlNode xmlNode in xmlNodes)
            {
                Customer customer = new Customer
                {
                    CustomerId = int.Parse(xmlNode.Attributes["id"].Value, formatProvider),
                    CustomerName = xmlNode.SelectSingleNode("CustomerName").InnerText,
                    Email = xmlNode.SelectSingleNode("Email").InnerText,
                    MobileNumber = xmlNode.SelectSingleNode("MobileNumber").InnerText
                };

                customers.Add(customer);
            }

            return customers;
        }
    }
}
