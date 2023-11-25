using Domain.Entities;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.QrCodeeGenerator
{
    public class QrCodeGeneratorService : IQrCodeGeneratorService
    {
        public void GenerateQrCode(AssignedTicket ticket)
        {

            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode(
                $"{{ \"FirstName\": \"{ticket.FirstName}\", \"LastName\": \"{ticket.LastName}\", \"ExpireDate\": \"{ticket.ExpireDate.ToString("yyyy-MM-ddTHH:mm:ss")}\" }}", QRCodeGenerator.ECCLevel.Q);


            var qrCode = new QRCode(qrCodeData);
            var bitmap = qrCode.GetGraphic(4);

            var imageConverter = new ImageConverter();
            var imageBytes = (byte[])imageConverter.ConvertTo(bitmap, typeof(byte[]));

            ticket.QrCodeData = Convert.ToBase64String(imageBytes);
        }
    }
}
