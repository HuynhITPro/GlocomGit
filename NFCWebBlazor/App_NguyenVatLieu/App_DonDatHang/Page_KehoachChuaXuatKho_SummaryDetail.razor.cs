using BlazorBootstrap;

using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class Page_KehoachChuaXuatKho_SummaryDetail
    {
        [Inject]
        PhanQuyenAccess phanQuyenAccess { get; set; }
        [Inject]
        ToastService ToastService { get; set; }
        bool PhanQuyenCheck = false;
        bool PhanQuyenDuyet = false;
        List<NvlKeHoachMuaHangItemShow> lstgroup = new List<NvlKeHoachMuaHangItemShow>();
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (nvlKeHoachMuaHangItemSummarycrr.lstitem == null)
                {
                    await searchAsync();
                }
                else
                {
                    lstdata= nvlKeHoachMuaHangItemSummarycrr.lstitem;
                    Grid.Reload();
                    StateHasChanged();
                }
            }

        }
        private async Task searchAsync()
        {
            if (String.IsNullOrEmpty(ListSerialDN))
                return;
            PanelVisible = true;
            
            if (lstdata == null)
                lstdata = new List<NvlKeHoachMuaHangItemShow>();
            lstdata.Clear();
           
            string sql = "";
            try
            {
                List<ParameterDefine> lstpara = new List<ParameterDefine>();
                if (string.IsNullOrEmpty(nvlKeHoachMuaHangItemSummarycrr.MaSP))
                {
                    
                    
                    sql = string.Format(@"Use NVLDB
                   
                   
                    select qry.*,hh.TenHang,hh.DVT from 
                                                (SELECT  [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
                                                          ,[SoLuong],SLTheoDoi,SLHuy,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet,KeyGroup
                                                      FROM [NvlKeHoachMuaHangItem]
                                                      Where SerialDN in ({0}) and KeyGroup is null and MaHang=@MaHang) as qry
									              
									                  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
									                 order by qry.STT,qry.Serial", ListSerialDN);
                    lstpara.Add(new ParameterDefine("@MaHang", nvlKeHoachMuaHangItemSummarycrr.MaHang));

                }
                else
                {
                    sql = string.Format(@"Use NVLDB
                    
                       declare @tbldm table(KeyGroup nvarchar(100))
                   insert into @tbldm(KeyGroup)
                SELECT  KeyGroup FROM [dbo].[NvlKeHoachMuaHang_DinhMuc]
                 where SerialLink in ({0}) and TableName='NvlKehoachMuaHang'
                  and MaSP=@MaSP and MaMau=@MaMau and TenDinhMuc=@TenDinhMuc and CongDoan=@CongDoan
                  group by KeyGroup
                    select qry.*,hh.TenHang,hh.DVT from 
                                                (SELECT  [Serial],[SerialDN],[MaHang],MaSP,isnull(STT,0) as STT
                                                          ,[SoLuong],SLTheoDoi,[DonGia],SLHuy ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet,KeyGroup
                                                      FROM [NvlKeHoachMuaHangItem]
                                                      Where KeyGroup in (select KeyGroup from @tbldm) and MaHang=@MaHang ) as qry
									              
									                  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
									                 order by qry.STT,qry.Serial", ListSerialDN);
                    lstpara.Add(new ParameterDefine("@MaHang", nvlKeHoachMuaHangItemSummarycrr.MaHang));
                    lstpara.Add(new ParameterDefine("@MaSP", nvlKeHoachMuaHangItemSummarycrr.MaSP));
                    lstpara.Add(new ParameterDefine("@MaMau", nvlKeHoachMuaHangItemSummarycrr.MaMau));
                    lstpara.Add(new ParameterDefine("@CongDoan", nvlKeHoachMuaHangItemSummarycrr.CongDoan));
                    lstpara.Add(new ParameterDefine("@TenDinhMuc", nvlKeHoachMuaHangItemSummarycrr.TenDinhMuc));
                }
                CallAPI callAPI = new CallAPI();
               
               
                

                string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<NvlKeHoachMuaHangItemShow>>(json);
                    if (query != null)
                    {
                       
                        lstdata.AddRange(query);

                        nvlKeHoachMuaHangItemSummarycrr.lstitem= lstdata;
                    }
                    //Grid.GetDataColumns().First(i => i.FieldName == "SerialLink").GroupIndex = 0;
                    Grid.Reload();
                }
                PanelVisible = false;
                //keHoachMuaHang_Show.lstitem = query;

                //keHoachMuaHang_Show.lstitem.AddRange(query);

            }
            catch (Exception ex)
            {
                ToastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message}"));
            }
            finally
            {
                PanelVisible = false;
                Grid.Reload();

                StateHasChanged();
            }
        }
    
        class KetquaResult
        {
            public int? Serial { get; set; }

            public string? ketqua { get; set; }
            public double? SLCL { get; set; }

            public string? ketquaexception { get; set; }

        }
      
      

    }
}

