using DAL.DTO.Req;
using DAL.DTO.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositores.Services.Interfaces
{
    public interface ILoanServices
    {
        Task<string> CreateLoan(ReqLoanDto loan);
        //Task<string> UpdateLoanStatus(string id, ReqUpdateLoanDto reqLoan);
        //Task<List<ResListLoanDto>> LoanList(string idBorrower, string? status, string? idLender);

        Task<bool> DeleteLoan(string id);
        Task<ResLoanDetailDto> GetLoanDetail(string id);
        Task<ResBaseDto<List<ResListLoanDto>>> GetAllLoans();
        //======

        Task<string> UpdateLoan(ReqUpdateLoanDto reqUpdate, string loanId);
        Task LoanList(string idBorrower, string? status, string? idLender);
        //Task<ResListLoanDto> GetLoanById(string loanId);
        //Task<List<ResListLoanDto>> GetAllLoans(string status);
        //Task<List<ResListLoanDto>> GetAllLoansByUserId(string id);

        // GetAllLoans
        Task<List<ResListLoanDto>> GetAllLoansByUserId(string id);

        // FundLoanAsync
        Task FundLoanAsync(string lenderId, string loanId);

        Task<IEnumerable<ResRepaymentDto>> GetLoanHistoryForLenderAsync(string lenderId);


    }
}
