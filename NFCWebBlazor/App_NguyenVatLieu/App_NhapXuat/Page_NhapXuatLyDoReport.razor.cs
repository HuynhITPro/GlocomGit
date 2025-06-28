using BlazorBootstrap;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;
using System.Data;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_NhapXuat
{
    public partial class Page_NhapXuatLyDoReport
    {
        [Inject] ToastService toastService { get; set; }
        [Inject] IJSRuntime _jsRuntime { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var dimension = await browserService.GetDimensions();
            // var heighrow = await browserService.GetHeighWithID("divcontainer");
            int height = dimension.Height - 70;
            heightgrid = string.Format("{0}px", height);
            await loadAsync();
            //return base.OnInitializedAsync();
        }
        private async Task loadAsync()
        {
            lstkhonvl = await Model.ModelData.GetKhoNvl();
            lstlydo = await Model.ModelData.Getlstnvllydo();
            lstnoigiaonhan = await Model.ModelData.Getlstnoigiaonhan();

        }

        class ItemReport
        {
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string NameGroup { get; set; }
            public string TextGroup { get; set; }
            public string DVT { get; set; }
            public DateTime Ngay { get; set; }
            public string QuyCach { get; set; }
            public string TenNhom { get; set; }
            public double SLNhap { get; set; }
            public double SLXuat { get; set; }
        }
        class itemnhapxuat
        {
            public string value;
            public string text;
            public itemnhapxuat(string _value, string _text)
            {
                value = _value;
                text = _text;

            }
            public itemnhapxuat()
            {

            }
        }
        bool expand = true;
        private async void expandall()
        {
            dxGrid.BeginUpdate();
            dxGrid.AutoExpandAllGroupRows = expand;
            dxGrid.EndUpdate();
            expand = !expand;
            StateHasChanged();
        }
        List<QuyDoiNgay> lstquydoi = new List<QuyDoiNgay>();
        string dieukienshow = "";
        public class CustomRoot
        {
            // Bind "Table" in JSON to "RequestList"
            [JsonProperty("Table")]
            public List<KeHoachMuaHang_Show> lstmuahang { get; set; }

            // Bind "Table1" in JSON to "SimpleRequestList"
            [JsonProperty("Table1")]
            public List<NvlKyDuyetShow> lstkyduyet { get; set; }
        }

        RenderFragment renderFragmentcolumn;
        private RenderFragment CreateColumns(List<itemnhapxuat> lstheader, List<itemnhapxuat> lstitem)
        {
            return builder =>
            {
                int sequence = 0;

                // Tạo DxGridBandColumn
                foreach (itemnhapxuat header in lstheader)
                {
     
                    builder.OpenComponent<DxGridBandColumn>(sequence++);
                    builder.AddAttribute(sequence++, "Caption", header.text);
                    // Render các cột bên trong BandColumn
                    builder.AddAttribute(sequence++, "Columns", (RenderFragment)(nestedBuilder =>
                    {
                        var query = lstitem.Where(p => p.value.StartsWith(header.value)).ToList();
                        foreach (var it in query)
                        {
                            nestedBuilder.OpenComponent<DxGridDataColumn>(sequence++);
                            nestedBuilder.AddAttribute(sequence++, "FieldName", it.value);
                            nestedBuilder.AddAttribute(sequence++, "Caption", it.text);
                            nestedBuilder.AddAttribute(sequence++, "Width", string.Format("{0}px", "85")); //" 110);
                            nestedBuilder.AddAttribute(sequence++, "DisplayFormat", "#,0.##;-#,0.##;#");
                            nestedBuilder.CloseComponent();
                        }
                    }));

                    builder.CloseComponent();
                }
                builder.OpenComponent<DxGridBandColumn>(sequence++);
                builder.AddAttribute(sequence++, "Caption", string.Format("Tồn đến {0}",dtpend.Value.ToString("dd/MM/yy")));
                // Render các cột bên trong BandColumn
                builder.AddAttribute(sequence++, "Columns", (RenderFragment)(nestedBuilder =>
                {
                    
                        nestedBuilder.OpenComponent<DxGridDataColumn>(sequence++);
                        nestedBuilder.AddAttribute(sequence++, "FieldName", "SLTon");
                        nestedBuilder.AddAttribute(sequence++, "Caption", "SL tồn");
                        nestedBuilder.AddAttribute(sequence++, "Width", string.Format("{0}px", "85")); //" 110);
                        nestedBuilder.AddAttribute(sequence++, "DisplayFormat", "#,0.##;-#,0.##;#");
                        nestedBuilder.CloseComponent();
                    
                }));

                builder.CloseComponent();

            };
        }
        List<itemnhapxuat> lstheader = new List<itemnhapxuat>();
        List<itemnhapxuat> lsttextgroup = new List<itemnhapxuat>();
        List<itemnhapxuat> lstitem = new List<itemnhapxuat>();
        App_ClassDefine.ClassProcess prs = new App_ClassDefine.ClassProcess();

        public async void searchAsync()
        {
            dieukienshow = "";
            if (dtpbegin == null || dtpend == null)
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Chọn ngày báo cáo"));
                return;
            }
            dieukienshow = string.Format("Từ ngày {0} - {1}", dtpbegin.Value.ToString("dd/MM/yy"), dtpend.Value.ToString("dd/MM/yy"));
            headerdauky = string.Format("Tồn trước ngày {0}", dtpbegin.Value.ToString("dd/MM/yy"));
            if (string.IsNullOrEmpty(loaibaocao))
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, "Chọn loại gộp"));

                return;
            }
          
            

            string KieuChuyen = "", sqlreplace = "";
           

            List<ParameterDefine> lstpara = new List<ParameterDefine>();
            string dieukien = " ";

            string dieukienmahang = " ";

            dtsource.Clear();
            lsttextgroup.Clear();
            renderFragmentcolumn = null;

            dieukien = " where Ngay<=@DateEnd";
            lstpara.Add(new ParameterDefine("@DateBegin", dtpbegin.Value.ToString("MM/dd/yyyy 00:00")));
            lstpara.Add(new ParameterDefine("@DateEnd", dtpend.Value.ToString("MM/dd/yyyy 23:59")));
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaKho))
            {
                dieukien += " and MaKho=@MaKho";
                lstpara.Add(new ParameterDefine("@MaKho", nvlNhapXuatItemShowcrr.MaKho));
                dieukienshow += Environment.NewLine + string.Format("Kho: {0}", nvlNhapXuatItemShowcrr.MaKho);
            }
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.LyDo))
            {
                dieukien += "  and LyDo = @LyDo";
                lstpara.Add(new ParameterDefine("@LyDo", nvlNhapXuatItemShowcrr.LyDo));
                dieukienshow += Environment.NewLine + string.Format("Kho: {0}", nvlNhapXuatItemShowcrr.LyDo.ToString());
            }
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.NhaMay))
            {
                dieukien += "  and NhaMay = @NhaMay";
                lstpara.Add(new ParameterDefine("@NhaMay", nvlNhapXuatItemShowcrr.NhaMay));
                dieukienshow += Environment.NewLine + string.Format("Nhà máy: {0}", nvlNhapXuatItemShowcrr.NhaMay);
            }
            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.TenGN))
            {
                dieukien += "  and MaGN = @MaGN";
                lstpara.Add(new ParameterDefine("@MaGN", nvlNhapXuatItemShowcrr.TenGN));
                //dieukienshow += Environment.NewLine + string.Format("Nơi giao nhận: {0}", txtMaKho.Text);
            }

            if (!string.IsNullOrEmpty(nvlNhapXuatItemShowcrr.MaHang))
            {
                dieukienmahang += " where MaHang = @MaHang";
                lstpara.Add(new ParameterDefine("@MaHang", nvlNhapXuatItemShowcrr.MaHang));
                dieukienshow += Environment.NewLine + string.Format("Mã hàng: {0}", nvlNhapXuatItemShowcrr.MaHang);
            }


            string sqlSearch = string.Format(@"use NVLDB 

                                    declare @tblnx table(Serial int primary key,Ngay date,LyDo nvarchar(100))
                                    insert into @tblnx(Serial,Ngay,LyDo)
                                    select Serial,Ngay,LyDo from NvlNhapXuat {0} 
                                     and MaKho<>N'K011'

                                    declare @tbltonkho table(MaHang nvarchar(100),DVT nvarchar(100),TenHang nvarchar(100),TenNhom nvarchar(100),SLTonDau decimal(18,6),SLTon decimal(18,6))


                                    insert into @tbltonkho(MaHang,DVT,TenHang,TenNhom,SLTonDau,SLTon)
                                    select hh.MaHang,hh.DVT,hh.TenHang,nh.TenNhom,qrynxkho.SLTonDau,qrynxkho.SLTon from
                                    (SELECT [MaHang],sum(case when nx.Ngay<@DateBegin then ([SLNhap]-SLXuat) else 0  end) as SLTonDau
                                    ,sum(SLNhap-SLXuat) as SLTon
                                              FROM [NvlNhapXuatItem] it
                                              inner join  @tblnx nx on it.SerialCT=nx.Serial
											
											  group by MaHang) as qrynxkho
											  inner join dbo.NvlHangHoa hh on qrynxkho.MaHang=hh.MaHang 
                                                inner join dbo.NvlNhomHang nh on hh.MaNhom=nh.MaNhom
												select * from @tbltonkho {2} order by MaHang

										select MaHang,SLNhap+SLXuat as SoLuong,LyDo
										from
										(SELECT [MaHang],sum([SLNhap]) as SLNhap,sum([SLXuat]) as SLXuat,case when SLNhap>0 then N'Nhap_'+ qryct.LyDo else N'Xuat_'+qryct.LyDo end as LyDo
                                              FROM [NvlNhapXuatItem] it
                                              inner join (select * from @tblnx  where Ngay>=@DateBegin)
                                              as qryct on it.SerialCT=qryct.Serial
                                              group by MaHang,case when SLNhap>0 then N'Nhap_'+ qryct.LyDo else N'Xuat_'+qryct.LyDo end ) as hh {2} order by MaHang", dieukien, sqlreplace, dieukienmahang);
          

            
            CallAPI callAPI = new CallAPI();

            try
            {
                DataTable dtresult = new DataTable();
                PanelVisible = true;
                lstheader.Clear();
                lstitem.Clear();
                string json = await callAPI.ExcuteQueryDatasetEncrypt(sqlSearch, lstpara);
                if (json != "")
                {
                    DataSet ds= JsonConvert.DeserializeObject<DataSet>(json);
                   
                    if (ds.Tables[0].Rows.Count==0)
                    {
                        toastService.Notify(new ToastMessage(ToastType.Danger, "Không có dữ liệu, vui lòng kiểm tra lại thông tin tìm kiếm"));
                        return;
                    }
                    DataTable dttonkho = ds.Tables[0];
                    DataTable dtlydo = ds.Tables[1];
                    var querycolumn = dtlydo.AsEnumerable().GroupBy(p => p.Field<string>("LyDo")).Select(p => new {LyDo=p.Key }).OrderBy(p => p.LyDo).ToList();
                    
                    //var querygroupngay = lst.GroupBy(p => p.GroupName).Select(p => new { Ngaydatetime = convertdatetime(p.Key.ToString()) }).ToList();  

                    foreach (var it in querycolumn)
                    {
                        string[] arr = it.LyDo.Split('_');
                        lstitem.Add(new itemnhapxuat(it.LyDo, arr[1]));
                        dttonkho.Columns.Add(it.LyDo, typeof(double));
                     
                    }
                    //Xử lý hiển thị text ở Band
                    lstheader.Add(new itemnhapxuat("Nhap_", "HOẠT ĐỘNG NHẬP"));
                    lstheader.Add(new itemnhapxuat("Xuat_", "HOẠT ĐỘNG XUẤT"));
                    //renderFragmentcolumn = CreateColumns(lstheader.Where(p=>p.value=="Nhap_").ToList(), lstitem.Where(p=>p.value.StartsWith("Nhap_")).ToList());
                    renderFragmentcolumn = CreateColumns(lstheader, lstitem);
                    bool check = false;
                    int indexbegin = 0;
                    int k = dttonkho.Rows.Count;
                    string MaHang = "";

                    foreach (DataRow it in dtlydo.Rows)//Tìm kiếm 2 mảng đã được sắp thứ tự theo mã hàng
                    {
                        check = false;
                        MaHang = it.Field<string>("MaHang");
                        for (int i = indexbegin; i < k; i++)
                        {
                          
                            if (MaHang == dttonkho.Rows[i].Field<string>("MaHang"))
                            {
                                if (!check)
                                {
                                    indexbegin = i;//Đánh dấu phần tử đầu tiên
                                    check = true;
                                    dttonkho.Rows[indexbegin][it.Field<string>("LyDo")] = it["SoLuong"];
                                }
                            }
                            else
                            {
                                if (check)//Đã tìm thấy ở phía trên rồi, do mảng sắp thứ tự, mà xuống đây lại không thấy thì ngắt luôn
                                {
                                    break;
                                }
                            }
                        }
                    }
                    dtsource = dttonkho;
                    querycolumn.Clear();
                    

                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                toastService.Notify(new ToastMessage(ToastType.Danger, "Lỗi :" + ex.Message));
            }
            finally
            {
                PanelVisible = false;
                dxGrid.Reload();

                StateHasChanged();
                await _jsRuntime.InvokeVoidAsync("hideCollapse", string.Format("#{0}", randomdivhide));
                //CallJson callJson = new CallJson(_jsRuntime);

                //await callJson.CollapseDiv("divshowhide");
            }
        }
    }
}
