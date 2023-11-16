using Domain.Entities;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.QrCodeeGenerator
{
    public interface IQrCodeGeneratorService : IService
    {
        void GenerateQrCode(AssignedTicket ticket);
    }
}
