using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;


namespace DAL.Repositores.Services.Interfaces
{
    public interface IRepaymentServices
    {
        public Task<string> CreateRepayment(ReqCreatePaymentDto reqCreate);
        public Task<ResRepaymentDto> GetRepaymentByLoanId(string loanId);
        public Task<ResRepaymentDto> GetRepaymentById(string id);

        public Task<List<ResMonthlyPaymentDto>> GetMonthlyPaymentByLoanId(string loanId);

        public Task<string> PayMultipleMonthlyPayment(ReqPayMultipleMonthlyPaymentDto reqPayMultipleMonthlyPaymentDto);
    }
}
