using AutoMapper;
using Repositories.Entities;
using Repositories.Interfaces;
using Service.Dto;
using Service.Interfaces;

public class UserRegisterService : IRegister<UserRegisterDto>
{
    private readonly IRepository<User> _repository;
    private readonly IMapper _mapper;
    private readonly IToken<User> _tokenService;

    public UserRegisterService(IRepository<User> repository, IMapper mapper, IToken<User> tokenService)
    {
        _repository = repository;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<string> Register(UserRegisterDto item)
    {
        var users = await _repository.GetAll();
        if (users.Any(u => u.Email == item.Email))
            throw new Exception("User already exists");

        var newUser = _mapper.Map<User>(item);
        newUser.Username = item.Name;
        newUser.PasswordHash = item.Password;

        var addedUser = await _repository.AddItem(newUser);

        return _tokenService.CreateToken(addedUser);
    }
}