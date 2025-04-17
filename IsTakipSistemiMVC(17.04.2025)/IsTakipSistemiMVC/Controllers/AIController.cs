using System.Text;
using System.Text.Json;
using IsTakipSistemiMVC.Models;
using IsTakipSistemiMVC.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class AIController : Controller
    {
        private readonly IsTakipDbContext _context;
        private readonly HttpClient _client;
        private readonly string _apiKey;
        private readonly string _apiUri;
        private static List<object> chatHistory = new List<object>();

        public AIController(IsTakipDbContext context)
        {
            var config = new ConfigurationBuilder().AddJsonFile(AppDomain.CurrentDomain.BaseDirectory
                + "appsettings.json").Build();
            _apiKey = config["GeminiAI:apiKey"];
            _apiUri = config["GeminiAI:apiUri"];
            _client = new HttpClient();
            _context = context;
        }




        int sayi = 0;
        [HttpPost]
        public async Task<IActionResult> GetGeminiResponse(string prompt)
        {
            string systemMessage = @"Sen bir iş takip sistemi için doğal dil komutlarını JSON'a çeviren bir asistansın.

                                     Kullanıcı aşağıdaki işlemleri yazabilir, sen sadece belirtilen JSON formatında döneceksin.:
                                    
                                    *Kullanıcı eklemek için:
                                    {
                                      ""action"": ""register"",
                                      ""adSoyad"": ""Fevzi Güler"",
                                      ""kullaniciAdi"": ""fevzig"",
                                      ""password"": ""1234"",
                                      ""birimId"": 1,
                                      ""phoneNumber"": ""5556667788""
                                    }
        
                                    *Çıkış yapmak için:
                                    {
                                     ""action"":""logout""   
                                    }
                            
                                    *İş atamak için:
                                    {
                                    ""action"": ""isAtaAI"",
                                    ""isBaslik"": ""taşıma"",
                                    ""isAciklama"": ""fıstık taşımacılığı""
                                    ""kullaniciAdi"": ""fevzig""
                                    }
                        
                                    *Kullanıcının tüm işlerini listelemek için:
                                    {
                                    ""action"":""IsListeleAI"",
                                    ""kullaniciAdi"":""fevzig""
                                    }
        
                                    *Kullancının belilirli bir tarihteki işlerini listelemek için:
                                    {
                                    ""action"":""IsListeleTarihAI"",
                                    ""kullaniciAdi"":""fevzig"",
                                    ""tarih"":""2025-03-29""
                                    }
                                    
                                    *Kullanıcı bilgilerini editlemek düzeltmek için:
                                    {   
                                    ""action"":""WorkerEditPageAI"",
                                    ""kullaniciAdi"":""fevzig""
                                    }

                                    *Kullanıcının tüm bilgilerini görmek için:
                                    {
                                    ""action"":""WorkerProfilePageAI"",
                                    ""kullaniciAdi"":""fevzig""
                                    }

                                    Sadece JSON dön. Açıklama yazma,yorum ekleme, metin dışı sembol kullanma.";
            if (sayi == 0)
            {
                string finalPrompt = systemMessage + "\n\n" + prompt;
                Console.WriteLine("PROMPT BU : " + finalPrompt);
                chatHistory.Add(new { role = "user", parts = new[] { new { text = finalPrompt } } });
                sayi = 1;
            }
            else
            {
                Console.WriteLine("PROMPT BU : " + prompt);
                chatHistory.Add(new { role = "user", parts = new[] { new { text = prompt } } });
            }
            var requestBody = new
            {
                contents = chatHistory.ToArray()
                //contents = new[]
                //{
                //    new
                //    {
                //        parts = new[]
                //        {
                //            new { text = prompt }
                //        }
                //    }
                //}
            };

            var jsonRequestBody = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_apiUri}?key={_apiKey}", jsonRequestBody);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
                string aiResponse = responseObject.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                chatHistory.Add(new { role = "model", parts = new[] { new { text = aiResponse } } });
                aiResponse = aiResponse.Replace("```json", "").Replace("```", "").Trim();
                TempData["AIResponse"] = aiResponse;
                Console.WriteLine("buraya ai repsonse geliyor ::::" + aiResponse);
                //return responseObject.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
            }
            else
            {
                TempData["AIResponse"] = $"API Hatası: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
            }

            return RedirectToAction("HandleAIAction");


        }

        //----------------------------------------------------------------------------------
        public IActionResult HandleAIAction()
        {
            if (TempData["AIResponse"] != null)
            {
                string json = TempData["AIResponse"].ToString();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                AIRegisterCommand command = JsonSerializer.Deserialize<AIRegisterCommand>(json, options);

                if (command?.action == "register")
                {
                    TempData["AIResponse"] = json;
                    return RedirectToAction("RegisterAI");
                }
                if (command?.action == "logout")
                {
                    TempData["AIResponse"] = json;
                    return RedirectToAction("LogoutAI");
                }
                if (command?.action == "isAtaAI")
                {
                    TempData["AIResponse"] = json;
                    return RedirectToAction("IsAtaAI");
                }
                if (command?.action == "IsListeleAI")
                {
                    TempData["AIResponse"] = json;
                    return RedirectToAction("IsListeleAI");
                }
                if (command?.action == "IsListeleTarihAI")
                {
                    TempData["AIResponse"] = json;
                    return RedirectToAction("IsListeleTarihAI");
                }
                if (command?.action == "WorkerEditPageAI")
                {
                    TempData["AIResponse"] = json;
                    return RedirectToAction("WorkerEditPageAI");
                }
                if (command?.action == "WorkerProfilePageAI")
                {
                    TempData["AIResponse"] = json;
                    return RedirectToAction("WorkerProfilePageAI");
                }

                else
                {
                    TempData["AIError"] = "Bilinmeyen eylem: " + command?.action;
                    return RedirectToAction("AIPage", "Home");
                }
            }

            TempData["AIError"] = "AI cevabı bulunamadı.";
            return RedirectToAction("AIPage", "Home");
        }

        //--------------------------------------------------------------------------
        public IActionResult RegisterAI()
        {

            if (TempData["AIResponse"] != null)
            {
                string json = TempData["AIResponse"].ToString();
                Console.WriteLine("burada jsonu yazdırıyorum:" + json);

                try
                {
                    Console.WriteLine(json);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    AIRegisterCommand command = JsonSerializer.Deserialize<AIRegisterCommand>(json, options);

                    if (command != null && command.action == "register")
                    {

                        var existingUser = _context.Personellers.FirstOrDefault(u => u.PersonelKullaniciAd == command.kullaniciAdi);
                        if (existingUser != null)
                        {
                            TempData["AIError"] = "Bu kullanıcı adı mevcut";
                            Console.WriteLine("hata1");
                            return RedirectToAction("AIPage", "Home");
                        }
                        var newPersonel = new Personeller
                        {
                            PersonelAdSoyad = command.adSoyad,
                            PersonelKullaniciAd = command.kullaniciAdi,
                            PersonelParola = command.password,
                            PersonelBirimId = command.birimId,
                            PersonelYetkiTurId = 2,
                            PersonelTelefonNo = command.phoneNumber
                        };
                        _context.Personellers.Add(newPersonel);
                        _context.SaveChanges();
                        TempData["AISuccess"] = "Kullanıcı başarıyla eklendi!";
                    }

                    else
                    {
                        TempData["AIError"] = "Geçersiz işlem tipi: " + command?.action;
                        Console.WriteLine("hata2");
                    }
                }
                catch (Exception ex)
                {
                    TempData["AIError"] = "JSON çözümlenirken hata oluştu: " + ex.Message;
                    Console.WriteLine("İçsel Hata: " + ex.InnerException?.Message);
                    Console.WriteLine("hata3");
                }
            }
            else
            {
                TempData["AIError"] = "AI verisi bulunamadı.";
                Console.WriteLine("hata4");
            }

            return RedirectToAction("AIPage", "Home");
        }

        //------------------------------------------------------------------------------------



        public IActionResult LogoutAI()
        {
            if (TempData["AIResponse"] != null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("LoginPage", "Home");
            }
            else
            {
                TempData["AIError"] = "Çıkış yapılamadı";
                Console.WriteLine("hata5");
            }
            return RedirectToAction("AIPage", "Home");
        }





        //---------------------------------------------------------------------------------------

        public IActionResult IsAtaAI()
        {
            if (TempData["AIResponse"] != null)
            {
                string json = TempData["AIResponse"].ToString();
                Console.WriteLine("burada jsonu yazdırıyorum:" + json);

                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    AIIsAtaCommand command = JsonSerializer.Deserialize<AIIsAtaCommand>(json, options);
                    bool control = AIControlHelper.Control(command.kullaniciAdi, command.action, _context);

                    if (command != null && command.action == "isAtaAI" && control)
                    {
                        var user = _context.Personellers.FirstOrDefault(u => u.PersonelKullaniciAd == command.kullaniciAdi);

                        var yeniIs = new Isler
                        {
                            IsBaslik = command.isBaslik,
                            IsAciklama = command.isAciklama,
                            IsPersonelId = user.PersonelId,
                            IletilenTarih = DateTime.Now,
                            IsDurumId = 1,
                            IsOkunma = false
                        };
                        _context.Islers.Add(yeniIs);
                        _context.SaveChanges();

                    }
                    else
                    {
                        TempData["AIError"] = "İş atanamadı, Kullanıcı veya action bulunamadı.";
                    }


                }
                catch (Exception ex)
                {
                    {
                        TempData["AIError"] = "İş atanamadı.";
                        Console.WriteLine("hata6" + ex.Message);
                    }


                }
                return RedirectToAction("AIPage", "Home");


            }
            else
            {
                TempData["AIError"] = "İş atama işlemi yapılamadı";
                Console.WriteLine("hata7");
                return RedirectToAction("AIPage", "Home");
            }
        }

        //____________________________________________________________________________


        public IActionResult IsListeleAI()
        {
            if (TempData["AIResponse"] != null)
            {
                string json = TempData["AIResponse"].ToString();
                Console.WriteLine("burada jsonu yazdırıyorum:" + json);

                try
                {
                    Console.WriteLine(json);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    AIIsListeleCommand command = JsonSerializer.Deserialize<AIIsListeleCommand>(json, options);
                    bool control = AIControlHelper.Control(command.kullaniciAdi, command.action, _context);

                    if (command != null && command.action == "IsListeleAI" && control)
                    {
                        var user = _context.Personellers.FirstOrDefault(u => u.PersonelKullaniciAd == command.kullaniciAdi);

                        Console.WriteLine("user ismi: " + user.PersonelAdSoyad);
                        var islerListesi = _context.Islers.Where(b => b.IsPersonelId == user.PersonelId)
                            .Join(_context.Durumlars,
                            isler => isler.IsDurumId,
                            durum => durum.DurumId,
                            (isler, durum) => new IsDetayViewModel
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
                            Personel = user,
                            Isler = islerListesi,
                        };
                        Console.WriteLine(viewModel);
                        Console.WriteLine($"Isler Listesi: {islerListesi.Count} adet iş var");

                        return View("~/Views/Home/IsListele.cshtml", viewModel);

                    }
                    else
                    {
                        TempData["AIError"] = "İşler listelenetmedi.Kullanıcı adı veya action bulunamadı.";
                    }
                }

                catch (Exception ex)
                {
                    {
                        TempData["AIError"] = "İş listelenemedi.";
                        Console.WriteLine("hata7" + ex.Message);
                    }


                }
                return RedirectToAction("AIPage", "Home");
            }
            else
            {

            }
            return RedirectToAction("AIPage", "Home");
        }

        //-------------------------------------------------------------------------------------

        public IActionResult IsListeleTarihAI()
        {
            if (TempData["AIResponse"] != null)
            {
                string json = TempData["AIResponse"].ToString();
                Console.WriteLine("burada jsonu yazdırıyorum:" + json);
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    AIIsListeleTarihCommand command = JsonSerializer.Deserialize<AIIsListeleTarihCommand>(json);
                    bool control = AIControlHelper.Control(command.kullaniciAdi, command.action, _context);

                    if (command != null && command.action == "IsListeleTarihAI" && control)
                    {
                        var user = _context.Personellers.FirstOrDefault(u => u.PersonelKullaniciAd == command.kullaniciAdi);
                        Console.WriteLine("user ismi: " + user.PersonelAdSoyad);
                        var islerListesi = _context.Islers.Where(b => b.IsPersonelId == user.PersonelId &&
                        (b.YapilanTarih.Value.Date == command.tarih || b.IletilenTarih.Value.Date == command.tarih))
                            .Join(_context.Durumlars,
                            isler => isler.IsDurumId,
                            durum => durum.DurumId,
                            (isler, durum) => new IsDetayViewModel
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
                        ViewBag.tarih = command.tarih;
                        var viewModel = new IsTakipViewModel
                        {
                            Personel = user,
                            Isler = islerListesi,
                        };
                        return View("AIIsListeleTarihPage", viewModel);
                    }
                    else
                    {
                        TempData["AIError"] = "İşler listelenemedi. Kullanıcı adı ve ya action bulunamadı.";
                        return RedirectToAction("AIPage", "Home");
                    }
                }
                catch (Exception ex)
                {
                    TempData["AIError"] = "İş listelenemedi.";
                    Console.WriteLine("hata8" + ex.Message);
                    return RedirectToAction("AIPage", "Home");
                }
            }
            else
            {
                return RedirectToAction("AIPage", "Home");

            }

        }

        //--------------------------------------------------------------------------------------

        public IActionResult WorkerEditPageAI()
        {
            if (TempData["AIResponse"] != null)
            {
                string json = TempData["AIResponse"].ToString();
                Console.WriteLine("Burada jsonu yazdırıyorum: " + json);

                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    AIWorkerEditPageCommand command = JsonSerializer.Deserialize<AIWorkerEditPageCommand>(json);
                    bool control = AIControlHelper.Control(command.kullaniciAdi, command.action, _context);

                    if (command != null && command.action == "WorkerEditPageAI" && control)
                    {
                        var user = _context.Personellers.FirstOrDefault(u => u.PersonelKullaniciAd == command.kullaniciAdi);
                        Console.WriteLine("useerpersonelID= " + user.PersonelId);
                        return View("~/Views/Home/WorkerEditPage.cshtml", user);
                    }
                    else
                    {
                        TempData["AIError"] = "Çalışan düzenlemesi yapılamıyor.Kullancı adı veya action bulunamadı.";
                        return RedirectToAction("AIPage", "Home");
                    }
                }

                catch (Exception ex)
                {
                    TempData["AIError"] = "Worker edit sayfasına gidilemiyor";
                    Console.WriteLine("hata9");
                    return RedirectToAction("AIPage", "Home");

                }

            }
            else
            {
                TempData["AIError"] = "Worker Edit Page kısmına gidilemiyor.";
                Console.WriteLine("hata10");
                return RedirectToAction("AIPage", "Home");
            }

        }

        //-------------------------------------------------------------------------------------

        public IActionResult WorkerProfilePageAI()
        {
            if (TempData["AIResponse"] != null)
            {
                string json = TempData["AIResponse"].ToString();
                Console.WriteLine("Burada jsonu yazdırıyorum: " + json);

                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    AIWorkerProifleCommand command = JsonSerializer.Deserialize<AIWorkerProifleCommand>(json);
                    bool control = AIControlHelper.Control(command.kullaniciAdi, command.action, _context);

                    if (command != null && command.action == "WorkerProfilePageAI" && control)
                    {
                        var personel = _context.Personellers.FirstOrDefault(u => u.PersonelKullaniciAd == command.kullaniciAdi);
                        if (personel == null)
                        {
                            return RedirectToAction("AIPage", "Home");
                        }
                        else
                        {
                            return View("AIWorkerProfilePage", personel);
                        }
                    }
                    else
                    {
                        TempData["AIError"] = "Çalışan profil sayfasına ulaşılamıyor.Kullanıcı adı veya action bulunamadı.";
                        return RedirectToAction("AIPage", "Home");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("hata11");
                    return RedirectToAction("AIPage", "Home");
                }
            }
            else
            {
                TempData["AIError"] = "Worker Profile Page kısmına gidilemiyor.";
                Console.WriteLine("hata12");
                return RedirectToAction("AIPage", "Home");
            }

        }
    }
}




//işlistele yap ---
//şu gün tamamlanmış işleri listele---
//olumsuz yorum atılmış işleri listele !!!!!!!!!!
//belirli bir kullanıcı bilgilerini editle---
//çalışan ve patron arasında konuşma yaptırt database (ekleme yapmalısın)!!!!!!!
//çalışanlara fotoğraf ekleme işleme yap database ekleme___

//HATA AI A YANLIŞ KULLANCI ADLI KOMUT GİRİNCE BUGA GİRİYOR

