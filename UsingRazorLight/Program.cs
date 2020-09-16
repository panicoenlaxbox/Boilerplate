using RazorLight;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UsingRazorLight
{
    class Program
    {
        static async Task Main(string[] args)
        {            
            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(Program))
                .UseMemoryCachingProvider()
                .Build();

            var template = "Hello, @Model.Bar";
            var model = new Foo { Bar = "Bar" };

            // Each template must have a templateKey that is associated with it, so you can render the same template next time without recompilation
            const string key = "templateKey";

            string result = await engine.CompileRenderStringAsync(key, template, model);

            Console.WriteLine(result); // Hello, Bar

            var cacheResult = engine.Handler.Cache.RetrieveTemplate(key);
            if (cacheResult.Success)
            {
                var templatePage = cacheResult.Template.TemplatePageFactory();
                result = await engine.RenderTemplateAsync(templatePage, new Foo() { Bar = "bar" });
                Console.WriteLine(result); // Hello, bar
            }

            await FileSourceTemplatesAsync();

            await EmbeddedResourceTemplatesAsync();
        }

        private async static Task FileSourceTemplatesAsync()
        {
            // https://github.com/toddams/RazorLight#file-source

            var engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(Directory.GetCurrentDirectory())
                .UseMemoryCachingProvider()
                .Build();

            var model = new { Name = "John Doe" };
            string result = await engine.CompileRenderAsync("a_view.cshtml", model);
            Console.WriteLine(result);
            // Hello John Doe from a file view
            // Other view for John Doe in a include
        }

        private async static Task EmbeddedResourceTemplatesAsync()
        {
            // https://github.com/toddams/RazorLight#embeddedresource-source

            var engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(Program))
                .UseMemoryCachingProvider()
                .Build();

            var model = new { Name = "John Doe" };
            string result = await engine.CompileRenderAsync("a_embedded_resource.cshtml", model);
            Console.WriteLine(result); // Hello John Doe from a embedded resource
        }
    }

    public class Foo
    {
        public string Bar { get; set; }
    }
}
