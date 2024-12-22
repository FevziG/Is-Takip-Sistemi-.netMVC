namespace IsTakipSistemiMVC.Models
{
    public class ProfileViewModel
    {
        public Personeller Personel { get; set; }
        public Birimler Birimler { get; set; }
        public List<ProfileDetayViewModel> Isler { get; set; }
    }
    public class ProfileDetayViewModel
    {
        public int IsId { get; set; }
        public string IsBaslik { get; set; }
        public string IsAciklama { get; set; }
        public DateTime? IletilenTarih { get; set; }
        public DateTime? YapilanTarih { get; set; }
        public string IsYorum { get; set; }
        public string DurumAd { get; set; }
    }
}
