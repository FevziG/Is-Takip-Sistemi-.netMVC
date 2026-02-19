🚀 İş Takip Sistemi
ASP.NET MVC Tabanlı AI Destekli Yönetim Uygulaması
📌 Proje Hakkında

İş Takip Sistemi, yöneticilerin çalışanlara görev atayabildiği, çalışanların görevlerini takip edip tamamlayabildiği web tabanlı bir yönetim uygulamasıdır.

Bu proje, klasik yönetim panellerinden farklı olarak doğal dil komutları ile işlem yapabilen AI destekli bir yönetim mekanizması içermektedir.

Sistem, manuel form doldurma süreçlerini azaltarak yönetim işlemlerini daha hızlı ve verimli hale getirmeyi amaçlamaktadır.

🤖 AI Destekli Komut Mekanizması

Uygulama içerisinde yer alan doğal dil komut sistemi sayesinde yöneticiler:

Çalışan ekleme / silme

Çalışanı işten çıkarma

Çalışana izin verme

Görev atama

Görev listeleme

Sistem içindeki yönetim işlemlerini gerçekleştirme

işlemlerini doğrudan yazılı komut vererek yapabilmektedir.

📌 Örnek Komutlar

Arda adlı çalışanı işten çıkar

Ayşe'ye 2 gün izin ver

Mehmet'e yarına kadar rapor hazırlama görevi ata

Tamamlanmamış görevleri listele

Sistem, girilen metni analiz ederek ilgili Controller metodunu otomatik olarak tetikler ve işlemi gerçekleştirir.

Bu yapı, klasik buton tabanlı yönetim anlayışına alternatif bir yaklaşım sunmaktadır.

🎯 Temel Özellikler

👤 Rol bazlı kullanıcı sistemi (Admin & Çalışan)

📝 Görev oluşturma ve atama

📊 Görev takibi ve durum yönetimi

💬 Görev sonrası yorum sistemi

🤖 Doğal dil komut desteği

🗂 Database First yaklaşımı

🏗 Kullanılan Teknolojiler

ASP.NET MVC

C#

Entity Framework Core

Microsoft SQL Server

HTML / CSS / Bootstrap

🧠 Mimari Yapı

Proje, MVC (Model-View-Controller) mimarisine uygun olarak geliştirilmiştir:

Model → Veri yapıları ve veritabanı yönetimi

View → Kullanıcı arayüzü

Controller → İş mantığı ve işlem yönlendirme

AI Komut Katmanı → Doğal dil analiz ederek uygun işlevi tetikleyen yapı

Entity Framework kullanılarak MSSQL veritabanı ile entegrasyon sağlanmıştır.

▶️ Kurulum ve Çalıştırma

Repository’i klonlayın:

git clone https://github.com/FevziG/Is-Takip-Sistemi-.netMVC.git


SQL Server üzerinde veritabanını oluşturun.

Connection string’i kendi SQL Server instance’ınıza göre düzenleyin.

Visual Studio 2022 ile projeyi açın.

Build & Run işlemi ile uygulamayı başlatın.

🎯 Projenin Amacı

Bu proje, geleneksel görev yönetim sistemlerini daha kullanıcı dostu ve hızlı hale getirmeyi amaçlamaktadır.

Doğal dil tabanlı komut sistemi ile yönetim işlemlerinin sadeleştirilmesi hedeflenmiştir.

👨‍💻 Geliştirici

Fevzi Güler
Bilgisayar Mühendisliği Mezunu
Backend & Full-Stack Development ile ilgilenmektedir.
