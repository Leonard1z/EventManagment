using Domain.Entities;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services.PdfGenerator
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        public byte[] GeneratePdf(AssignedTicket ticket)
        {
           using (MemoryStream stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph($"Ticket Information\n\n" +
                                            $"Ticket: {ticket.TicketType}\n" +
                                            $"Event: {ticket.EventName}\n"));

                var qrCodeImage = new iText.Layout.Element.Image(iText.IO.Image.ImageDataFactory.Create(Convert.FromBase64String(ticket.QrCodeData)));
                document.Add(qrCodeImage);

                document.Close();

                return stream.ToArray();
            }
        }
    }
}
