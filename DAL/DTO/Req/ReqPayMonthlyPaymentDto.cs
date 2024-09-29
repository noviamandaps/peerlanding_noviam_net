using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqPayMonthlyPaymentDto
    {
        public string PaymentId { get; set; }
    }

    // req multiple pay monthly payment
    public class ReqPayMultipleMonthlyPaymentDto
    {
        public List<string> PaymentIds { get; set; }
    }
}
