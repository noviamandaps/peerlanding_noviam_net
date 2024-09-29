using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositores.Services.Interfaces
{
    public interface IMonthlyPaymentsServices
    {
        Task<string> GenerateMonthlyPatments(string repaymentId);
        Task<List<ResMonthlyPaymentDto>> GetMonthlyPaymentByRepaymentId(string repaymentId);
        Task<string> PayMonthlyPayments(List<ReqPayMonthlyPaymentDto> payments);
    }
}
