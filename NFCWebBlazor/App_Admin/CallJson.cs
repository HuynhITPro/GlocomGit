using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace NFCWebBlazor.App_Admin
{
    public class CallJson
    {
        IJSRuntime _jsRuntime { get; set; }
        public CallJson(IJSRuntime jsRuntime) => _jsRuntime = jsRuntime;
        public async Task ToggleDiv(string div_id)
        {
            await _jsRuntime.InvokeVoidAsync("bootstrap.Collapse.toggle", div_id);
        }

        public async Task CollapseDiv(string div_id)
        {
            await _jsRuntime.InvokeVoidAsync("bootstrap.Collapse.hide", div_id);
        }

        public async Task ExpandDiv(string div_id)
        {
            await _jsRuntime.InvokeVoidAsync("bootstrap.Collapse.show", div_id);
        }
    }
    public enum CallNameJson
    {
        toggleCollapse,
        hideCollapse,
        showCollapse
    }
}
