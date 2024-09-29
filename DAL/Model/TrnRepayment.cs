using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Model
{
    [Table("trn_repayment")]
    public class TrnRepayment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Loans")]
        [Column("loan_id")]
        public string Loan_id { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("repaid_amount")]
        public decimal Repaid_amount { get; set; }

        [Required]
        [Column("balance_amount")]
        public decimal Balance_amount { get; set; }

        [Required]
        [Column("repaid_status")]
        public string Repaid_status { get; set; }

        [Required]
        [Column("paid_at")]
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        // Foreign key relationship
        public MstLoans Loans { get; set; }
    }
}