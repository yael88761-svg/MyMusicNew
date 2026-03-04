using Microsoft.Extensions.Configuration;
using Repositories.Entities;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class TokenService(IConfiguration config) : IToken<User>
    {
        private readonly IConfiguration _config = config;

        public string CreateToken(User item)
        {
            throw new NotImplementedException();
        }
    }
}
