using AutoMapper;
using Repositories.Entities;
using Repositories.Interfaces;
using Repositories.Repositories;
using Service.Dto;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class UserLoginService(IRepository<User> repository, IToken<User> tokenService) : ILogin<UserLoginDto>
    {
        private readonly IRepository<User> _repository = repository;
        private readonly IToken<User> _tokenService= tokenService; // הזרקת שירות הטוקן


        public async Task<string> Login(UserLoginDto item)
        {
            var users = await _repository.GetAll();

            // מחפשים את המשתמש
            var user = users.FirstOrDefault(u =>
                u.Email == item.Email &&
                u.PasswordHash == item.Password); // הערה: בהמשך כדאי לעבור ל-Hash אמיתי

            if (user == null)
                throw new Exception("Invalid email or password");

            // מחזירים את הטוקן במקום את הטקסט "Login successful"
            return _tokenService.CreateToken(user);
        }
    }
}
