using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum ReservationStatus
    {
        Active, //0
        PaymentInProgress,//1
        Paid,//2
        Expired,//3
    }
}
