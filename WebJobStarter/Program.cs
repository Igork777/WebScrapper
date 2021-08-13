using System;
using Microsoft.EntityFrameworkCore;
using WebJobStarter.DbContext;

namespace WebJobStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            Starter starter = new Starter(new DBContext(new DbContextOptions<DBContext>()));
            starter.Start();
        }
    }
}
