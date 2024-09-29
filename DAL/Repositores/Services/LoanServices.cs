using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Model;
using DAL.Repositores.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL.Repositores.Services
{
    public class LoanServices : ILoanServices
    {
        private readonly PeerlandingContext _peerlandingContext;


        public LoanServices(PeerlandingContext peerlandingContext)
        {
            _peerlandingContext = peerlandingContext;
        }

        public async Task<string> CreateLoan(ReqLoanDto loan)
        {

            var newLoan = new MstLoans
            {
                BorrowerId = loan.BorrowerId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration
            };
            await _peerlandingContext.AddAsync(newLoan);
            await _peerlandingContext.SaveChangesAsync();
            return newLoan.BorrowerId;
        }

        public async Task<List<ResListLoanDto>> LoanList(string status)
        {
            var loans = await _peerlandingContext.MstLoans
                .Include(l => l.User)
                .Where(l => status == null || l.Status == status)
                .OrderByDescending(l => l.CreatedAt)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    //BorrowerName = loan.User.Name,
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status,
                    CreatedAt = loan.CreatedAt,
                    UpdatedAt = loan.UpdatedAt,
                }).ToListAsync();

            return loans;
        }


        public async Task<string> UpdateLoanStatus(string id, ReqUpdateLoanDto loan)
        {
            var existingLoan = await _peerlandingContext.MstLoans.FindAsync(id);
            if (existingLoan == null)
            {
                throw new Exception("Loan not found");
            }
            existingLoan.Status = loan.Status ?? existingLoan.Status;

            // You can add more properties to update if needed, for example:
            // existingLoan.Amount = loan.Amount;
            // existingLoan.InterestRate = loan.InterestRate;
            // existingLoan.Duration = loan.Duration;

            _peerlandingContext.MstLoans.Update(existingLoan);
            await _peerlandingContext.SaveChangesAsync();
            return existingLoan.Id;
        }

        public async Task<bool> DeleteLoan(string id)
        {
            var loan = await _peerlandingContext.MstLoans.FindAsync(id);
            if (loan == null)
                return false;
            _peerlandingContext.MstLoans.Remove(loan);
            var result = await _peerlandingContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<ResLoanDetailDto> GetLoanDetail(string id)
        {
            var loan = await _peerlandingContext.MstLoans
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
            {
                throw new Exception($"Loan with ID {id} not found.");
            }

            return new ResLoanDetailDto
            {
                LoanId = loan.Id,
                BorrowerName = loan.User.Name,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
                Status = loan.Status,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt
                // Tambahkan properti lain sesuai kebutuhan
            };
        }

        public async Task<ResBaseDto<List<ResListLoanDto>>> GetAllLoans()
        {
            try
            {
                var loans = await _peerlandingContext.MstLoans
                    .Include(l => l.User)
                    .OrderByDescending(l => l.CreatedAt)
                    .Select(loan => new ResListLoanDto

                    {
                        LoanId = loan.Id,
                        User = new User { Id = loan.User.Id, Name = loan.User.Name },
                        Amount = loan.Amount,
                        InterestRate = loan.InterestRate,
                        Duration = loan.Duration,
                        Status = loan.Status,
                        CreatedAt = loan.CreatedAt,
                        UpdatedAt = loan.UpdatedAt,
                    }).ToListAsync();

                return new ResBaseDto<List<ResListLoanDto>>
                {
                    Success = true,
                    Message = "Semua Loans ditampilkan",
                    Data = loans
                };
            }
            catch (Exception ex)
            {
                return new ResBaseDto<List<ResListLoanDto>>
                {
                    Success = false,
                    Message = $"Error menampilkan loans: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResEditLoanDto> EditStatus(string id, ReqEditLoanDto editLoan)
        {
            var loan = await _peerlandingContext.MstLoans.SingleOrDefaultAsync(x => x.Id == id);
            if (loan == null)
            {
                throw new Exception("Loan Not found");
            }
            loan.Status = editLoan.Status;
            _peerlandingContext.MstLoans.Update(loan);
            await _peerlandingContext.SaveChangesAsync();
            return new ResEditLoanDto
            {
                Id = id,
                Status = loan.Status
            };
        }

        public async Task<string> UpdateLoan(ReqUpdateLoanDto updateLoan, string id)
        {
            var findLoan = await _peerlandingContext.MstLoans.SingleOrDefaultAsync(e => e.Id == id);
            if (findLoan == null)
            {
                throw new Exception("Loan did not exist");
            }
            findLoan.Status = updateLoan.Status;
            findLoan.UpdatedAt = DateTime.UtcNow;

            await _peerlandingContext.SaveChangesAsync();

            return updateLoan.Status;
        }

        public Task LoanList(string idBorrower, string? status, string? idLender)
        {
            throw new NotImplementedException();
        }

        // GetAllLoansByUserId
        public async Task<List<ResListLoanDto>> GetAllLoansByUserId(string id)
        {
            var loans = await _peerlandingContext.MstLoans
                .Include(l => l.User)
                .Where(l => l.BorrowerId == id)
                .OrderByDescending(l => l.CreatedAt)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    User = new User { Id = loan.User.Id, Name = loan.User.Name },
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status,
                    CreatedAt = loan.CreatedAt,
                    UpdatedAt = loan.UpdatedAt,
                }).ToListAsync();
            return loans;

        }

        public async Task FundLoanAsync(string lenderId, string loanId)
        {
            var loan = await _peerlandingContext.MstLoans.FindAsync(loanId);
            if (loan == null)
                throw new KeyNotFoundException("Loan not found");

            var lender = await _peerlandingContext.MstUsers.FindAsync(lenderId);
            if (lender == null)
                throw new KeyNotFoundException("Lender not found");

            if (lender.Balance < loan.Amount)
                throw new InvalidOperationException("Insufficient balance");


            if (loan.Status != "requested")
                throw new InvalidOperationException("Loan is not available for funding");

            var funding = new TrnFundings
            {
                Id = Guid.NewGuid().ToString(),
                Lender_id = lenderId,
                Loan_id = loanId,
                Amount = loan.Amount,
                FundedAt = DateTime.UtcNow
            };

            await _peerlandingContext.TrnFundings.AddAsync(funding);

            loan.Status = "funded";
            loan.UpdatedAt = DateTime.UtcNow;
            loan.UpdatedAt = DateTime.UtcNow;
            // await _peerlandingContext.MstLoans.AddAsync(loan);

            lender.Balance -= loan.Amount;
            // await _peerlandingContext.MstUsers.AddAsync(lender);

            var borrower = await _peerlandingContext.MstUsers.FindAsync(loan.BorrowerId);
            borrower.Balance += loan.Amount;
            // await _peerlandingContext.MstUsers.AddAsync(borrower);


            // update table trn_repayment and table trn_monthly_payments
            // calculate repayment amount each month

            var balanceAmountWithInterest = loan.Amount + (loan.Amount * loan.InterestRate / 100);

            var repayment = new TrnRepayment
            {
                Id = Guid.NewGuid().ToString(),
                Loan_id = loanId,
                Amount = loan.Amount,
                Repaid_amount = 0,
                Balance_amount = balanceAmountWithInterest,
                Repaid_status = "unpaid",
                PaidAt = DateTime.UtcNow
            };
            await _peerlandingContext.TrnRepayments.AddAsync(repayment);

            var interestRate = loan.InterestRate;
            var interest = (loan.Amount * interestRate) / 100;
            var monthlyRepayment = (loan.Amount + interest) / loan.Duration;
            for (int i = 0; i < loan.Duration; i++)
            {
                var monthlyPayment = new TrnMonthlyPayments
                {
                    Id = Guid.NewGuid().ToString(),
                    Repayment_id = repayment.Id,
                    Amount = monthlyRepayment,
                    Status = false,
                    PaidAt = DateTime.UtcNow
                };
                await _peerlandingContext.TrnMonthlyPayments.AddAsync(monthlyPayment);
            }


            await _peerlandingContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ResRepaymentDto>> GetLoanHistoryForLenderAsync(string lenderId)
        {
            var loansIds = await _peerlandingContext.TrnFundings
                .Where(f => f.Lender_id == lenderId)
                .Select(f => f.Loan_id)
                .ToListAsync();

            var borrowerNames = await _peerlandingContext.MstLoans
                .Include(l => l.User)
                .Where(l => loansIds.Contains(l.Id))
                .Select(l => new { l.Id, l.User.Name })
                .ToDictionaryAsync(l => l.Id, l => l.Name);

            var repayments = await _peerlandingContext.TrnRepayments.Where(r => loansIds.Contains(r.Loan_id))
                .Select(r => new ResRepaymentDto
                {
                    Id = r.Id,
                    LoanId = r.Loan_id,
                    Amount = r.Amount,
                    RepaidAmount = r.Repaid_amount,
                    BalanceAmount = r.Balance_amount,
                    RepaidStatus = r.Repaid_status,
                    PaidAt = r.PaidAt
                }).ToListAsync();

            repayments.ForEach(r => r.BorrowerName = borrowerNames[r.LoanId]);

            return repayments;


        }
    }

}
