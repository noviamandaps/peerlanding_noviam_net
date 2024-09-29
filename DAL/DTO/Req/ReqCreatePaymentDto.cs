using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqCreatePaymentDto
    {
        [Required]
        public string LoanId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public decimal RepaidAmount { get; set; }
        [Required]
        public decimal BalanceAmount { get; set; }
        [Required]
        public string repaid_status { get; set; } = "on_repay";

        [Required]
        public DateTime paid_at { get; set; } = DateTime.UtcNow;
    }
}
