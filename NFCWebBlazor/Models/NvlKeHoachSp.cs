﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace NFCWebBlazor.Models;

public partial class NvlKeHoachSp
{
    public int Stt { get; set; }

    public string Id { get; set; }

    public string MaKeHoach { get; set; }

    public string TenKeHoach { get; set; }

    public string MaSp { get; set; }

    public int SoLuongSp { get; set; }

    public string KhuVuc { get; set; }

    public string GhiChu { get; set; }

    public DateTime? Ngay { get; set; }

    public int? SoTtkh { get; set; }

    public int? Active { get; set; }

    public string NhaMay { get; set; }

    public string ArticleNumber { get; set; }

    public string ArticleNumberDm { get; set; }

    public string MaMauKh { get; set; }

    public string Cont { get; set; }

    public int? SerialKhthangItem { get; set; }

    public int? SerialDn { get; set; }

    public string Ponumber { get; set; }

    public string KyDuyet { get; set; }

    public int? BoSung { get; set; }

    public string UserInsert { get; set; }

    public DateTime? DateFinishActive { get; set; }

    public virtual ICollection<NvlKeHoachChiTiet> NvlKeHoachChiTiets { get; set; } = new List<NvlKeHoachChiTiet>();

    public virtual NvlKehoachMuaHang SerialDnNavigation { get; set; }
}