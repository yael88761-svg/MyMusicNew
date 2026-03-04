using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;

namespace MyMusicNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(
          IRegister<UserRegisterDto> registerService
        , ILogin<UserLoginDto> loginService
        , IService<UserDto> service)
        : ControllerBase
    {
        private readonly IRegister<UserRegisterDto> _registerService = registerService;
        private readonly ILogin<UserLoginDto> _loginService = loginService;
        private readonly IService<UserDto> _service = service;


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            try
            {
                var result = await _registerService.Register(userRegisterDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            try
            {
                var result = await _loginService.Login(userLoginDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAll();
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _service.GetById(id);
            if (user == null)
                return NotFound();
            return Ok(user);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteItem(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddItem(UserDto item)
        {
            try
            {
                var addedUser = await _service.AddItem(item);
                return Ok(addedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }
    }
}
