using AutoMapper;
using Repositories.Entities;
using Repositories.Interfaces;
using Service.Dto; 
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services
{
    public class UserService(IRepository<User> repository, IMapper mapper) : IService<UserDto>
    {
        private readonly IRepository<User> _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<UserDto> AddItem(UserDto item)
        {
            var userEntity = _mapper.Map<User>(item);
            if (string.IsNullOrEmpty(userEntity.PasswordHash))
            {
                userEntity.PasswordHash = "default_hash_until_implemented";
            }
            var addedUser = await _repository.AddItem(userEntity);
            return _mapper.Map<UserDto>(addedUser);
        }

        public async Task DeleteItem(int id)
        {
            await _repository.DeleteItem(id);
        }

        public async Task<List<UserDto>> GetAll()
        {
            var users = await _repository.GetAll();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetById(int id)
        {
            var user = await _repository.GetById(id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateItem(int id, UserDto item)
        {
            var userEntity = _mapper.Map<User>(item);
            var updatedUser = await _repository.UpdateItem(id, userEntity);
            return _mapper.Map<UserDto>(updatedUser);
        }
    }
}