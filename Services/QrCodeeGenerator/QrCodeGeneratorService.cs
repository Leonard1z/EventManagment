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
        private static HashSet<int> generatedNumbers = new HashSet<int>();
        public void GenerateQrCode(AssignedTicket ticket)
        {

            ticket.TicketNumber = GenerateUniqueRandomNumber(1, 1000000);
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode(
                $"{{\"FirstName\": \"{ticket.FirstName}\", \"LastName\": \"{ticket.LastName}\", \"EventName\": \"{ticket.EventName}\", \"TicketNumber\": \"{ticket.TicketNumber}\", \"Email\": \"{ticket.Email}\", \"ExpireDate\": \"{ticket.ExpireDate.ToString("yyyy-MM-ddTHH:mm:ss")}\" }}", QRCodeGenerator.ECCLevel.Q);
         
            var qrCode = new QRCode(qrCodeData);
            var bitmap = qrCode.GetGraphic(4);

            var imageConverter = new ImageConverter();
            var imageBytes = (byte[])imageConverter.ConvertTo(bitmap, typeof(byte[]));

            ticket.QrCodeData = Convert.ToBase64String(imageBytes);
        }
        private static int GenerateUniqueRandomNumber(int minValue, int maxValue)
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                while (true)
                {
                    byte[] randomNumber = new byte[4];
                    rng.GetBytes(randomNumber);

                    int result = BitConverter.ToInt32(randomNumber, 0);

                    // Ensure the result is within the specified range
                    result = Math.Abs(result % (maxValue - minValue + 1)) + minValue;

                    // Check if the number is unique
                    if (generatedNumbers.Add(result))
                    {
                        return result;
                    }
                }
            }
        }
    }
}
