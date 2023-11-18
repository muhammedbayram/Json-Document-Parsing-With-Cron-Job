using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using sovosTask.Models;
using System;
using System.IO;
using Hangfire;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Timers;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using sovosTask.Interfaces;
using sovosTask.Concrete;
using sovosTask;
using Microsoft.Extensions.Options;

public class JobScheduler
{
    private readonly InvoiceDbContext _dbContext;
    //private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public JobScheduler(InvoiceDbContext dbContext, IEmailService emailService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _configuration = configuration;

    }


    public void ScheduleJob()
    {
        var jobId = BackgroundJob.Schedule(() => ProcessJsonFiles(), TimeSpan.FromSeconds(10));


        //RecurringJob.AddOrUpdate(() => ProcessJsonFiles(), "*/50 * * * * *");

    }

    public async Task ProcessJsonFiles()
    {

        // the folder containing the JSON documents
        string folderPath = Path.Combine(AppContext.BaseDirectory, "Documents");

        // Get JSON documents in folder
        string[] files = Directory.GetFiles(folderPath, "*.json");

        foreach (var file in files)
        {
            try
            {
                // Read JSON document
                using (var reader = new StreamReader(file))
                {
                    var jsonString = reader.ReadToEnd();

                    var jsonObject = JObject.Parse(jsonString);
                    var invoiceHeader = jsonObject["InvoiceHeader"].ToObject<InvoiceHeader>();


                    var isExistingInvoiceHeader = _dbContext.InvoiceHeaders.Any(x => x.InvoiceId == invoiceHeader.InvoiceId);
                    if (!isExistingInvoiceHeader)
                    {
                        // Add InvoiceHeader
                        _dbContext.InvoiceHeaders.Add(invoiceHeader);
                        await _dbContext.SaveChangesAsync();


                        var invoiceLines = jsonObject["InvoiceLine"].ToObject<List<InvoiceLine>>();

                        int maxId = 0;
                        //InvoiceLine test = _dbContext.InvoiceLines.OrderByDescending(x => x.Id).FirstOrDefault();
                        //if (test != null)
                        //    maxId = test.Id;

                        foreach (var invoiceLine in invoiceLines)
                        {
                            maxId = _dbContext.InvoiceLines.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();

                            // Create a new InvoiceLine object and copy the values
                            var newInvoiceLine = new InvoiceLine
                            {
                                Id = maxId + 1,
                                Name = invoiceLine.Name,
                                Quantity = invoiceLine.Quantity,
                                UnitCode = invoiceLine.UnitCode,
                                UnitPrice = invoiceLine.UnitPrice,
                            };

                            newInvoiceLine.InvoiceHeaderId = invoiceHeader.InvoiceId;

                            // Add the new InvoiceLine to the database
                            _dbContext.InvoiceLines.Add(newInvoiceLine);
                            await _dbContext.SaveChangesAsync();

                        }

                        Thread.Sleep(2000);

                        string emailBody = $"Doc Info:\n InvoiceId: {invoiceHeader.InvoiceId},\n SenderTitle: {invoiceHeader.SenderTitle},\n ReceiverTitle: {invoiceHeader.ReceiverTitle},\n InvoiceDate: {invoiceHeader.Date}";
                        string receiverEmail = _configuration.GetValue<string>("EmailOptions:ReceiverEmail");
                        _emailService.SendEmail(receiverEmail, "A new document has been saved.", emailBody);



                    }


                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}


