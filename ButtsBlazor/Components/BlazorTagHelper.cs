using System.Globalization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ButtsBlazor.Server.Components
{

    [HtmlTargetElement("blazor-page")]
    public class BlazorRazorPageTagHelper(
    //IClientStateProvider blazorStateProvider
    ) : PersistComponentStateTagHelper
    {
        public string? ApplicationBasePath { get; set; }
        public string ContentPathBase { get; set; } = "";
        public bool IncludeBlazor { get; set; } = true;
        public bool AutostartBlazor { get; set; } = true;
        public string BlazorScriptPath { get; set; } = "_framework/blazor.webassembly.js";
        public bool IncludeTelerik { get; set; } = false;
        public string TelerikScriptPath { get; set; } = "_content/Telerik.UI.for.Blazor/js/telerik-blazor.js";
        public required Assembly? ApplicationAssembly { get; set; }
        public bool IncludeApplicationCss { get; set; } = true;
        public List<string> AdditionalScripts { get; set; } = new List<string>();
        public List<string> AdditionalModules { get; set; } = new List<string>();
        public List<string> AdditionalScriptFiles { get; set; } = new List<string>();
        public List<string> AdditionalCssFiles { get; set; } = new List<string>();
        public IDictionary<string, object> State { get; set; } = new Dictionary<string, object>();
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var basePath = ApplicationBasePath ?? throw new ArgumentException(nameof(ApplicationBasePath));
            ViewContext.AddHeadContent($@"<base href=""{basePath}"" />", false);
            //if (!ViewContext.HttpContext.Request.IsAjaxRequest())
            //{
            //    ViewContext.AddHeadContent($@"<base href=""{basePath}"" />", false);
            //}
            //else
            //{
            //    ViewContext.AppendScriptBlock(
            //        $"""
            //            let h = $('head');
            //            let b = h.children('base'); 
            //            if(b.length == 0)
            //               h.prepend('<base href="{basePath}">');
            //            else
            //               b.attr('href', {basePath});
            //        """);
            //}

            if (IncludeBlazor)
            {
                if (AutostartBlazor)
                    AddScriptFile(BlazorScriptPath, false);
                else
                {
                    AddScriptFile(BlazorScriptPath, false, new() { { "autostart", "false" } });
                    ViewContext.AppendScriptBlock("""
                                                 document.addEventListener("DOMContentLoaded", function () {
                                                    Blazor.start();
                                                 });
                                              """);
                }
            }

            foreach (var module in AdditionalModules)
                ViewContext.AppendScript($"""<script type="module">${module}</script>""", true);
            if (IncludeTelerik)
            {
                AddScriptFile(TelerikScriptPath);
                AddCssFile("_content/Telerik.UI.for.Blazor/css/kendo-theme-default/all.css");
            }
            if (IncludeApplicationCss && ApplicationAssembly != null)
                AddCssFile($"{ApplicationAssembly.GetName().Name}.styles.css");
            foreach (var css in AdditionalCssFiles)
                AddCssFile(css);

            foreach (var script in AdditionalScriptFiles)
                AddScriptFile(script);
            foreach (var script in AdditionalScripts)
                ViewContext.AppendScript(ContentPath(script));


            //        blazorStateProvider.SetState(clientConfig);
            //        foreach (var state in State)
            //            blazorStateProvider.SetState(new(state.Value.GetType(), state.Key), state.Value);
            return base.ProcessAsync(context, output);
        }

        private void AddScriptFile(string contentPath, bool defer = true, Dictionary<string, object?>? attributes = null) =>
            ViewContext.AddScriptFile(ContentPath(contentPath), defer, attributes);
        private void AddCssFile(string contentPath) =>
            ViewContext.AddCssFile(ContentPath(contentPath));

        private string ContentPath(string filePath)
        {
            if (string.IsNullOrEmpty(ContentPathBase))
                return filePath;
            var separator = "";
            if (!ContentPathBase.EndsWith("/"))
                separator = "/";

            return $"{ContentPathBase}{separator}{filePath}";
        }
    }


    public static class PageScriptExtensions
    {
        public const string ScriptType = "text/javascript";
        private static string Attr(string attr, string? value) => string.IsNullOrEmpty(value) ? "" : $"{attr}='{value}'";
        private static string BeginScript(string type, string[] baseAttributes, params string[] additionalAttributes) => $"<script type={type} {string.Join(" ", baseAttributes)} {string.Join(" ", additionalAttributes)}>";
        private static string BeginScript(string[]? baseAttributes = null, params string[] additionalAttributes) =>
            BeginScript(ScriptType, baseAttributes ?? Array.Empty<string>(), additionalAttributes);

        private static string BeginScript(string? id, params string[] attributes) =>
            BeginScript([Attr("id", id)], @attributes);

        private static string BeginScript(string? id, string? type, params string[] attributes) =>
        BeginScript(!string.IsNullOrWhiteSpace(type) ? type : ScriptType, [Attr("id", id)], @attributes);

        public const string EndScript = "</script>";
        public static readonly IHtmlContent EndScriptContent = new HtmlString(EndScript);
        public static string ScriptFile(string src, bool defer, params string[] attributes) => BeginScript([Attr("src", src), defer ? "defer" : ""], attributes) + EndScript;

        public static string CssFile(string href) =>
            $"""<link rel="stylesheet" href="{href}" />""";
        private static string Script(string begin, string? script) => begin + (script ?? "") + EndScript;
        //public static IHtmlContent ScriptBlock(this IHtmlHelper @this, string type, string id, IHtmlContent script) =>
        //    new EnumerableHtmlContent(new HtmlString(BeginScript(id,type)),script,EndScriptContent);
        public static HtmlString ScriptBlock(this IHtmlHelper @this, string type, string? id, string script) =>
            new HtmlString(Script(BeginScript(id, type), script));
        //public static HtmlString ScriptBlock(this IHtmlHelper @this, string type, Func<HtmlString> script) =>
        //    new HtmlString(Script(BeginScript(type),script?.Invoke()?.ToHtmlString()));
        //public static HtmlString ScriptBlock(this IHtmlHelper @this, Func<HtmlString> script) =>
        //    new HtmlString(Script(BeginScript(),script?.Invoke().ToHtmlString()));
        public static HtmlString BeginScriptBlock(this IHtmlHelper @this) =>
            new HtmlString(BeginScript());

        public static ViewContext BeginScriptBlock(this ViewContext @this)
        {
            @this.Writer.Write(BeginScript());
            return @this;
        }

        public static void AppendScriptBlock(this ViewContext @this, string script, string? id = null) =>
            @this.AppendScript(Script(BeginScript(id), script));

        public static HtmlString EndScriptBlock(this IHtmlHelper @this) =>
            new HtmlString(EndScript);

        public static ViewContext EndScriptBlock(this ViewContext @this)
        {
            @this.Writer.Write(EndScript);
            return @this;
        }

        public static IHtmlContent RenderHeadContent(this IHtmlHelper htmlHelper)
        {
            if (htmlHelper.ViewContext.HttpContext.Items.TryGetValue(ScriptContext.ScriptContextItem, out var scriptContextObj) &&
                scriptContextObj is ScriptContext scriptContext)
                return new HtmlString(scriptContext.ToHeadContentString());

            return HtmlString.Empty;
        }

        public static IHtmlContent RenderScripts(this IHtmlHelper htmlHelper)
        {
            if (htmlHelper.ViewContext.HttpContext.Items.TryGetValue(ScriptContext.ScriptContextItem, out var scriptContextObj) &&
                    scriptContextObj is ScriptContext scriptContext)
                return new HtmlString(scriptContext.ToScriptString());

            return HtmlString.Empty;
        }

        internal static ScriptContext GlobalScriptContext(this ViewContext @this) =>
            @this.HttpContext.Items.TryGetValue(ScriptContext.ScriptContextItem, out var val) && val is ScriptContext sc ? sc :
                (ScriptContext)(@this.HttpContext.Items[ScriptContext.ScriptContextItem] = new ScriptContext(@this.HttpContext, @this.Writer));

        public static void AddHeadContent(this ViewContext @this, string headContent, bool renderOnAjax) =>
            @this.GlobalScriptContext().AddHeadContent(headContent, renderOnAjax);
        public static void AppendScript(this ViewContext @this, string script, bool renderOnAjax = true) =>
            @this.GlobalScriptContext().AddScriptBlock(script, renderOnAjax);
        public static void AddScriptFile(this ViewContext @this, string src, bool defer, IDictionary<string, object?>? attributes = null, bool renderOnAjax = true)
        {
            var script = ScriptFile(src, defer, attributes?.ToAttributeString() ?? "");
            if (defer)
                @this.AddHeadContent(script, renderOnAjax);
            else
                @this.AppendScript(script);
        }

        public static void AddCssFile(this ViewContext @this, string src, bool renderOnAjax = true) =>
            @this.AddHeadContent(CssFile(src), renderOnAjax);

        public static void AddScriptFile(this IHtmlHelper @this, string src, bool renderOnAjax) =>
            @this.ViewContext.AddScriptFile(src, renderOnAjax);

        public static void AppendScript(this IHtmlHelper @this, string script, bool renderOnAjax = true) =>
            @this.ViewContext.AppendScript(script, renderOnAjax);
        public static void WriteScriptBlock(this ViewContext @this, string script) =>
            @this.Writer.Write(Script(BeginScript(), script));
        public static string ToAttributeString<TValue>(this IDictionary<string, TValue> instance)
        {
            StringBuilder stringBuilder = new StringBuilder();
            HtmlEncoder htmlEncoder = HtmlEncoder.Default;
            foreach (KeyValuePair<string, TValue> keyValuePair in instance)
                stringBuilder.Append(
                    string.Format(CultureInfo.CurrentCulture, " {0}=\"{1}\"", htmlEncoder.Encode(keyValuePair.Key), htmlEncoder.Encode(keyValuePair.Value?.ToString() ?? "")));
            return stringBuilder.ToString();
        }
    }

    /// <summary>
    /// A context in which to add references to script files and blocks of script
    /// to be rendered to the view at a later point.
    /// </summary>
    public class ScriptContext
    {
        internal const string ScriptContextItem = "__ScriptContext__v1";
        private readonly TextWriter _writer;
        private readonly bool _isAjaxRequest;
        internal readonly IList<string> ScriptBlocks = new List<string>();
        internal readonly IList<string> HeadBlocks = new List<string>();
        private readonly HttpContext _httpContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScriptContext" /> class.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="writer"></param>
        /// <exception cref="ArgumentNullException">httpContext</exception>
        public ScriptContext(HttpContext httpContext, TextWriter writer)
        {
            _httpContext = httpContext;
            _writer = writer;
            _isAjaxRequest = false;
        }


        /// <summary>
        ///     Adds a block of script to be rendered out at a later point in the page rendering when
        ///     <see cref="ScriptHtmlHelperExtensions.RenderScripts(HtmlHelper)" /> is called.
        /// </summary>
        /// <param name="scriptBlock">the block of script to render. The block must not include the &lt;script&gt; tags</param>
        /// <param name="renderOnAjax">
        ///     if set to <c>true</c> and the request is an AJAX request, the script will be written in the response.
        /// </param>
        /// <remarks>
        ///     A call to <see cref="ScriptHtmlHelperExtensions.RenderScripts(HtmlHelper)" /> will render all scripts.
        /// </remarks>
        public void AddScriptBlock(string scriptBlock, bool renderOnAjax = false, bool addScriptTags = false)
        {
            if (!_isAjaxRequest || renderOnAjax)
            {
                if (addScriptTags)
                    scriptBlock = "<script type='text/javascript'>" + scriptBlock + "</script>";
                ScriptBlocks.Add(scriptBlock);
            }
        }
        public void AddHeadContent(string headContent, bool renderOnAjax)
        {
            if (!_isAjaxRequest || renderOnAjax)
                HeadBlocks.Add(headContent);
        }


        /// <summary>
        ///     Adds a block of script to be rendered out at a later point in the page rendering when
        ///     <see cref="ScriptHtmlHelperExtensions.RenderScripts(HtmlHelper)" /> is called.
        /// </summary>
        /// <param name="scriptTemplate">
        ///     the template for the block of script to render. The template must include the &lt;script
        ///     &gt; tags
        /// </param>
        /// <param name="renderOnAjax">
        ///     if set to <c>true</c> and the request is an AJAX request, the script will be written in the response.
        /// </param>
        /// <remarks>
        ///     A call to <see cref="ScriptHtmlHelperExtensions.RenderScripts(HtmlHelper)" /> will render all scripts.
        /// </remarks>
        public void AddScriptBlock(Func<dynamic?, HelperResult> scriptTemplate, bool renderOnAjax = false)
        {
            if (_isAjaxRequest)
            {
                if (renderOnAjax)
                {
                    var str = scriptTemplate(null).ToString();
                    if (str != null) ScriptBlocks.Add(str);
                }
            }
            else
            {
                var str = scriptTemplate(null).ToString();
                if (str != null) ScriptBlocks.Add(str);
            }
        }
        public string ToScriptString() => string.Join(Environment.NewLine, ScriptBlocks);
        public string ToHeadContentString() => string.Join(Environment.NewLine, HeadBlocks);
    }

}
