using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace WindowPrincipal
{
    class Program
    {
        //
        static void Main(string[] args)
        {
            WindowsIdentity identity = ShowIdentityInformation();
            //WindowsPrincipal principal;

            Console.ReadKey();
        }

        public static WindowsIdentity ShowIdentityInformation()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (identity == null)
            {
                WriteLine("not a Windows Identity");
                return null;
            }

            WriteLine($"IdentityType: {identity}");
            WriteLine($"Name: {identity.Name}");
            WriteLine($"Authenticated: {identity.IsAuthenticated}");
            WriteLine($"Authentication Type:{identity.AuthenticationType}");
            WriteLine($"Anonymous? {identity.IsAnonymous}");
            WriteLine($"Access Token: {identity.AccessToken.DangerousGetHandle()}");
            WriteLine();

            return identity;
        }

    }
}
