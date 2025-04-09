using System;
using System.Collections.Generic;

namespace IsTakipSistemiMVC.Models;

public partial class Personeller
{
    public int PersonelId { get; set; }

    public string? PersonelAdSoyad { get; set; }

    public string? PersonelKullaniciAd { get; set; }

    public string? PersonelParola { get; set; }

    public int? PersonelBirimId { get; set; }

    public int? PersonelYetkiTurId { get; set; }

    public string? PersonelTelefonNo { get; set; }

    public virtual ICollection<Isler> Islers { get; set; } = new List<Isler>();

    public virtual Birimler? PersonelBirim { get; set; }

    public virtual YetkiTurler? PersonelYetkiTur { get; set; }
}
