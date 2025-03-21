using System;
using System.Collections.Generic;

namespace IsTakipSistemiMVC.Models;

public partial class YetkiTurler
{
    public int YetkiTurId { get; set; }

    public string? YetkiTurAd { get; set; }

    public virtual ICollection<Personeller> Personellers { get; set; } = new List<Personeller>();
}
