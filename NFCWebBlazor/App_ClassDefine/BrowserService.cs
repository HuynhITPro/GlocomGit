using Microsoft.JSInterop;
using System.Threading.Tasks;
namespace NFCWebBlazor.App_ClassDefine
{
   

    public class BrowserService
    {
        private readonly IJSRuntime _js;

        public BrowserService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<BrowserDimension> GetDimensions()
        {
            return await _js.InvokeAsync<BrowserDimension>("getDimensions");
        }
        public async Task<int> GetHeighWithID(string id)
        {
            return await _js.InvokeAsync<int>("elementFunctions.getHeightById",id);
        }

    }

    public class BrowserDimension
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
