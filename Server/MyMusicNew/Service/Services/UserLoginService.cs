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
    public class UserLoginService(IRepository<User> repository, IMapper mapper) : ILogin<UserLoginDto>
    {
        private readonly IRepository<User> _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<string> Login(UserLoginDto item)
        {
            var users = await _repository.GetAll();

            var user = users.FirstOrDefault(u =>
                u.Email == item.Email &&
                u.PasswordHash == item.Password);

            if (user == null)
                throw new Exception("Invalid email or password");

            return "Login successful";
        }
    }
}
