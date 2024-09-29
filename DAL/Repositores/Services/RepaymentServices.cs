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
    public class RepaymentServices : IRepaymentServices
    {
        private readonly PeerlandingContext _peerLandingContext;

        public RepaymentServices(PeerlandingContext peerLandingContext)
        {
            _peerLandingContext = peerLandingContext;
        }

        public async Task<string> CreateRepayment(ReqCreatePaymentDto reqCreate)
        {
            var newRepayment = new TrnRepayment
            {
                Loan_id = reqCreate.LoanId,
                Amount = reqCreate.Amount,
                Repaid_amount = reqCreate.RepaidAmount,
                Balance_amount = reqCreate.BalanceAmount,
            };

            await _peerLandingContext.AddAsync(newRepayment);
            await _peerLandingContext.SaveChangesAsync();

            return newRepayment.Id;
        }



        public async Task<ResRepaymentDto> GetRepaymentById(string id)
        {
            var repayment = await _peerLandingContext.TrnRepayments
            .Where(r => r.Id == id)
            .Select(r => new ResRepaymentDto
            {
                Id = r.Id,
                Amount = r.Amount,
                BalanceAmount = r.Balance_amount,
                LoanId = r.Loan_id,
                PaidAt = r.PaidAt,
                RepaidAmount = r.Repaid_amount,
                RepaidStatus = r.Repaid_status

            })
            .FirstOrDefaultAsync();

            if (repayment == null)
            {
                throw new Exception("User not found");
            }

            return repayment;
        }

        public async Task<ResRepaymentDto> GetRepaymentByLoanId(string loanId)
        {
            var repayment = await _peerLandingContext.TrnRepayments
                .Where(r => r.Loan_id == loanId)
                .Select(r => new
                {
                    r.Id,
                    r.Loan_id,
                    r.Amount,
                    r.Repaid_amount,
                    r.Balance_amount,
                    r.Repaid_status,
                    r.PaidAt
                })
                .FirstOrDefaultAsync();

            if (repayment == null)
            {
                throw new Exception("Repayment not found");
            }

            var result = new ResRepaymentDto
            {
                Id = repayment.Id,
                LoanId = repayment.Loan_id,
                Amount = repayment.Amount,
                RepaidAmount = repayment.Repaid_amount,
                BalanceAmount = repayment.Balance_amount,
                RepaidStatus = repayment.Repaid_status,
                PaidAt = repayment.PaidAt,
            };

            return result;
        }

        // GetMonthlyPaymentByRepaymentId
        public async Task<List<ResMonthlyPaymentDto>> GetMonthlyPaymentByLoanId(string loanId)
        {

            var repaymentId = await _peerLandingContext.TrnRepayments
                .Where(r => r.Loan_id == loanId)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var query = _peerLandingContext.TrnMonthlyPayments
               .Select(payment => new ResMonthlyPaymentDto
               {
                   Id = payment.Id,
                   Amount = payment.Amount,
                   RepaymentId = payment.Repayment_id,
                   status = payment.Status
               })
               .Where(payment => payment.RepaymentId == repaymentId);

            query = query.OrderBy(payment => payment.Id);

            return await query.ToListAsync();
        }

        public async Task<string> PayMultipleMonthlyPayment(ReqPayMultipleMonthlyPaymentDto reqPayMultipleMonthlyPaymentDto)
        {
            var paymentIds = reqPayMultipleMonthlyPaymentDto.PaymentIds;

            foreach (var paymentId in paymentIds)
            {
                var payment = await _peerLandingContext.TrnMonthlyPayments
                    .Where(p => p.Id == paymentId)
                    .FirstOrDefaultAsync();

                if (payment == null)
                {
                    throw new Exception("Payment not found");
                }

                payment.Status = true;

                // Update balance amount
                var repayment = await _peerLandingContext.TrnRepayments
                    .Where(r => r.Id == payment.Repayment_id)
                    .FirstOrDefaultAsync();

                if (repayment == null)
                {
                    throw new Exception("Repayment not found");
                }

                repayment.Balance_amount -= payment.Amount;
                repayment.Repaid_amount += payment.Amount;
                // repayment.Repaid_status = "on-paid";
                if (repayment.Repaid_amount <= repayment.Amount)
                {
                    repayment.Repaid_status = "on-paid";
                }
                else
                {
                    repayment.Repaid_status = "done";
                }

                var loan = await _peerLandingContext.MstLoans
                     .Where(l => l.Id == repayment.Loan_id)
                     .FirstOrDefaultAsync();

                if (loan == null)
                {
                    throw new Exception("Loan not found");
                }

                if (repayment.Repaid_status == "done")
                {
                    loan.Status = "repaid";
                }
                else
                {
                    loan.Status = "on-repaid";
                }

                var borrowerId = loan.BorrowerId;
                // Update balance amount
                var borrower = await _peerLandingContext.MstUsers
                    .Where(b => b.Id == borrowerId)
                    .FirstOrDefaultAsync();

                if (borrower == null)
                {
                    throw new Exception("Borrower not found");
                }

                borrower.Balance -= payment.Amount;


                var funding = await _peerLandingContext.TrnFundings
                    .Where(f => f.Loan_id == repayment.Loan_id)
                    .FirstOrDefaultAsync();

                var lenderId = funding.Lender_id;
                // Update balance amount
                var lender = await _peerLandingContext.MstUsers
                    .Where(l => l.Id == lenderId)
                    .FirstOrDefaultAsync();

                if (lender == null)
                {
                    throw new Exception("Lender not found");
                }

                lender.Balance += payment.Amount;
            }

            await _peerLandingContext.SaveChangesAsync();

            return "Payment success";
        }
    }
}
