using Repositories.Entities;
using Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Repositories.Repositories
{
    public class UserRepository: IRepository<User>
    {
        private readonly IContext ctx;
        public UserRepository(IContext context)
        {
            ctx = context;
        }

        public async Task<User> AddItem(User item)
        {
            await ctx.Users.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            User user = ctx.Users.FirstOrDefault(x => x.UserId == id);
            ctx.Users.Remove(user);         
            await ctx.Save();

        }

        public async Task<List<User>> GetAll()
        {
            return await ctx.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await ctx.Users.FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<User> UpdateItem(int id, User item)
        {
            var u = await ctx.Users.FirstOrDefaultAsync(x => x.UserId == id);
            u.Username = item.Username;
            u.UserId = item.UserId;
            u.Email = item.Email;
            u.PasswordHash= item.PasswordHash;
            await ctx.Save();
            return u;
        }
    }
}
