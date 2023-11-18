using System.Reflection.Metadata;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using sovosTask.Controllers;
using sovosTask.Models;
using System.Collections.Generic;

namespace sovosTaskTEST
{
    public class Test
    {
        [Fact]
        public void Test_GetInvoices()
        {
            // Arrange
            var controller = new InvoiceController();

            // Act
            var result = controller.GetInvoices();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var invoices = Assert.IsType<List<Invoice>>(viewResult.Value);
            Assert.NotNull(invoices);
        }

        [Fact]
        public void Test_GetInvoice()
        {
            // Arrange
            var invoiceService = new InvoiceService();
            var invoiceId = 1;

            // Act
            var result = invoiceService.GetInvoice(invoiceId);

            // Assert
            Assert.NotNull(result);
            
        }

        [Fact]
        public void Test_UploadDocument()
        {
            // Arrange
            var documentService = new DocumentService();
            var document = new Document();

            // Act
            var result = documentService.UploadDocument(document);

            // Assert
            Assert.True(result);
           
        }

        [Fact]
        public void Test_ProcessJsonFiles()
        {
            // Arrange
            var scheduler = new JobScheduler();

            // Act
            var result = scheduler.ProcessJsonFiles();

            // Assert
            Assert.True(result);
           
        }
    }
}