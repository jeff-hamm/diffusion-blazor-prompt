using System.ComponentModel;
using Microsoft.AspNetCore.Components.Web;

namespace ButtsBlazor.Server.Components
{
    public class NoPreRenderWebAssembly : InteractiveWebAssemblyRenderMode
    {
        public static readonly NoPreRenderWebAssembly Instance = new NoPreRenderWebAssembly();
        public NoPreRenderWebAssembly() : base(false)
        { }
    }
}
