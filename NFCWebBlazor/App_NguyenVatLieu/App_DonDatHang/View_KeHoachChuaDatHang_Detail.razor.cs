using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Collections.ObjectModel;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_DonDatHang
{
    public partial class View_KeHoachChuaDatHang_Detail
    {
        [Inject]
        ToastService toastService { get; set; }
        private async Task loaddatadropdownAsync()
        {
            //Console.WriteLine(this.GetType().Name);
            await  searchAsync();
          
        }
        private async Task searchAsync()
        {
           
                PanelVisible = true;

                string sql = "";
                try
                {
                    //KeHoachSanXuat Không cần duyệt
                    sql = string.Format(@"use NVLDB
                     declare @SerialDN int={0}
					 declare @LoaiKeHoach nvarchar(100)
					 select @LoaiKeHoach=LoaiKehoach from NvlKehoachMuaHang where Serial=@SerialDN

                    select qry.*,hh.TenHang,nh.PhanLoai,hh.DVT,isnull(sp.TenSP,N'Vật tư ngoài kế hoạch') as TenSP,khsp.SoLuongSP,qry.SerialLink,art.ArticleNumber,mm.TenMau,isnull(mm.Color,0) as Color
                    ,isnull(qrytk.SLTon,0) as SLTon,hh.MinTK,hh.MaxTK,SLHuy
					,case when @LoaiKeHoach=N'KeHoachSanXuat' then N'Đã duyệt' 
					 when qryduyet.SerialLinkItem is null then N'Chưa duyệt' else N'Đã duyệt' end as TextDuyet from 
                                (SELECT  [Serial],[SerialDN],[MaHang],MaSP
                                          ,[SoLuong],SLTheoDoi,[DonGia] ,[VAT],[GhiChu],NgayEdit,[NgayInsert],[UserInsert] ,[SerialLink],[TableName],DuyetItemMsg,HuyDatHang,isnull(TenLienKet,'') as TenLienKet,SLHuy
                                      FROM [NvlKeHoachMuaHangItem]
                                      Where SerialDN = @SerialDN) as qry
                                       
									  left join (select MaHang,sum(SLNhap-SLXuat) as SLTon from NvlNhapXuatItem group by MaHang) as qrytk
										on qry.MaHang=qrytk.MaHang
									left join (SELECT [SerialLinkItem] 
	                    
									  FROM [NvlKyDuyetItem]
									  where TableName='NvlKehoachMuaHang' and LoaiDuyet=N'Duyệt' and SerialLinkMaster=@SerialDN
									 group by SerialLinkItem) as qryduyet on qry.Serial=qryduyet.SerialLinkItem
									  inner join dbo.NvlHangHoa hh on qry.MaHang=hh.MaHang
                                       inner join NvlNhomHang nh on hh.MaNhom=nh.MaNhom 
									  left join (select * from dbo.NvlKeHoachSP where SerialDN=@SerialDN) khsp on (qry.SerialLink=khsp.STT and qry.TableName='NvlKeHoachSP')
									  left join dbo.GetArticleNumber() art 
									  on art.ArticleNumber=khsp.ArticleNumber
									  left join dbo.GetMaMau() mm on art.MaMau=mm.MaMau
									  left join dbo.GetSanPham() sp on qry.MaSP=sp.MaSP
                    ", keHoachMuaHangcrr.Serial);
                    CallAPI callAPI = new CallAPI();
                    List<ParameterDefine> lstpara = new List<ParameterDefine>();
                    string json = await callAPI.ExcuteQueryEncryptAsync(sql, lstpara);
                    if (json != "")
                    {
                        var query = JsonConvert.DeserializeObject<List<NvlKeHoachMuaHangItemShow>>(json);
                        if (query != null)
                        {
                            lstdata = new ObservableCollection<NvlKeHoachMuaHangItemShow>(query);


                            keHoachMuaHangcrr.lstitem = lstdata;
                            query.Clear();
                        }
                        Grid.Reload();
                    }
                    PanelVisible = false;
                    //keHoachMuaHang_Show.lstitem = query;

                    //keHoachMuaHang_Show.lstitem.AddRange(query);

                }
                catch (Exception ex)
                {
                    toastService.Notify(new(ToastType.Danger, $"Lỗi: {ex.Message}"));
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    PanelVisible = false;
                    Grid.Reload();
                    StateHasChanged();
                }
           
        }
        private async Task huydathangAsync(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow)
        {
            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_HuyDatHang";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Serial", nvlKeHoachMuaHangItemShow.Serial));

            lstpara.Add(new ParameterDefine("@HuyDatHang", "Hủy"));
            double SLTheoDoi = nvlKeHoachMuaHangItemShow.SLTheoDoi.Value;
            string huydathang = "Hủy";
            try
            {

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        if(SLTheoDoi>0)
                        {
                            toastService.Notify(new ToastMessage(ToastType.Success, $"Kế hoạch này còn {SLTheoDoi}. Bạn chỉ hủy được {SLTheoDoi}"));
                        }
                        else
                            toastService.Notify(new ToastMessage(ToastType.Success, $"Kế hoạch này đã đặt hàng đủ rồi."));
                        nvlKeHoachMuaHangItemShow.SLHuy = nvlKeHoachMuaHangItemShow.SLTheoDoi;
                        nvlKeHoachMuaHangItemShow.SLTheoDoi = 0;
                        nvlKeHoachMuaHangItemShow.HuyDatHang = huydathang;
                       await Grid.SaveChangesAsync();
                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0},{1}", query[0].ketqua, query[0].ketquaexception)));
                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0}", ex.Message)));
                Console.WriteLine(ex.Message.ToString());
            }

        }
        private async Task tieptucdathangAsync(NvlKeHoachMuaHangItemShow nvlKeHoachMuaHangItemShow)
        {
            CallAPI callAPI = new CallAPI();
            string sql = "NVLDB.dbo.NvlKeHoachMuaHangItem_HuyDatHang";
            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            lstpara.Add(new ParameterDefine("@Serial", nvlKeHoachMuaHangItemShow.Serial));

            lstpara.Add(new ParameterDefine("@HuyDatHang",null));
            
            string huydathang = null;
            try
            {

                string json = await callAPI.ProcedureEncryptAsync(sql, lstpara);
                if (json != "")
                {
                    var query = JsonConvert.DeserializeObject<List<Ketqua>>(json);
                    if (query[0].ketqua == "OK")
                    {
                        toastService.Notify(new ToastMessage(ToastType.Success, $"Lưu thành công"));
                        nvlKeHoachMuaHangItemShow.GhiChu = "";
                        nvlKeHoachMuaHangItemShow.SLTheoDoi = nvlKeHoachMuaHangItemShow.SLHuy;
                        nvlKeHoachMuaHangItemShow.HuyDatHang = huydathang;
                        await Grid.SaveChangesAsync();

                    }
                    else
                    {
                        toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0},{1}", query[0].ketqua, query[0].ketquaexception)));
                    }
                    //Grid.Data = lstDonDatHangSearchShow;
                }
            }
            catch (Exception ex)
            {
                toastService.Notify(new ToastMessage(ToastType.Warning, string.Format("Lỗi: {0}", ex.Message)));
                Console.WriteLine(ex.Message.ToString());
            }
            //SqlConnection sqlConnection = prs.ConnectNVL();
            //sqlConnection.Open();
            //try
            //{
            //    SqlCommand sqlCommand = new SqlCommand("NvlKeHoachMuaHangItem_HuyDatHang", sqlConnection);
            //    sqlCommand.CommandType = CommandType.StoredProcedure;
            //    sqlCommand.Parameters.AddWithValue("@Serial", nvlKeHoachMuaHangItemShowcrr.Serial);
            //    string huydathang = null;

            //    sqlCommand.Parameters.AddWithValue("@HuyDatHang", DBNull.Value);

            //    DataTable dataTable = prs.dt_sqlcmd(sqlCommand, sqlConnection);
            //    if (dataTable == null)
            //    {
            //        msgBox.Show("Đã xảy ra lỗi", IconMsg.iconwarning);

            //    }
            //    else
            //    {
            //        if (dataTable.Rows.Count > 0)
            //        {
            //            string ketqua = dataTable.Rows[0]["ketqua"].ToString();
            //            if (ketqua == "OK")
            //            {

            //                msgBox.Show("LƯU THÀNH CÔNG", IconMsg.iconssuccess);
            //                nvlKeHoachMuaHangItemShowcrr.GhiChu = "";
            //                nvlKeHoachMuaHangItemShowcrr.SLTheoDoi = nvlKeHoachMuaHangItemShowcrr.SLDatHang;


            //                nvlKeHoachMuaHangItemShowcrr.HuyDatHang = huydathang;
            //                gridcurrent.RefreshData();
            //            }
            //            else
            //            {
            //                msgBox.Show(ketqua, IconMsg.iconinfomation);
            //            }
            //        }
            //    }
            //    dataTable.Dispose();
            //    sqlCommand.Dispose();
            //    sqlConnection.Close();
            //}
            //catch (Exception ex)
            //{
            //    if (ex.Message.Contains("duplicate key"))
            //    {
            //        //msgBox.Show("Mã này đã tồn tại rồi", IconMsg.iconerror);

            //    }
            //    else
            //        msgBox.Show("Lỗi:" + ex.Message, IconMsg.iconerror);
            //}
        }
    }
}
