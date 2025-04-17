using Microsoft.AspNetCore.Mvc;

namespace IsTakipSistemiMVC.Models
{
    public class AIRegisterCommand
    {
        public string action { get; set; }
        public string adSoyad { get; set; }
        public string kullaniciAdi { get; set; }
        public string password { get; set; }
        public int birimId { get; set; }
        public string phoneNumber { get; set; }
    }
}
