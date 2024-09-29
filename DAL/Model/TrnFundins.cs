using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    [Table("trn_fundings")]
    public class TrnFundings
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Loans")]
        [Column("loan_id")]
        public string Loan_id { get; set; }

        [Required]
        [ForeignKey("User")]
        [Column("lender_id")]
        public string Lender_id { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("funded_at")]
        public DateTime FundedAt { get; set; } = DateTime.UtcNow;

        // Foreign key relationships
        public MstLoans Loans { get; set; }
        public MstUser User { get; set; }
    }
}

