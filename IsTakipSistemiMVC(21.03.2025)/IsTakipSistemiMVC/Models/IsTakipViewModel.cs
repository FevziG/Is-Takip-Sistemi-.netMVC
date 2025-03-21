namespace IsTakipSistemiMVC.Models
{
    public class IsTakipViewModel
    {
        public Personeller Personel { get; set; }
        public List<IsDetayViewModel> Isler { get; set; }
    }
    public class IsDetayViewModel
    {
        public int IsId { get; set; }
        public string IsBaslik { get; set; }
        public string IsAciklama { get; set; }
        public int? IsDurumId { get; set; }
        public DateTime? IletilenTarih { get; set; }
        public DateTime? YapilanTarih { get; set; }
        public string IsYorum { get; set; }
        public string DurumAd { get; set; }
    }
}
