using Microsoft.AspNetCore.Mvc;

namespace IsTakipSistemiMVC.Models
{
    public class AIIsListeleTarihCommand
    {
      public string action {get;set;}
      public string kullaniciAdi { get;set;}
      public DateTime? tarih {  get;set;}
    }
}
