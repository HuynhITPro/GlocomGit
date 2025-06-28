using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using NFCWebBlazor.App_ModelClass;
using System.Collections.Generic;
using System.Text;

namespace NFCWebBlazor.App_ClassDefine
{
    public class DxTagBoxDefine:DxTagBox<DataDropDownList,DataDropDownList>
    {
        [Parameter]
        public string? TypeName { get; set; }
       

        private string _value { get; set; }
        public string GetText 
        { get { return getStringSelected(); }  
        }
       
        public string SetSelectedValue
        {
            set
            {
                if (this.Data == null)
                    return;
                _value= value;
                if (!String.IsNullOrEmpty(_value))
                {
                    string[] arr = _value.Split(';');
                    this.BeginUpdate();
                        this.Values = arr.Where(p=>!string.IsNullOrEmpty(p)).GroupBy(p => p).Select(p => new DataDropDownList { Name = p.Key.ToString(), FullName = p.Key.ToString(), TypeName = TypeName });
                    this.EndUpdate();
                }
            }
           
        }
        public List<DataDropDownList> lst { get; set; } = new List<DataDropDownList>();
        protected override async Task OnInitializedAsync()
        {
            if (TypeName != null)
            {
                lst = await NFCWebBlazor.Model.ModelData.GetDataDropDownListsAsync(TypeName);// Model.ModelData.GetDataDropDownListsAsync();
                this.Data = lst;
            }
            // return base.OnInitializedAsync();
        }
      
        private IEnumerable<DataDropDownList> setSelected(string value)
        {
            IEnumerable<DataDropDownList> lstselected=new List<DataDropDownList>();
            if(!String.IsNullOrEmpty(value))
            {
                string[] arr=value.Split(';');
                lstselected = arr.GroupBy(p => p).Select(p => new DataDropDownList { Name = p.Key.ToString(), FullName = p.Key.ToString(), TypeName = TypeName });

            }
            return lstselected;
        }
      
        private string getStringSelected()
        {
            if (this.Values == null)
                return "";

            StringBuilder stringBuilder= new StringBuilder();
            foreach(var it in this.Values)
            {
                stringBuilder.Append(it.Name.ToString()+";");
            }
           
            return stringBuilder.ToString();
           
        }

    }
}
