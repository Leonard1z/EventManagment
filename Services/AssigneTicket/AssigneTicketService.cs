using Domain.Entities;
using Infrastructure.Repositories.AssigneTicket;
using Microsoft.AspNetCore.Hosting;
using Services.PdfGenerator;
using Services.QrCodeeGenerator;
using Services.SendEmail;
using System.Net.Mail;

namespace Services.AssigneTicket
{
    public class AssigneTicketService : IAssigneTicketService
    {
        private readonly IAssigneTicketRepository _assigneeTicketRepository;
        private readonly IQrCodeGeneratorService _qrCodeGeneratorService;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPdfGeneratorService _pdfGeneratorService;
        public AssigneTicketService(IAssigneTicketRepository assigneeTicketRepository,
            IQrCodeGeneratorService qrCodeGeneratorService,
            IEmailService emailService,
            IWebHostEnvironment webHostEnvironment,
            IPdfGeneratorService pdfGeneratorService
            )
        {
            _assigneeTicketRepository = assigneeTicketRepository;
            _qrCodeGeneratorService = qrCodeGeneratorService;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _pdfGeneratorService = pdfGeneratorService;
        }

        public async Task<bool> AssignTicketsAsync(List<AssignedTicket> assigneeTicket)
        {
            try
            {
                foreach (var ticket in assigneeTicket)
                {
                    if(string.IsNullOrWhiteSpace(ticket.FirstName)||string.IsNullOrWhiteSpace(ticket.LastName) 
                        ||string.IsNullOrWhiteSpace(ticket.Email) || string.IsNullOrWhiteSpace(ticket.PhoneNumber))
                    {
                        throw new ArgumentException("Please provide all the required information for the ticket assignment");
                    }
                    _qrCodeGeneratorService.GenerateQrCode(ticket);
                    var result = await _assigneeTicketRepository.CreateAsync(ticket);
                    await SendEmailAsync(result);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        private async Task SendEmailAsync(AssignedTicket ticket)
        {
            var pathToFile = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
           + "Templates" + Path.DirectorySeparatorChar.ToString() + "EmailTemplates"
           + Path.DirectorySeparatorChar.ToString() + "TicketTemplate.html";


            string subject = "Ticket";
            string tittle = "Confirm Ticket";
            string message = "Your Ticket";
            string body = "";

            using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
            {
                body = streamReader.ReadToEnd();
            }

            string messageBody = string.Format(body,tittle, message);

            var pdfByteArray = _pdfGeneratorService.GeneratePdf(ticket);

            try
            {
                using(var stream = new MemoryStream(pdfByteArray))
                {
                    var atachment = new Attachment(stream, $"Ticket.pdf", "application/pdf");

                    await _emailService.SendEmailWithAtachmentAsync(ticket.Email, subject, messageBody,atachment);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }
    }
}
