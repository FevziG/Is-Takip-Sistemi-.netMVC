using Microsoft.AspNetCore.Mvc;

namespace IsTakipSistemiMVC.Models
{
    public class AIIsAtaCommand 
    {
       public string action {  get; set; }
       public string isBaslik {  get; set; }
       public string isAciklama {  get; set; }
       public string kullaniciAdi {  get; set; }

    }
}
