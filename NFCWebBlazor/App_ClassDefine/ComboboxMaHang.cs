using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using NFCWebBlazor.App_ModelClass;

namespace NFCWebBlazor.App_ClassDefine
{
    public class ComboboxMaHang : DxComboBox<NvlHangHoaDropDown, string>
    {
       

        public List<NvlHangHoaDropDown> lst { get; set; } = new List<NvlHangHoaDropDown>();
        protected override async Task OnInitializedAsync()
        {
           
            lst = await NFCWebBlazor.Model.ModelData.GetHangHoa();// Model.ModelData.GetDataDropDownListsAsync();
            BeginUpdate();
                this.Data = lst;
                
            EndUpdate();
           
            //await InvokeAsync(StateHasChanged);
        }
       
        protected override void OnAfterRender(bool firstRender)
        {
            if(firstRender)
            {

               // Console.WriteLine("giá trị combobox MaHang: {0} tên hàng {1}", this.Value,this.Text);
               
               
                //Console.WriteLine(string.Format("List OnAfterRender là {0}:", lst.Count));
                // this.Data = lst;


            }
            
            //Console.WriteLine(string.Format("List OnAfterRender"));
            base.OnAfterRender(firstRender);
        }
        bool checkrendervalue = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
               
            }
            if (this.Value != null)
            {
                
                if (this.Text == null && lst != null)
                {
                    if (!checkrendervalue)
                    {
                        var query = lst.Where(p => p.MaHang.Equals(this.Value)).FirstOrDefault();
                        if (query != null)
                        {
                            BeginUpdate();
                            this.Text = query.TenHang;
                            EndUpdate();
                            // Console.WriteLine("giá trị combobox MaHang: {0} tên hàng {1}", this.Value, this.Text);
                        }
                        checkrendervalue = true;
                    }

                }
            }
            //base.OnAfterRender(firstRender);
        }
        public string SetValue
        {
            set
            {
                if (lst != null)
                {
                    this.BeginUpdate();
                    this.Value = value;
                    this.EndUpdate();
                }
                //this.Value=
            }
        }
        public string GetValue
        {
            get
            {
                return this.Value;
            }
        }
        public int SetIndexSelected
        {
            set
            {
                if (lst != null)
                {
                    int _value = value;
                    if (_value > lst.Count - 1)
                        _value = lst.Count - 1;
                    if (_value >= 0)
                    {
                        this.BeginUpdate();
                        this.Value = lst[_value].MaHang;
                        this.EndUpdate();
                    }

                }
            }
        }
        string? _selectedValue { get; set; }
        public NvlHangHoaDropDown getHangHoaSelected()
        {
            if (this.Value == null)
                return null;
            return lst.FirstOrDefault(p => p.MaHang.Equals(this.Value));
        }
        public NvlHangHoaDropDown? SelectedValue(string value)
        {
            return lst.FirstOrDefault(p => p.MaHang.Equals(value));

        }
    }
 
}
