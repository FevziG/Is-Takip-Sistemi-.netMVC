using System;
using System.Collections.Generic;

namespace IsTakipSistemiMVC.Models;

public partial class Isler
{
    public int IsId { get; set; }

    public string? IsBaslik { get; set; }

    public string? IsAciklama { get; set; }

    public int? IsPersonelId { get; set; }

    public DateTime? IletilenTarih { get; set; }

    public DateTime? YapilanTarih { get; set; }

    public int? IsDurumId { get; set; }

    public string? IsYorum { get; set; }

    public bool IsOkunma { get; set; }

    public virtual Durumlar? IsDurum { get; set; }

    public virtual Personeller? IsPersonel { get; set; }
}
