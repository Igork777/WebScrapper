using System;
using System.Net.Security;

namespace WebScrapper.JWT
{
    public interface IJwtAuthenticationManager
    {
        string Authenticate(String username, String password);
    }
}