using Hospital.Models;

namespace Hospital.Repositories
{
    public class AccountRepo
    {
        private readonly Context _context;
        public AccountRepo(Context context)
        {
            _context = context;
        }

        public Admin GetAdmin(string username) 
        {
            return _context.Admins.FirstOrDefault(a => a.Username == username);
        }
    }
}
