using System.Diagnostics;
using IsTakipSistemiMVC.Models;
using IsTakipSistemiMVC.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IsTakipSistemiMVC.Controllers
{
    public class HomeController : Controller
    {
       private readonly IsTakipDbContext _context;
        //private readonly OpenAIService _openAIService; , OpenAIService openAIService _openAIService = openAIService;

        public HomeController(IsTakipDbContext context)
        {
            _context = context;
            
        }



        //YAPILACAKLAR: REGÝSTER SAYFASINI ADMÝN PANELÝN ÝLK SAYFASINA KOY. TELEFON NUMARASI EKLE (yapýldý) 
        //AYRICA ADMÝNDE ÇALIÞANLARIN BÝLGÝLERÝNÝ GÖRÜNTÜLEME VE GÜNCELLEME SAYFASI YAP.(yapýldý)
        //WORKERDA BÝLGÝLERÝ GÖRÜNTÜLEME SAYFASI YAP.





        //LOGÝN-------------------------------------------------------
        public IActionResult LoginPage()
        {


            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Personellers
                .FirstOrDefault(u => u.PersonelKullaniciAd == username && u.PersonelParola == password);

            if (user != null)
            {
                //SessionExtensions burada kullanmak istiyorum nasýl yapýlýr 
                HttpContext.Session.SetString("username", user.PersonelKullaniciAd);
                HttpContext.Session.SetInt32("personelId", user.PersonelId);
                HttpContext.Session.SetInt32("personelBirimId", (int)user.PersonelBirimId);
                HttpContext.Session.SetInt32("personelYetkiTurId", (int)user.PersonelYetkiTurId);


                switch (user.PersonelYetkiTurId)
                {
                    case 1:
                        return RedirectToAction("AdminPage", "Home");

                    case 2:
                        return RedirectToAction("WorkerPage", "Home");
                    default:
                        return View("LoginPage");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Geçersiz kullanýcý adý veya þifre";
                return View("LoginPage");
            }
        }


        

        //LOGOUT----------------------------------------------------------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            
            return RedirectToAction("LoginPage","Home");
        }




        //-----------------------------------------------------------------------------------











        //WORKER PAGE------------------------------------------------------
        [ServiceFilter(typeof(ActionWorkerSessionHelper))]
        public IActionResult WorkerPage()
        {
            int? personelId = HttpContext.Session.GetInt32("personelId");
            string uname = HttpContext.Session.GetString("username");
            int? uid = HttpContext.Session.GetInt32("personelBirimId");
            var birim = _context.Birimlers.FirstOrDefault(b => b.BirimId == uid);

            
                ViewBag.WelcomeMessage = $"Welcome {uname}!";
                //ViewBag.BirimAd = $"{birim.BirimAd} için çalýþanýsýnýz.";
                var yeniIs = _context.Islers.Where(b => b.IsPersonelId == personelId && b.IsOkunma == false).ToList();

                return View(yeniIs);
            
            
        }
        [ServiceFilter(typeof(ActionWorkerSessionHelper))]
        public IActionResult ProfilePage() 
        {
            int? personelId = HttpContext.Session.GetInt32("personelId");
            int? personelBirimId = HttpContext.Session.GetInt32("personelBirimId");
            var personel = _context.Personellers.FirstOrDefault(b => b.PersonelId == personelId);
            var birim = _context.Birimlers.FirstOrDefault(b => b.BirimId == personelBirimId);
            var isler = _context.Islers.Where(b => b.IsPersonelId == personelId && b.IsDurumId==2)
                .Join(_context.Durumlars,
                isler => isler.IsDurumId,
                durum => durum.DurumId,
                (isler,durum)=>new ProfileDetayViewModel 
                {
                    IsId = isler.IsId,
                    IsBaslik = isler.IsBaslik,
                    IsAciklama = isler.IsAciklama,
                    IsYorum = isler.IsYorum,
                    IletilenTarih = isler.IletilenTarih,
                    YapilanTarih = isler.YapilanTarih,
                    DurumAd = durum.DurumAd
                })
                .ToList();
            var viewModel = new ProfileViewModel
            {
                Personel = personel,
                Isler = isler,
                Birimler = birim
            };                     

            return View(viewModel);
        }

        public IActionResult WorkerYeniIs(int IsId) 
        {
            var isler = _context.Islers.FirstOrDefault(b => b.IsId == IsId);
            isler.IsOkunma = true;
            _context.SaveChanges();

            return RedirectToAction("WorkerPage", "Home");
        }



        [ServiceFilter(typeof(ActionWorkerSessionHelper))]
        public IActionResult IsListeleWorker() 
        {
            int? personelId = HttpContext.Session.GetInt32("personelId");

            var islerListesi = _context.Islers.Where(b => b.IsPersonelId == personelId)
                .Join(_context.Durumlars,
                isler =>isler.IsDurumId,
                durum =>durum.DurumId,
                (isler,durum)=> new IsDurumViewModel 
                {
                    IsId = isler.IsId,
                    IsBaslik = isler.IsBaslik,
                    IsAciklama = isler.IsAciklama,
                    IsYorum = isler.IsYorum,
                    IsDurumId = isler.IsDurumId,
                    IletilenTarih = isler.IletilenTarih,
                    YapilanTarih = isler.YapilanTarih,
                    DurumAd = durum.DurumAd
                }).ToList();
            

            return View(islerListesi);
        }




        [ServiceFilter(typeof(ActionWorkerSessionHelper))]
        public IActionResult isTamamlaPage() 
        {
            int? personelId = HttpContext.Session.GetInt32("personelId");
            var isler = _context.Islers.Where(b => b.IsPersonelId == personelId && b.IsDurumId ==1).ToList();

            var errorMessage = TempData["ErrorMessage"] as string;
            ViewBag.errorMessage = errorMessage;
            return View(isler);
        }

        [HttpPost]
        public IActionResult isTamamla(int IsId,string isYorum) 
        {
            if (IsId != null && isYorum !=null) {
                var Is = _context.Islers.FirstOrDefault(b => b.IsId == IsId);
               
                    Is.IsYorum = isYorum;
                    Is.IsDurumId = 2;
                    Is.YapilanTarih = DateTime.Now;
                    _context.SaveChanges();
            }
                else
                {
                    TempData["ErrorMessage"] = "Please, comment about job.";
                    return RedirectToAction("isTamamlaPage", "Home");
                }

            return RedirectToAction("isTamamlaPage", "Home");
        }






        //ADMÝN PAGE---------------------------------------------------------
        [ServiceFilter(typeof(ActionSessionHelper))]
        public IActionResult AdminPage()
        {
            string uname = HttpContext.Session.GetString("username");
            int? uid = HttpContext.Session.GetInt32("personelBirimId");

            int personelYetkiTurId = (int)HttpContext.Session.GetInt32("personelYetkiTurId");

            var birim = _context.Birimlers.FirstOrDefault(b => b.BirimId == uid);

            var personeller = _context.Personellers.Where(b => b.PersonelBirimId == uid && b.PersonelYetkiTurId !=1).ToList();
    
            ViewBag.WelcomeMessage = $"Welcome, {uname}!";
            ViewBag.BirimAd = birim.BirimAd;

            return View(personeller);
        }

        public IActionResult WorkerEditPage(int personelId) 
        {
            var personel = _context.Personellers.FirstOrDefault(b => b.PersonelId == personelId);
            Console.WriteLine("perosnel ýd::" + personel.PersonelId);
            if (ViewBag.ErrorMessage != null) 
            {
                ViewData["ErrorMessage"] = ViewBag.ErrorMessage;
            
            }
            return View(personel);
        }

        public IActionResult WorkerEdit(int personelId, string adSoyad, string username, string password, int personelyetkiId, string phoneNumber)
        {
            var personel = _context.Personellers.FirstOrDefault(u => u.PersonelId == personelId);
            int? uid = HttpContext.Session.GetInt32("personelBirimId");
            Console.WriteLine("perosnel wwww ýd::" + personelId);
            if (string.IsNullOrWhiteSpace(adSoyad) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phoneNumber) ||
                personelyetkiId < 1 || personelyetkiId > 2)
            {
                ViewBag.ErrorMessage = "Hatalý düzeleme.";
                return View("WorkerEditPage", personel);
            }

            var existingUser = _context.Personellers.FirstOrDefault(u => u.PersonelKullaniciAd == username && u.PersonelId != personelId);

            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Bu kullanýcý adý mevcut";
                return View("WorkerEditPage", personel);
            }

            else
            {
                personel.PersonelAdSoyad = adSoyad;
                personel.PersonelKullaniciAd = username;
                personel.PersonelParola = password;
                personel.PersonelBirimId = uid;
                personel.PersonelYetkiTurId = personelyetkiId;
                personel.PersonelTelefonNo = phoneNumber;

                _context.SaveChanges();
                Console.WriteLine("Baþarýyla güncellendi.");
                 return RedirectToAction("AdminPage", "Home");
        }
        }

        public IActionResult WorkerDelete(int personelId) 
        {
            var personel = _context.Personellers.FirstOrDefault(b => b.PersonelId == personelId);
            var isler = _context.Islers.Where(b => b.IsPersonelId == personelId);
            _context.Islers.RemoveRange(isler);
            _context.Personellers.Remove(personel);
            _context.SaveChanges();
            return RedirectToAction("AdminPage", "Home");
        }

        //YÖNETÝCÝ ÝÞ ATAMA-----------------------------------------------------
        [ServiceFilter(typeof(ActionSessionHelper))]
        public IActionResult isAtaPage()
        {
            int? uid = HttpContext.Session.GetInt32("personelBirimId");
            var birim = _context.Birimlers.FirstOrDefault(b => b.BirimId == uid);
            ViewBag.BirimAd = birim.BirimAd;


            var calýsanlar = _context.Personellers.Where(b => b.PersonelYetkiTurId == 2 && b.PersonelBirimId == uid).ToList();
            ViewBag.çalýþanlar = calýsanlar;


            return View();
        }



        [HttpPost]
        public IActionResult isAta(string isBaslik, string isAciklama, int selectPerId)
        {
            if (string.IsNullOrWhiteSpace(isBaslik) ||
                string.IsNullOrWhiteSpace(isAciklama))
            {     
                ViewBag.ErrorMessage = "Hatalý Giriþ";
                return RedirectToAction("isAtaPage", "Home");
            }

            var yeniIs = new Isler
            {
                IsBaslik = isBaslik,
                IsAciklama = isAciklama,
                IsPersonelId = selectPerId,
                IletilenTarih = DateTime.Now,
                IsDurumId = 1,
                IsOkunma = false
            };


            _context.Islers.Add(yeniIs);
            _context.SaveChanges();


            return RedirectToAction("isAtaPage", "Home");
        }

        //YÖNETÝCÝ ÝÞ TAKÝP----------------------------------
        [ServiceFilter(typeof(ActionSessionHelper))]
        public IActionResult isTakipPage() 
        {
            ViewBag.Message = "iþtakip page sayfasý";

            int? uid = HttpContext.Session.GetInt32("personelBirimId");

            var birim = _context.Birimlers.FirstOrDefault(b => b.BirimId == uid);
            ViewBag.BirimAd = birim.BirimAd;


            var calýsanlar = _context.Personellers.Where(b => b.PersonelYetkiTurId == 2 && b.PersonelBirimId == uid).ToList();
            ViewBag.çalýþanlar = calýsanlar;
            
            return View();
        }


        [HttpPost]
        public IActionResult isTakip(int selectPerId) 
        {
            //var personel = _context.Personellers.FirstOrDefault(b => b.PersonelId == selectPerId);
            //ViewBag.personel = personel;
            //var isler = _context.Islers.Where(b => b.IsPersonelId == selectPerId).ToList();
            //ViewBag.isler = isler;
            var personel = _context.Personellers.FirstOrDefault(b => b.PersonelId == selectPerId);
            TempData["secilen"] = JsonConvert.SerializeObject(personel);
            return RedirectToAction("isListele", "Home");
        
        }

        [HttpGet]
        [ServiceFilter(typeof(ActionSessionHelper))]
        public IActionResult isListele() 
        {
            var personelJson = TempData["secilen"] as string;
            var personel = personelJson != null ? JsonConvert.DeserializeObject<Personeller>(personelJson) : null;
            if (TempData["secilen"] == null) 
            {
                return RedirectToAction("isTakipPage", "Home");
            }
            var islerListesi = _context.Islers.Where(b => b.IsPersonelId == personel.PersonelId)
                .Join(_context.Durumlars,
                isler=> isler.IsDurumId,
                durum => durum.DurumId,
                (isler,durum)=> new IsDetayViewModel 
                {
                    IsId = isler.IsId,
                    IsBaslik = isler.IsBaslik,
                    IsAciklama = isler.IsAciklama,
                    IsYorum = isler.IsYorum,
                    IsDurumId = isler.IsDurumId,
                    IletilenTarih = isler.IletilenTarih,
                    YapilanTarih = isler.YapilanTarih,
                    DurumAd = durum.DurumAd
                })
                .ToList();

            var viewModel = new IsTakipViewModel
            {
                Personel = personel,
                Isler = islerListesi
            };
            return View( viewModel);
        }



        //YÖNETTÝCÝ REGÝSTER ÝÞLEMÝ------------------------------------------------------
        [ServiceFilter(typeof(ActionSessionHelper))]
        public IActionResult RegisterPage()
        {
           
                return View();
           
        }


        [HttpPost]
        public IActionResult Register(string adSoyad, string username, string password, int birimId, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(adSoyad) ||
               string.IsNullOrWhiteSpace(username) ||
               string.IsNullOrWhiteSpace(password) ||
               string.IsNullOrWhiteSpace(phoneNumber) ||
               birimId < 0 || 
               birimId > 2)
            {
                ViewBag.ErrorMessage = "Hatalý Giriþ";
                return View("RegisterPage");
            }

            var existingUser = _context.Personellers.FirstOrDefault(u => u.PersonelKullaniciAd == username);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Bu kullanýcý adý mevcut";
                return View("RegisterPage");
            }

            var newPersonel = new Personeller
            {
                PersonelAdSoyad = adSoyad,
                PersonelKullaniciAd = username,
                PersonelParola = password,
                PersonelBirimId = birimId,
                PersonelYetkiTurId = 2,
                PersonelTelefonNo = phoneNumber
            };

            _context.Personellers.Add(newPersonel);
            _context.SaveChanges();

            return RedirectToAction("AdminPage", "Home");
        }
        //__________________________________________________________________________

        public IActionResult AIPage()
        {
           int ?yetkiId = HttpContext.Session.GetInt32("personelYetkiTurId");
           ViewBag.AIResponse = TempData["AIResponse"];
            if (TempData["RegisterAIError"] != null)
            {
                ViewBag.RegisterAIError = TempData["RegisterAIError"];
            }
            if (TempData["RegisterAISuccess"] != null) 
            {
                ViewBag.RegisterAISuccess = TempData["RegisterAISuccess"];
            }
           return View(yetkiId);

        }


       

        //----------------------------------------------------------------------------

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
