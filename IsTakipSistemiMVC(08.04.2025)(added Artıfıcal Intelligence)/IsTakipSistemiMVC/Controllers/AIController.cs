using System;
using System.Text;
using System.Text.Json;
using IsTakipSistemiMVC.Models;
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
                                      ""adSoyad"": ""Ali Veli"",
                                      ""username"": ""aliveli"",
                                      ""password"": ""1234"",
                                      ""birimId"": 1,
                                      ""phoneNumber"": ""05556667788""
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
                                    ""kullaniciAdi"": ""aliveli""
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
                else
                {
                    TempData["RegisterAIError"] = "Bilinmeyen eylem: " + command?.action;
                    return RedirectToAction("AIPage", "Home");
                }
            }

            TempData["RegisterAIError"] = "AI cevabı bulunamadı.";
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

                        var existingUser = _context.Personellers.FirstOrDefault(u => u.PersonelKullaniciAd == command.username);
                        if (existingUser != null)
                        {
                            TempData["RegisterAIError"] = "Bu kullanıcı adı mevcut";
                            Console.WriteLine("hata1");
                            return RedirectToAction("AIPage", "Home");
                        }
                        var newPersonel = new Personeller
                        {
                            PersonelAdSoyad = command.adSoyad,
                            PersonelKullaniciAd = command.username,
                            PersonelParola = command.password,
                            PersonelBirimId = command.birimId,
                            PersonelYetkiTurId = 2,
                            PersonelTelefonNo = command.phoneNumber
                        };
                        _context.Personellers.Add(newPersonel);
                        _context.SaveChanges();
                        TempData["RegisterAISuccess"] = "Kullanıcı başarıyla eklendi!";
                    }

                    else
                    {
                        TempData["RegisterAIError"] = "Geçersiz işlem tipi: " + command?.action;
                        Console.WriteLine("hata2");
                    }
                }
                catch (Exception ex)
                {
                    TempData["RegisterAIError"] = "JSON çözümlenirken hata oluştu: " + ex.Message;
                    Console.WriteLine("İçsel Hata: " + ex.InnerException?.Message);
                    Console.WriteLine("hata3");
                }
            }
            else
            {
                TempData["RegisterAIError"] = "AI verisi bulunamadı.";
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
                TempData["LogoutAIError"] = "Çıkış yapılamadı";
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
                    Console.WriteLine(json);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    AIIsAtaCommand command = JsonSerializer.Deserialize<AIIsAtaCommand>(json, options);

                    if (command != null && command.action == "isAtaAI")
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
                   

                }
                catch (Exception ex)
                {
                    {
                        TempData["IsAtaAIError"] = "İş atanamadı.";
                        Console.WriteLine("hata6" + ex.Message);
                    }


                }
                return RedirectToAction("AIPage", "Home");


            }
            else 
            {
                TempData["IsAtaAI"] = "İş atama işlemi yapılamadı";
                Console.WriteLine("hata7");
                return RedirectToAction("AIPage","Home");
            }
        }
    }
}

//istakip yap
//tamamlanmamış olan işleri listele
//tamamlanmış işleri listele
//şu gün tamamlanmış işleri listele
//olumsuz yorum atılmış işleri listele
//kullanıcı bilgilerini görüntüle
//çalışan ve patron arasında konuşma yaptırt database (ekleme yapmalısın)