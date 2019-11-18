using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Services.Interfaces;

namespace TechnicalAssessment.Services
{
    public class CustomerService : IServiceUpload
    {
        private DatabaseContext databaseContext;

        public CustomerService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public void UploadCsv(IFormFile file)
        {
            IFormatProvider provider = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.ThreeLetterISOLanguageName);
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

            databaseContext.SaveChanges();
        }

        public void UploadXml(IFormFile file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.OpenReadStream());
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Customers/Customer");

            foreach (XmlNode node in nodes)
            {
                Customer customer = new Customer
                {
                    CustomerId = int.Parse(node.Attributes["id"].Value),
                    CustomerName = node.SelectSingleNode("CustomerName").InnerText,
                    Email = node.SelectSingleNode("Email").InnerText,
                    MobileNumber = node.SelectSingleNode("MobileNumber").InnerText
                };

                databaseContext.Customers.Add(customer);
            }

            databaseContext.SaveChanges();
        }
    }
}
