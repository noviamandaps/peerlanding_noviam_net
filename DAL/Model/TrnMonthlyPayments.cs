using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    [Table("trn_monthly_payments")]
    public class TrnMonthlyPayments
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Repayment")]
        [Column("repayment_id")]
        public string Repayment_id { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("status")]
        public bool Status { get; set; }

        [Required]
        [Column("paid_at")]
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        // Foreign key relationship
        public TrnRepayment Repayment { get; set; }
    }
}
