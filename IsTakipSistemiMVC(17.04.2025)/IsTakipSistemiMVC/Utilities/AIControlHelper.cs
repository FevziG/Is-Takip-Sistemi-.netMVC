using IsTakipSistemiMVC.Models;
using System.Text.Json;

namespace IsTakipSistemiMVC.Utilities
{
    public static class AIControlHelper
    {
        public static bool Control(string username,string action,IsTakipDbContext context)
        {
            try
            {
                var user = context.Personellers.FirstOrDefault(u => u.PersonelKullaniciAd == username);
                bool actionControl = new[] {"register","logout","isAtaAI","IsListeleAI",
                    "IsListeleTarihAI","WorkerEditPageAI","WorkerProfilePageAI"}.Contains(action, StringComparer.OrdinalIgnoreCase);
                if (user != null && actionControl)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
           
        }
    }
}