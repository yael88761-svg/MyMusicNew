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
    public class UserRegisterService(IRepository<User> repository, IMapper mapper) : IRegister<UserRegisterDto>
    {
        private readonly IRepository<User> _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<string> Register(UserRegisterDto item)
        {
            var users = await _repository.GetAll();
            var existingUser = users.FirstOrDefault(u => u.Email == item.Email);

            if (existingUser != null)
                throw new Exception("User already exists");

            var newUser = _mapper.Map<User>(item);

            await _repository.AddItem(newUser);

            return "Register successful";
        }
    }
}
