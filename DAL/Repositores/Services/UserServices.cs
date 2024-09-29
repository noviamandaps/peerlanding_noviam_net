using BCrypt.Net;
using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Model;
using DAL.Repositores.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositores.Services
{

    public class UserServices : IUserServices
    {
        private readonly PeerlandingContext _context;
        private readonly IConfiguration _configuration;
        public UserServices(PeerlandingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }
        public async Task<string> Register(ReqRegisterUserDto register)
        {
            var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == register.Email);
            if (isAnyEmail != null)
            {
                throw new Exception("email already used");
            }
            var newUser = new MstUser
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Role = register.Role,
                Balance = (decimal)register.Balance,
            };


            await _context.MstUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser.Name;


        }
        public async Task<List<ResUserDto>> GetAllUsers()
        {
            return await _context.MstUsers
                .Where(user => user.Role.ToLower() != "admin") // Filter out admin users
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance,
                })
                .ToListAsync();
        }

        public async Task<List<ResUserDto>> GetAllNonAdminUsers()
        {
            var users = await _context.MstUsers
                .Where(user => user.Role != "admin") // Asumsikan bahwa ada properti Role
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance,
                })
                .ToListAsync();

            return users;
        }

        public async Task<bool> DeleteUser(string id)
        {
            var user = await _context.MstUsers.FindAsync(id);
            if (user == null)
                return false;

            _context.MstUsers.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<ResUserDto> UpdateUser(string id, ReqUpdateUserDto updateDto)
        {
            var user = await _context.MstUsers.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Update user properties
            user.Name = updateDto.Name ?? user.Name;
            user.Role = updateDto.Role ?? user.Role;

            if (updateDto.Balance.HasValue)
                user.Balance = updateDto.Balance.Value;

            await _context.SaveChangesAsync();

            return new ResUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Balance = user.Balance
            };
        }
        //public async Task<ResLoginDto> Login(ReqLoginDto reqLogin)
        //{
        //    var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == reqLogin.Email);
        //    if (user == null)
        //    {
        //        throw new Exception("Invalid email or password");
        //    }
        //    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, user.Password);
        //    if (!isPasswordValid)
        //    {
        //        throw new Exception("Invalid email or password");
        //    }
        //    var token = GenerateJwtToken(user);
        //    var loginResponse = new ResLoginDto
        //    {
        //        Token = token,

        //        Role = user.Role
        //    };
        //    return loginResponse;
        //}

        public async Task<ResLoginDto> Login(ReqLoginDto reqLogin)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == reqLogin.Email);
            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password");
            }

            var token = GenerateJwtToken(user);
            var loginResponse = new ResLoginDto
            {
                UserId = user.Id,
                Token = token,
                Role = user.Role
            };

            return loginResponse;

        }

        public string GenerateJwtToken(MstUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];


            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };

            var token = new JwtSecurityToken(
              issuer: jwtSettings["ValidIssuer"],
              audience: jwtSettings["ValidAudience"],
              claims: claims,
              expires: DateTime.Now.AddHours(2),
              signingCredentials: credentials
             );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> AddUser(ReqRegisterUserDto addUser)
        {
            var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == addUser.Email);
            if (isAnyEmail != null)
            {
                throw new Exception("Email already used");
            }

            var newUser = new MstUser
            {
                Name = addUser.Name,
                Email = addUser.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(addUser.Password),
                Role = addUser.Role,
                Balance = (decimal)addUser.Balance
            };

            await _context.MstUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser.Name;
        }

        public async Task<ResGetUserById> GetUserById(string id)
        {
            var user = await _context.MstUsers
                .Where(u => u.Id == id)
                .Select(user => new ResGetUserById
                {
                    Id = user.Id,
                    Name = user.Name,
                    Role = user.Role,
                    Balance = user.Balance,
                })
                .FirstOrDefaultAsync();

            return user;
        }

        public Task UpdateSaldo(string id, ReqEditSaldoDto dto)
        {
            throw new NotImplementedException();
        }

        //public Task<ResUserDto> UpdateUser(string id, ReqUpdateUserDto updateDto)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<ResUserDto>> GetAllUsers()
        //{
        //    throw new NotImplementedException();
        //}



        //async Task<string> IUserServices.Update(string userId, ReqUpdateUserDto reqUpdate)
        //{
        //    var existingUser = await _context.MstUsers.SingleOrDefaultAsync(user => user.Id == userId);

        //    if (existingUser == null)
        //    {
        //        throw new Exception("User not found");
        //    }

        //    existingUser.Name = reqUpdate.Name ?? existingUser.Name;
        //    existingUser.Role = reqUpdate.Role ?? existingUser.Role;
        //    existingUser.Balance = reqUpdate.Balance ?? existingUser.Balance;

        //    _context.MstUsers.Update(existingUser);
        //    await _context.SaveChangesAsync();

        //    return reqUpdate.Name;
        //}
        //public Task<object?> GetUserById(object id)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<ResEditSaldo> EditSaldo(ReqEditSaldo editSaldo)
        //{
        //    var user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Id == id);
        //    if (user == null)
        //    {
        //        throw new Exception("User not found");
        //    }

        //    user.Balance = editSaldo.Balance;

        //    _context.MstUsers.Update(user);
        //    await _context.SaveChangesAsync();

        //    return new ResEditSaldoD
        //    {
        //        Balance = user.Balance ?? 0,
        //    };
        //}
    }
}

