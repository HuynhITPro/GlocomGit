using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using NFCWebBlazor.App_ModelClass;

namespace NFCWebBlazor.App_ClassDefine
{
    public class ComboboxFilteredString: DxComboBox<DataDropDownList, string>
    {
        [Parameter]
        public string TypeName { get; set; }

        public List<DataDropDownList> lst { get; set; } = new List<DataDropDownList>();
        protected override async Task OnInitializedAsync()
        {
            //this.ListRenderMode = ListRenderMode.Virtual;
            //this.FilteringMode = DataGridFilteringMode.Contains;
            //this.ClearButtonDisplayMode = DataEditorClearButtonDisplayMode.Auto;
            if (TypeName != null)
            {
                lst = await NFCWebBlazor.Model.ModelData.GetDataDropDownListsAsync(TypeName);// Model.ModelData.GetDataDropDownListsAsync();
                BeginUpdate();
                    this.Data = lst;
                EndUpdate();
            }

            // return base.OnInitializedAsync();
        }
        public void setList(List<DataDropDownList> lstset)
        {
            lst = lstset;
        }
        public string SetValue
        {
            set
            {
                if (this.Data != null)
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
                        this.Value = lst[_value].Name;
                        this.EndUpdate();
                    }

                }
            }
        }
        string? _selectedValue { get; set; }
        public DataDropDownList? SelectedValue(string value)
        {
            return this.Data.FirstOrDefault(p => p.Name.Equals(value));

        }
    }
}
