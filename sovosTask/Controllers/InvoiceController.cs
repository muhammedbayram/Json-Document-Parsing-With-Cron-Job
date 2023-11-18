using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sovosTask.Concrete;
using sovosTask.Interfaces;
using sovosTask.Models;

namespace sovosTask.Controllers
{
    public class InvoiceController : Controller
    {

        private readonly InvoiceDbContext _dbContext;
        private readonly JobScheduler _jobScheduler;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public InvoiceController(InvoiceDbContext dbContext, JobScheduler jobScheduler, IEmailService emailService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _jobScheduler = jobScheduler;
            _configuration = configuration;
            _emailService = emailService;

        }


        // GET: InvoiceController
        public async Task<IActionResult> Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetInvoices()
        {

            var invoiceHeaders = await _dbContext.InvoiceHeaders.ToListAsync();
            return Ok(invoiceHeaders);
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoice(string invoiceId)
        {
            try
            {
                var invoice = _dbContext.InvoiceHeaders.Include("InvoiceLines").FirstOrDefault(i => i.InvoiceId == invoiceId);

                if (invoice == null)
                {
                    return NotFound("Document not found");
                }

                var invoiceData = new
                {
                    invoiceHeader = invoice,
                    invoiceLines = invoice.InvoiceLines
                };

                var serializerSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                var jsonData = JsonConvert.SerializeObject(invoiceData, serializerSettings);
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Document fetch error: {ex.Message}");
            }
        }



        [HttpPost]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            //_dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            if (file != null)
            {
                try
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        var jsonString = reader.ReadToEnd();

                        var jsonObject = JObject.Parse(jsonString);
                        var invoiceHeader = jsonObject["InvoiceHeader"].ToObject<InvoiceHeader>();
                        var invoiceLines = jsonObject["InvoiceLine"].ToObject<List<InvoiceLine>>();

                        var isExistingInvoiceHeader = _dbContext.InvoiceHeaders.Any(x => x.InvoiceId == invoiceHeader.InvoiceId);
                        if (!isExistingInvoiceHeader)
                        {
                            // Add InvoiceHeader
                            _dbContext.InvoiceHeaders.Add(invoiceHeader);
                            await _dbContext.SaveChangesAsync();

                            // Add Invoice Lines
                            foreach (var invoiceLine in invoiceLines)
                            {
                                // Set InvoiceHeaderId
                                invoiceLine.InvoiceHeaderId = invoiceHeader.InvoiceId;
                                _dbContext.InvoiceLines.Add(invoiceLine);
                            }

                            await _dbContext.SaveChangesAsync();

                            string emailBody = $"Document Information:\n InvoiceId: {invoiceHeader.InvoiceId},\n SenderTitle: {invoiceHeader.SenderTitle},\n ReceiverTitle: {invoiceHeader.ReceiverTitle},\n InvoiceDate: {invoiceHeader.Date}";
                            string receiverEmail = _configuration.GetValue<string>("EmailOptions:ReceiverEmail");
                            _emailService.SendEmail(receiverEmail, "A new document has been saved.", emailBody);

                            return Json(new { message = "Document upload successful" });

                        }
                        return Json(new { message = "This document has been uploaded before !" });

                    }
                }
                catch (Exception ex)
                {
                    return Json(new { error = "Document upload error", message = ex.Message });
                }
            }
            return Json(new { message = "No document selected to upload !" });

        }


        [HttpPost]
        public async Task<IActionResult> DeleteAll()
        {

            try
            {
                foreach (var item in _dbContext.InvoiceHeaders)
                {
                    _dbContext.InvoiceHeaders.Remove(item);
                }

                await _dbContext.SaveChangesAsync();
                return Json(new { message = "Document deletion successful" }); // alert çalışmadığı için Json a çevirdim

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Document deletion error: {ex.Message}");
            }

        }




        // GET: InvoiceController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: InvoiceController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InvoiceController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InvoiceController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: InvoiceController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InvoiceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: InvoiceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
