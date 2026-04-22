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

        // 1. מיפוי ראשוני (יעביר רק את ה-Email כי השאר לא תואם בשמות)
        var newUser = _mapper.Map<User>(item);

        // 2. השלמה ידנית של השדות שלא עברו בגלל שמות שונים:

        // מעביר את Name ל-Username
        newUser.Username = item.Name;

        // מעביר את Password ל-PasswordHash
        newUser.PasswordHash = item.Password;

        // 3. שמירה למסד הנתונים
        var addedUser = await _repository.AddItem(newUser);

        // 4. החזרת טוקן
        return _tokenService.CreateToken(addedUser);
    }
}