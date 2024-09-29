using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositores.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/loan/[action]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanServices _loanServices;

        public LoanController(ILoanServices loanServices)
        {
            _loanServices = loanServices;
        }

        [HttpPost]
        public async Task<IActionResult> NewLoan(ReqLoanDto loan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        });
                    var errorMessage = new StringBuilder("Validation errors occurred!");

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }
                var res = await _loanServices.CreateLoan(loan);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Loan added",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut]
        [Authorize(Roles = "lender")]
        public async Task<IActionResult> UpdateLoan(ReqUpdateLoanDto updateLoan, string id)
        {
            try
            {
                var response = await _loanServices.UpdateLoan(updateLoan, id);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Succes Updating Loan",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Loan did not exist")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });

            }
        }





        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LoanList(string idBorrower, string? status, string? idLender)
        {
            try
            {
                var res = _loanServices.LoanList(idBorrower, status, idLender);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Success Getting Loan List",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteLoan(string id)
        {
            try
            {
                var result = await _loanServices.DeleteLoan(id);
                if (result)
                {
                    return Ok(new { message = $"Loan with ID {id} has been deleted successfully." });
                }
                return BadRequest(new { message = "Failed to delete the loan." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoanDetail(string id)
        {
            try
            {
                var loanDetail = await _loanServices.GetLoanDetail(id);
                return Ok(new ResBaseDto<ResLoanDetailDto>
                {
                    Success = true,
                    Message = "Loan detail retrieved successfully",
                    Data = loanDetail
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLoans()
        {
            var result = await _loanServices.GetAllLoans();
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        // GET https://localhost:7248/ApiLoan/GetAllLoansByUserId?userId=c5517421-fa00-4075-9e7c-99d7ed525aea 404 (Not Found)
        [HttpGet]
        public async Task<IActionResult> GetAllLoansByUserId(string userId)
        {
            try
            {
                var result = await _loanServices.GetAllLoansByUserId(userId);
                return Ok(new ResBaseDto<List<ResListLoanDto>>
                {
                    Success = true,
                    Message = "Success Getting Loan List",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> FundLoan(ReqFundLoanDto fundLoanDto)
        {

            try
            {
                await _loanServices.FundLoanAsync(fundLoanDto.LenderId, fundLoanDto.LoanId);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Loan funded successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLoanHistoryForLender(string lenderId)
        {
            try
            {
                var result = await _loanServices.GetLoanHistoryForLenderAsync(lenderId);
                return Ok(new ResBaseDto<IEnumerable<ResRepaymentDto>>
                {
                    Success = true,
                    Message = "Success Getting Loan History",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}