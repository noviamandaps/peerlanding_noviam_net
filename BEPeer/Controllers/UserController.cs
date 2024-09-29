using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositores.Services;
using DAL.Repositores.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userservices;
        public UserController(IUserServices userServices)
        {
            _userservices = userServices;
        }

        [HttpPost]
        public async Task<IActionResult> Register(ReqRegisterUserDto register)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();
                    var errorMessage = new StringBuilder("Validation error occurred!");
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = error
                    });
                }

                var res = await _userservices.Register(register);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User registered",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "email already used")
                {
                    return BadRequest(new ResBaseDto<object>
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
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userservices.GetAllUsers();
                return Ok(new ResBaseDto<List<ResUserDto>>
                {
                    Success = true,
                    Message = "List of users",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<List<ResUserDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllNonAdminUsers()
        {
            try
            {
                var users = await _userservices.GetAllNonAdminUsers();
                return Ok(new ResBaseDto<List<ResUserDto>>
                {
                    Success = true,
                    Message = "List of non-admin users retrieved successfully",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<List<ResUserDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Login(ReqLoginDto loginDto)
        {
            try
            {
                var response = await _userservices.Login(loginDto);
                return Ok(new ResBaseDto<ResLoginDto>
                {
                    Success = true,
                    Message = "User Login Success",
                    Data = new ResLoginDto
                    {
                        Token = response.Token,
                        UserId = response.UserId,
                        Role = response.Role
                    }
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Invalid email or password")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<object>
                {
                    Success = false,
                    Message = "An error occurred during login",
                    Data = null
                });
            }
        }



        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddUser(ReqRegisterUserDto register)
        {
            var currentUser = HttpContext.User;
            if (!currentUser.IsInRole("admin"))
            {
                return Forbid("You do not have permission to add users.");
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Message = x.Value.Errors.Select(equals => equals.ErrorMessage).ToList()
                        }).ToList();

                    var errorMessage = new StringBuilder("Validation errors occured!");

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }

                var res = await _userservices.Register(register);

                return Ok(new ResBaseDto<String>
                {
                    Success = true,
                    Message = "user registered!",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Email already used")
                {
                    return BadRequest(new ResBaseDto<object>
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _userservices.DeleteUser(id);
                if (result)
                    return Ok(new ResBaseDto<object>
                    {
                        Success = true,
                        Message = "User deleted successfully",
                        Data = null
                    });
                else
                    return NotFound(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Data = null
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

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,lender")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] ReqUpdateUserDto updateDto)
        {
            try
            {
                var updatedUser = await _userservices.UpdateUser(id, updateDto);
                return Ok(new ResBaseDto<ResUserDto>
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = updatedUser
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResBaseDto<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the user",
                    Data = null
                });
            }
        }

        [HttpGet]
       
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userservices.GetUserById(id);
                if (user == null)
                {
                    return NotFound(new ResBaseDto<ResGetUserById>
                    {
                        Success = false,
                        Message = "User not found",
                        Data = null
                    });
                }
                return Ok(new ResBaseDto<ResGetUserById>
                {
                    Success = true,
                    Message = "User found",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<ResGetUserById>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

    [Authorize]
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateSaldo(string id, ReqEditSaldoDto dto)
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
                    }).ToList();
                var errorMessage = new StringBuilder("Validation error occured!");
                return BadRequest(new ResBaseDto<object>
                {
                    Success = false,
                    Message = errorMessage.ToString(),
                    Data = errors
                });
            };

                    await _userservices.GetUserById(id);
                    //await _userservices.UpdateSaldo(id, dto);

            return Ok(new ResBaseDto<object>
            {
                Success = true,
                Message = "User updated successfully",
                Data = User
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
