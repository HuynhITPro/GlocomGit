using Microsoft.AspNetCore.Components;
using NFCWebBlazor.App_Admin;
using NFCWebBlazor.App_ModelClass;
using static NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi.Page_KeHoachMuaHangMaster;

namespace NFCWebBlazor.App_NguyenVatLieu.App_KeHoachDeNghi
{
    public partial class View_KeHoachMuaHangItem
    {
        [Parameter]
        public KeHoachMuaHang_Show kehoachmuahangshow { get; set; }
        [Parameter]
        public EventCallback<KeHoachMuaHang_Show> ShowFlyChucNang { get; set; }
        bool IsOpenfly { get; set; } = false;
        string img { get; set; } = IconImg.User;
        private async void btclickshowmodel()
        {
            bool bl = await dialogMsg.Show("Header test", "Body test");
            if (bl)
            {
                Console.WriteLine("Nó true rồi");
            }
            else
            {
                Console.WriteLine("Nó false rồi");
            }

        }
        bool shouldRender { get; set; } = true;
        //protected override bool ShouldRender()
        //{
        //    if (shouldRender)
        //    {

        //        shouldRender = false;
        //        return true;
        //    }
        //    //Console.WriteLine("Should render:" + kehoachmuahangshow.PathDuyet);
        //    return shouldRender;
        //}
       
       
        private void showmsg()
        {
            msgBox.Show("Xin chào chú ", IconMsg.iconerror);
        }
        private void ShowFlyout()
        {
            string s = "";
            //CurrentEmployee = employee;
            ShowFlyChucNang.InvokeAsync(kehoachmuahangshow);
        }
    }
}
