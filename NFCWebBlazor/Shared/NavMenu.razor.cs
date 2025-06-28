using DevExpress.Blazor;
using DevExpress.Blazor.Navigation.Internal;
using DevExpress.DataAccess.DataFederation;
using Microsoft.AspNetCore.Components;
using NFCWebBlazor.Model;
using NFCWebBlazor.Models;
using System.ServiceModel.Channels;

namespace NFCWebBlazor.Shared
{
    public partial class NavMenu
    {
        [Parameter]
       public List<MenuItem> menuItems { get; set; }
        [Inject]ThemeColor themeColor { get; set; }
       [Inject]NavigationManager navigationManager { get; set; }
       DxTreeView treeView { get; set; }
        public bool shouldrender { get; set; } = true;
        protected override bool ShouldRender()
        {

            return shouldrender;
        }


      
        async void OnClick(TreeViewNodeClickEventArgs args)
        {
            var Node = args.NodeInfo;
            
            MenuItem menuItem = (MenuItem)Node.DataItem;
           if(!String.IsNullOrEmpty(menuItem.ComponentName))
            {
                await  GotoMainForm.InvokeAsync(menuItem);
                //navigationManager.NavigateTo(menuItem.NavigateUrl);
            }
           else
            {
                
            }
        }

    }
}
