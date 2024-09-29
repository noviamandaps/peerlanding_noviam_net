using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositores.Services.Interfaces;
using DAL.Repositores.Services;
using DAL.Repositores.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BEPeer.Controllers
{
    [Route("rest/v1/funding")]
    [ApiController]
    public class FundingController : Controller
    {
        private readonly IFundingServices _fundingService;

        public FundingController(IFundingServices fundingService)
        {
            _fundingService = fundingService;
        }

        [HttpGet("{lenderId}")]
        public async Task<IActionResult> GetFundingsByLenderId(string lenderId)
        {
            try
            {
                var response = await _fundingService.GetFundingsByLenderId(lenderId);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "Success retrieve list funding!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<List<ResLoginDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> FundingLoan(ReqFundingLoanDto reqFunding)
        {
            try
            {
                var response = await _fundingService.FundingLoan(reqFunding);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "loan funded succesfully!"
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Insufficient lender balance")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<List<ResLoginDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}