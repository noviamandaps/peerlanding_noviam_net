using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResFundingDto
    {
        public string Id { get; set; }
        public Borrower Borrower { get; set; }
        public decimal Amount { get; set; }
        public decimal RepaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public string RepaidStatus { get; set; }
        public DateTime PaidAt { get; set; }
    }

    public class Borrower
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
