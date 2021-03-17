using WebScrapper.Scraping.DTO;

namespace WebScrapper.Services
{
    public interface ILogInService
    {
        bool checkIfNameAndPasswordsCorrespond(User user)
        {
            return true;
        }
    }
}