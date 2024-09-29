using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqCreateFundingDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string LoanId { get; set; }

        [Required]
        public string LenderId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime FundedAt { get; set; }
    }
}
