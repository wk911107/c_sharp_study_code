using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace WindowPrincipal
{
    class Program
    {
        static void Main(string[] args)
        {
            WindowsIdentity identity = ShowIdentityInformation();
            WindowsPrincipal principal = ShowPrincipal(identity);
            ShowClaims(principal.Claims);

            Console.ReadKey();
        }

        public static WindowsIdentity ShowIdentityInformation()
        {
            //获取当前用户的身份信息
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


        public static WindowsPrincipal ShowPrincipal(WindowsIdentity identity)
        {
            WriteLine("Show principal information.");

            WindowsPrincipal principal = new WindowsPrincipal(identity);

            if (principal == null)
            {
                WriteLine("not a Windows Principal");
                return null;
            }

            WriteLine($"Users? {principal.IsInRole(WindowsBuiltInRole.User)}");
            WriteLine($"Administrators? {principal.IsInRole(WindowsBuiltInRole.Administrator)}");
            WriteLine();

            return principal;
        }

        /// <summary>
        /// 展示声明的信息
        /// </summary>
        /// <param name="claims"></param>
        public static void ShowClaims(IEnumerable<Claim> claims)
        {
            WriteLine("Claims...");
            foreach (var v in claims)
            {
                //声明的主题
                WriteLine($"Subject:{v.Subject}");
                //声明的颁发者
                WriteLine($"Issuer:{v.Issuer}");
                //声明的类型
                WriteLine($"Type:{v.Type}");
                //声明的值类型
                WriteLine($"Value type:{v.ValueType}");
                //声明的值
                WriteLine($"Value:{v.Value}");

                foreach (var prop in v.Properties)
                {
                    WriteLine($"\tProperty: {prop.Key} {prop.Value}");
                }
                WriteLine();
            }
        }

    }
}
