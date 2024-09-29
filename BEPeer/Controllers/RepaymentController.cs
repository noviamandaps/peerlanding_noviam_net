using DAL.DTO.Res;
using DAL.DTO.Req;
using DAL.Repositores.Services;
using DAL.Repositores.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace BEPeer.Controllers
{
    [Route("rest/v1/repayment/[action]")]
    [ApiController]
    public class RepaymentController : Controller
    {
        private readonly IRepaymentServices _repaymentService;

        public RepaymentController(IRepaymentServices repaymentService)
        {
            _repaymentService = repaymentService;
        }

        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetRepaymentByLoanId(string loanId)
        {
            try
            {
                var response = await _repaymentService.GetRepaymentByLoanId(loanId);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "Success Get Repayment"
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
        public async Task<IActionResult> GetMonthlyPaymentByLoanId(string loanId)
        {
            try
            {
                var response = await _repaymentService.GetMonthlyPaymentByLoanId(loanId);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "Success Get Monthly Payment"
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

        // PayMultipleMonthlyPayment
        [HttpPost]
        public async Task<IActionResult> PayMultipleMonthlyPayment(ReqPayMultipleMonthlyPaymentDto reqPayMultipleMonthlyPaymentDto)
        {
            try
            {
                var response = await _repaymentService.PayMultipleMonthlyPayment(reqPayMultipleMonthlyPaymentDto);
                return Ok(new ResBaseDto<string>
                {
                    Data = response,
                    Success = true,
                    Message = "Success Pay Multiple Monthly Payment"
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