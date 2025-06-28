using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using NFCWebBlazor.App_ModelClass;
using NFCWebBlazor.Model;

namespace NFCWebBlazor.App_ClassDefine
{
    public class ComboboxFilteredTable:DxComboBox<DataDropDownList,DataDropDownList>
    {
        [Parameter]
        public string TypeName { get; set; }
        
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
        public string SetValue
        {
            set
            {
                if (lst != null)
                {
                    this.BeginUpdate();
                    this.Value = lst.FirstOrDefault(p => p.Name.Equals(value));
                    this.EndUpdate();
                }
                //this.Value=
            }
        }
        public DataDropDownList GetValue
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
                if(lst!= null)
                {
                    int _value = value;
                    if(_value>lst.Count-1)
                        _value = lst.Count-1;
                    if (_value >= 0)
                    {
                        this.BeginUpdate();
                        this.Value = lst[_value];
                        this.EndUpdate();
                    }

                }
            }
        }
        string? _selectedValue { get; set; }
        public DataDropDownList? SelectedValue(string value)
        {
            return lst.FirstOrDefault(p => p.Name.Equals(value)); 
          
        }
       
         

    }
}
