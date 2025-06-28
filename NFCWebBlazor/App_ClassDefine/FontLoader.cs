using DevExpress.Drawing;

namespace NFCWebBlazor.App_ClassDefine
{
    public static class FontLoader
    {
        private static List<string> lstfontloaded = new List<string>();
        public async static Task LoadFonts(HttpClient httpClient, List<string> fontNames)
        {

            foreach (var fontName in fontNames)
            {
                var query=lstfontloaded.FirstOrDefault(p=>p.Equals(fontName));
                if(query != null)
                {
                    return;
                }
                var fontBytes = await httpClient.GetByteArrayAsync($"fonts/{fontName}");
                DXFontRepository.Instance.AddFont(fontBytes);
                lstfontloaded.Add(fontName);
            }
        }
    }
}
