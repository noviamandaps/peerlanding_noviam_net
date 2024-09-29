using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositores.Services.Interfaces
{
    public interface IFundingServices
    {
        Task<string> CreateFunding(ReqCreateFundingDto reqCreate);
        Task<string> FundingLoan(ReqFundingLoanDto reqFunding);
        Task<List<ResFundingDto>> GetFundingsByLenderId(string lenderId);
        Task<ResFundingByIdDto> GetFundingByLoanId(string loanId);
    }
}
