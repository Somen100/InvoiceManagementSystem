using InvoiceMgmt.BAL.IService;
using InvoiceMgmt.DAL.IRepo;
using InvoiceMgmt.DAL.Repo;
using InvoiceMgmt.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceMgmt.BAL.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepo _invoiceRepo;

        public InvoiceService(IInvoiceRepo invoiceRepo)
        {
            _invoiceRepo = invoiceRepo;
        }

        public Task<Invoice> AddOnlyInvoiceAsync(Invoice invoice, List<IFormFile>? uploadedFiles)
        {
            return _invoiceRepo.AddOnlyInvoiceAsync(invoice, uploadedFiles);
        }
        public async Task<Invoice> UpdateOnlyInvoiceAsync(Invoice invoice)
        {
            return await _invoiceRepo.UpdateOnlyInvoiceAsync(invoice);
        }

        public async Task<Invoice> CreateInvoice(Invoice invoice, List<InvoiceItem> invoiceItems)
        {
            return await _invoiceRepo.CreateInvoice(invoice, invoiceItems);
        }

        public Task DeleteInvoice(int id)
        {
            return _invoiceRepo.DeleteInvoice(id);
        }

        public Task<IEnumerable<Invoice>> GetAllInvoice(int pageNumber = 1, int pageSize = 10, string? invoiceNumber = null, string? status = null, int? customerId = null)
        {
            return _invoiceRepo.GetAllInvoice(pageNumber, pageSize, invoiceNumber, status, customerId);
        }

        public async Task<Invoice> GetInvoiceById(int id)
        {
            return await _invoiceRepo.GetInvoiceById(id);
        }

        public Task<Invoice> GetInvoiceByNumber(string invoiceNo)
        {
           return _invoiceRepo.GetInvoiceByNumber(invoiceNo);
        }

        public async Task<Invoice> UpdateInvoice(int id, Invoice invoice, List<InvoiceItem> updatedInvoiceItems)
        {
            return await _invoiceRepo.UpdateInvoice(id, invoice, updatedInvoiceItems);
        }

    }
}
