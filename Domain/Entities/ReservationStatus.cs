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
        Expired,//1
        PaymentInProgress,//2
        Paid,//3
    }
}
