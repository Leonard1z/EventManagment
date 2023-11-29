using Domain.Entities;
using Infrastructure.Repositories.AssigneTicket;
using Infrastructure.Repositories.Registrations;
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
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IQrCodeGeneratorService _qrCodeGeneratorService;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPdfGeneratorService _pdfGeneratorService;
        public AssigneTicketService(IAssigneTicketRepository assigneeTicketRepository,
            IRegistrationRepository registrationRepository,
            IQrCodeGeneratorService qrCodeGeneratorService,
            IEmailService emailService,
            IWebHostEnvironment webHostEnvironment,
            IPdfGeneratorService pdfGeneratorService
            )
        {
            _assigneeTicketRepository = assigneeTicketRepository;
            _registrationRepository = registrationRepository;
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
                    var registration = await _registrationRepository.GetById(ticket.RegistrationId);
                    if (registration != null)
                    {
                        registration.Quantity -= 1;
                        await _registrationRepository.UpdateAsync(registration);
                        _qrCodeGeneratorService.GenerateQrCode(ticket);
                        var result = await _assigneeTicketRepository.CreateAsync(ticket);
                        await SendEmailAsync(result);
                    }
                    else
                    {
                        throw new InvalidOperationException("Registration not found for the specified ticket. Cannot assign ticket.");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<AssignedTicket> GetTicketByTicketNumberAsync(int ticketNumber)
        {
            return await _assigneeTicketRepository.GetTicketByTicketNumberAsync(ticketNumber);
        }

        public async Task UpdateAsync(AssignedTicket ticket)
        {
            await _assigneeTicketRepository.UpdateAsync(ticket);
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
