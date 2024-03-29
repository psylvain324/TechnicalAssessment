﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechnicalAssessment.Data;
using TechnicalAssessment.Models;
using TechnicalAssessment.Models.ViewModels;
using TechnicalAssessment.Services;
using TechnicalAssessment.Services.Interfaces;

namespace TechnicalAssessment.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> logger;
        private readonly DatabaseContext databaseContext;
        private readonly IServiceUpload<Transaction> transactionUploadService;
        private readonly IServiceExport<Transaction> transactionExportService;

        public TransactionController(ILogger<TransactionController> logger, DatabaseContext databaseContext, 
            IServiceUpload<Transaction> transactionUploadService, IServiceExport<Transaction> transactionExportService)
        {
            this.logger = logger;
            this.databaseContext = databaseContext;
            this.transactionUploadService = transactionUploadService;
            this.transactionExportService = transactionExportService;
        }

        //GET: Transaction
        public async Task<IActionResult> TransactionIndex(string sortOrder, int? pageNumber)
        {
            ViewData["CurrencyCodeSortParm"] = sortOrder =="Code" ? "code_desc" : "Code";
            ViewData["TransactionDateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var transactions = from t in databaseContext.Transactions select t;
            switch (sortOrder)
            {
                case "Code":
                    transactions = transactions.OrderBy(s => s.CurrencyCode);
                    break;
                case "code_desc":
                    transactions = transactions.OrderByDescending(s => s.CurrencyCode);
                    break;
                case "Date":
                    transactions = transactions.OrderBy(s => s.TransactionDate);
                    break;
                case "date_desc":
                    transactions = transactions.OrderByDescending(s => s.TransactionDate);
                    break;
                default:
                    transactions = transactions.OrderBy(s => s.CurrencyCode);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Transaction>.CreateAsync(transactions.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //GET: Transaction/TransactionDetails/{id}
        [Route("/TransactionDetails/{id}")]
        public async Task<IActionResult> TransactionDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await databaseContext.Transactions.FirstOrDefaultAsync().ConfigureAwait(false);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        //GET: Transaction/TransactionDelete/{id}
        public async Task<IActionResult> TransactionDelete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var transaction = await databaseContext.Transactions.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        //POST: Transaction/TransactionDelete/{id}
        [HttpPost, ActionName("TransactionDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransactionConfirmDelete(int id)
        {
            var transaction = await databaseContext.Transactions.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            databaseContext.Transactions.Remove(transaction);
            await databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("TransactionIndex");
        }

        public IActionResult TransactionCreate()
        {
            return View();
        }

        // POST: Transaction/TransactionCreate/{transaction}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransactionCreate([Bind("Id,TransactionId,TransactionDate,Amount,CurrencyCode,Status")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                databaseContext.Transactions.Add(transaction);
                await databaseContext.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction("TransactionIndex");
            }
            return View(transaction);
        }

        // GET: Transaction/TransactionEdit/{id}
        [Route("/TransactionEdit/{id}")]
        public async Task<IActionResult> TransactionEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactions = await databaseContext.Transactions.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (transactions == null)
            {
                return NotFound();
            }
            return View(transactions);
        }

        //GET: Transaction/TransactionSearch/{search}/{field}
        [Route("/TransactionSearch/{search}/{field}")]
        public IActionResult TransactionSearch(string search, string field)
        {
            var transactions = from t in databaseContext.Transactions select t;
            switch (field)
            {
                case "TransactionId":
                    transactions = from t in databaseContext.Transactions
                                   where t.TransactionId == search
                                   select t;
                    break;
                case "CurrencyCode":
                    transactions = from t in databaseContext.Transactions
                                   where t.CurrencyCode == search
                                   select t;
                    break;
                case "Status":
                    transactions = from t in databaseContext.Transactions
                                   where t.Status.ToString() == search
                                   select t;
                    break;
                default:
                    return NotFound();
            }

            return View(transactions);
        }

        //TODO: Refactor this.
        [HttpPost, ActionName("Upload")]
        public ActionResult UploadTransaction(List<IFormFile> files)
        {
            if (files != null)
            {
                foreach (IFormFile file in files)
                {
                    if (file.Length > 1000000)
                    {
                        logger.LogInformation("Request File Size was too large. File was: " + file.Length + " Bytes.");
                        return BadRequest();
                    }
                    var filePath = file.FileName;
                    if (Path.GetExtension(filePath) == ".csv")
                    {
                        transactionUploadService.UploadCsv(file);
                    }
                    else if (Path.GetExtension(filePath) == ".xml")
                    {
                        transactionUploadService.UploadXml(file);
                    }
                    else
                    {
                        logger.LogInformation(Path.GetExtension(filePath) + " is not a supported format.");
                        return BadRequest();
                    }
                }
            }
            else
            {
                return BadRequest();
            }

            return RedirectToAction("TransactionIndex", "Transaction");
        }

        //TODO: Refactor this.
        [HttpPost, ActionName("Export")]
        public ActionResult ExportTransaction(List<Transaction> transactions, string fileType)
        {
            string docName = "Transactions - " + DateTime.Now;
            string fileContentType;
            string docPath;
            string fileContent;

            if (fileType == "Csv")
            {
                try
                {
                    fileContentType = "text/csv";
                    docPath = transactionExportService.CsvExport(transactions, docName);
                    fileContent = transactionExportService.CsvExport(transactions, docName);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else if (fileType == "Xml")
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    fileContentType = "text/xml";
                    docPath = transactionExportService.XmlExport(transactions, docName);
                    xmlDoc.Load(docPath);
                    XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("/Transactions/Transaction");
                    fileContent = transactionUploadService.ParseXmlNodes(nodes).ToString();
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            else
            {
                logger.LogInformation("There was a problem with the Request.");
                return BadRequest();
            }

            return File(fileContent, fileContentType, docPath);
        }
    }
}
