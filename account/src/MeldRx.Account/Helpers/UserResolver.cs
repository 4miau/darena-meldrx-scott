namespace MeldRx.Account.Helpers
{
    public class UserResolver<TUser> where TUser : class
    {
        private readonly UserManager<TUser> _userManager;
        private readonly LoginResolutionPolicy _policy;

        public UserResolver(UserManager<TUser> userManager, RegisterConfiguration configuration)
        {
            _userManager = userManager;
            _policy = configuration.ResolutionPolicy;
        }

        public async Task<TUser> GetUserAsync(string login)
        {
            try
            {
                switch (_policy)
                {
                    case LoginResolutionPolicy.Username:
                        return await _userManager.FindByNameAsync(login);
                    case LoginResolutionPolicy.Email:
                        return await _userManager.FindByEmailAsync(login);
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}








