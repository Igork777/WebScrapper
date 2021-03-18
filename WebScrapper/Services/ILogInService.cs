using WebScrapper.Scraping.DTO;

namespace WebScrapper.Services
{
    public interface ILogInService
    {
        bool CheckIfNameAndPasswordsCorrespond(User user);
    }
}