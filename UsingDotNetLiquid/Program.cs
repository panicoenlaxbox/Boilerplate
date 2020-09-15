using DotLiquid;
using DotLiquid.FileSystems;
using System;
using System.Collections.Generic;
using System.IO;

namespace UsingDotNetLiquid
{
    class Program
    {
        static void Main(string[] args)
        {
            // Register filters globally
            Template.RegisterFilter(typeof(MyFilter));
            Template.RegisterFilter(typeof(YourFilter));

            Template.RegisterTag<MyTag>("awesomeTag");
            Template.RegisterTag<MyBlock>("myCustomBlock");

            // You can use types that don't inherit from Drop
            Template.RegisterSafeType(typeof(Bar), new[] { "Baz" });

            // https://github.com/dotliquid/dotliquid/wiki/DotLiquid-for-Designers#includes
            Template.FileSystem = new LocalFileSystem(Directory.GetCurrentDirectory());

            const string source = @"
hi {{ name | my }}, 
hi {{ name | your_great }}, 
{% include ""partial_template"" %}
{{ foo.bar }}
{{ foo.unknown }}
{{ bar.baz }}
{{ baz.qux }}
{{ qux.quux_corge }}
{% awesomeTag naruto %}
{% myCustomBlock 44 %} inside my custom block {% endmyCustomBlock %}";

            // The Parse step creates a fully compiled template which can be re-used. 
            // You can store it in memory or in a cache for faster rendering later.
            Template template = Template.Parse(source);
            var obj = Hash.FromAnonymousObject(
                new
                {
                    name = "tobi",
                    foo = new FooDrop(new Foo() { Bar = "bar" }),
                    bar = new Bar() { Baz = "baz" },
                    baz = new Baz() { Qux = "qux" },
                    qux = new Qux() { QuuxCorge = "QuuxCorge" }
                });

            // All parameters you want to use in your DotLiquid templates have to be passed as parameters to the Render method.
            string result = template.Render(obj);

            Console.WriteLine(result);
            /*
hi my tobi,
hi your great tobi,
Hi from partial template, bar
Hi from other partial template, bar
bar
method: unknown
baz
qux
QuuxCorge
An awesome tag naruto
my-block inside my custom block
             */
        }
    }

    public static class MyFilter
    {
        public static string My(Context context, string input)
        {
            return $"my {input}";
        }
    }

    // snake_case, your_great
    public static class YourFilter
    {
        public static string YourGreat(Context context, string input)
        {
            return $"your great {input}";
        }
    }

    public class MyTag : Tag
    {
        private string _markup;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            // tagName = awesomeTag
            // markup = naruto
            base.Initialize(tagName, markup, tokens);
            _markup = markup;
        }

        public override void Render(Context context, TextWriter result)
        {
            result.Write($"An awesome tag {_markup}");
        }
    }

    public class MyBlock : Block
    {
        private int _arg;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            // tagName = myCustomBlock
            // markup = 44
            base.Initialize(tagName, markup, tokens);
            _arg = Convert.ToInt32(markup);
        }

        public override void Render(Context context, TextWriter result)
        {
            result.Write("my-block");
            base.Render(context, result);
        }
    }

    # region "rules-for-template-rendering-parameters"

    // https://github.com/dotliquid/dotliquid/wiki/DotLiquid-for-Developers#rules-for-template-rendering-parameters    

    public class Foo
    {
        public string Bar { get; set; }
    }

    public class FooDrop : Drop
    {
        private readonly Foo _foo;

        public FooDrop(Foo foo)
        {
            _foo = foo;
        }

        public string Bar => _foo.Bar;

        public override object BeforeMethod(string method)
        {
            // method = "unknown
            return $"method: {method}";
        }
    }

    public class Bar
    {
        public string Baz { get; set; }
    }

    public class Baz : ILiquidizable
    {
        public string Qux { get; set; }
        public object ToLiquid()
        {
            // https://github.com/dotliquid/dotliquid/issues/29#issuecomment-901655
            return Hash.FromAnonymousObject(new { Qux });
        }
    }

    [LiquidType(nameof(QuuxCorge))]
    public class Qux
    {
        public string QuuxCorge { get; set; }
    }

    # endregion
}
