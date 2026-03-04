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

        // שמירת המשתמש וקבלת האובייקט עם ה-ID שנוצר
        var addedUser = await _repository.AddItem(newUser);

        // מחזירים טוקן עבור המשתמש החדש
        return _tokenService.CreateToken(addedUser);
    }
}