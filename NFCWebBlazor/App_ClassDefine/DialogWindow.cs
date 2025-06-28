using DevExpress.Blazor;
using DevExpress.CodeParser;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;

namespace NFCWebBlazor.App_ClassDefine
{
    public class DialogWindow:DxPopup
    {

        [Inject] NavigationManager Navigation { get;set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject] PopupService PopupService { get; set; }
        [Parameter]
        public EventCallback CloseCallback { get; set; }

        //private DotNetObjectReference<DialogWindow>? dotNetRef;

     

        string _title;

        private string PopupId { get; set; } = Guid.NewGuid().ToString(); // ID duy nhất cho popup này
        public async Task showAsync(string title)
        {
            _title = title;
            HeaderText = _title;

            CloseOnOutsideClick = false;
            ShowHeader = true;
            AllowResize = true;
            ShowCloseButton = true;

            CloseOnEscape = false;

            //dotNetRef = DotNetObjectReference.Create(this);

            Navigation.NavigateTo("#");
            PopupService.OpenPopup(PopupId);
            PopupService.OnClosePopup += HandlePopupClose;

            
            //await JSRuntime.InvokeVoidAsync("PopupHelper.registerBackEvent", dotNetRef);

        }
     
        private void HandlePopupClose(string popupId)
        {
            if (popupId == PopupId)
            {
               
                // Logic để đóng popup
                Console.WriteLine($"Đóng popup {PopupId}");
                this.CloseAsync();
            }
        }
        public void Dispose()
        {
            PopupService.OnClosePopup -= HandlePopupClose;
        }
        private void InitBody(RenderFragment componentType)
        {
            this.BodyContentTemplate = context =>
            {
                return componentType;
            };

        }

      
        private void ShowPopup()
        {
            // Hiển thị popup và đăng ký với PopupService
            PopupService.OpenPopup(PopupId);
        }

     
    }
}
