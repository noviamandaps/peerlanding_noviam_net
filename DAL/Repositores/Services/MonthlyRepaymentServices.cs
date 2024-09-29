using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Model;
using DAL.Repositores.Services.Interfaces;
using DAL.Repositores.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositores.Services
{
    public class MonthlyRepaymentServices : IMonthlyPaymentsServices
    {
        private readonly PeerlandingContext _peerLandingContext;
        private readonly IRepaymentServices _repaymentService;
        private readonly ILoanServices _loanService;
        private readonly IUserServices _userServices;

        public MonthlyRepaymentServices(PeerlandingContext peerLandingContext, IRepaymentServices repaymentService, ILoanServices loanService, IUserServices userServices)
        {
            _peerLandingContext = peerLandingContext;
            _repaymentService = repaymentService;
            _loanService = loanService;
            _userServices = userServices;
        }
        public async Task<string> GenerateMonthlyPatments(string repaymentId)
        {
            var repayment = await _repaymentService.GetRepaymentById(repaymentId);

            if (repayment == null)
            {
                throw new Exception("No Repayment Detected!");
            }

            for (int i = 1; i < 12; i++)
            {
                var data = new TrnMonthlyPayments
                {
                    Repayment_id = repaymentId,
                    Status = false,
                    Amount = repayment.Amount / 12,
                };

                await _peerLandingContext.AddAsync(data);
                await _peerLandingContext.SaveChangesAsync();
            }

            return repayment.Id;
        }

        public async Task<List<ResMonthlyPaymentDto>> GetMonthlyPaymentByRepaymentId(string repaymentId)
        {
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

        public async Task<string> PayMonthlyPayments(List<ReqPayMonthlyPaymentDto> payments)
        {
            foreach (var paymentDto in payments)
            {
                var payment = await _peerLandingContext.TrnMonthlyPayments
                                .FirstOrDefaultAsync(p => p.Id == paymentDto.PaymentId);

                if (payment == null)
                {
                    throw new Exception($"Payment with ID {paymentDto.PaymentId} not found");
                }

                //payment.Status = true;

                //_peerLandingContext.TrnMonthlyPayments.Update(payment);

                //var repaymen = await _repaymentService.GetRepaymentById(payment.RepaymentId);

                //var loan = await _loanService.GetLoanById(repaymen.LoanId);

                //var funding = await _peerLandingContext.TrnFundings
                //.SingleOrDefaultAsync(l => l.LoanId == loan.LoanId);

                //if (funding == null)
                //{
                //    throw new Exception("funding not found");
                //}

                //var borrower = await _userServices.GetUserById(loan.User.Id);

                //var lender = await _userServices.GetUserById(funding.LenderId);

                //await _userServices.Update(lender.Id, new ReqUpdateUserDto
                //{
                //    Balance = lender.Balance + payment.Amount
                //});

                //await _userServices.Update(borrower.Id, new ReqUpdateUserDto
                //{
                //    Balance = borrower.Balance - payment.Amount
                //});

                //var repaymenEdit = await _peerLandingContext.TrnRepayments
                //.SingleOrDefaultAsync(r => r.Id == payment.RepaymentId);

                //if (repaymenEdit == null)
                //{
                //    throw new Exception("funding not found");
                //}

                //repaymenEdit.RepaidAmount += payment.Amount;
                //repaymenEdit.BalanceAmount -= payment.Amount;
            }

            await _peerLandingContext.SaveChangesAsync();

            return "Payments updated successfully";
        }

      
        


    }
}
