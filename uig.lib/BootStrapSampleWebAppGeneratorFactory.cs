using System.Collections.Generic;

namespace uig.lib
{
    public class BootStrapSampleWebAppGeneratorFactory : AWebAppGeneratorFactory
    {
        public BootStrapSampleWebAppGeneratorFactory(string appName, List<AnNgComponent> components)
            : base(appName, components)
        {
            IndexPageBuilderIn = () =>
            {
                var body = new List<HtmlElement>
                {
                    new HtmlElement("nav", new Dictionary<string, string>
                    {
                        {"class", "navbar navbar-default navbar-fixed-top"}
                    }, new List<HtmlElement>
                    {
                        new HtmlElement("div", new Dictionary<string, string>
                        {
                            {"class", "container"}
                        }, new List<HtmlElement>
                        {
                            new HtmlElement("div", new Dictionary<string, string>
                            {
                                {"class", "navbar-header"}
                            }, new List<HtmlElement>
                            {
                                new HtmlElement("button", new Dictionary<string, string>
                                {
                                    {"type", "button"},
                                    {"class", "navbar-toggle collapsed"},
                                    {"data-toggle", "collapse"}
                                })
                            }),
                            new HtmlElement("div", new Dictionary<string, string>
                            {
                                {"id", "navbar"},
                                {"class", "collapse navbar-collapse"}
                            }, new List<HtmlElement>
                            {
                                new HtmlElement("ul", new Dictionary<string, string>
                                {
                                    {"class", "nav navbar-nav"}
                                }, new List<HtmlElement>
                                {
                                    new HtmlElement("li", new Dictionary<string, string>(), new List<HtmlElement>
                                    {
                                        new HtmlElement("a", new Dictionary<string, string>(), new List<HtmlElement>
                                        {
                                            new HtmlElement("Home")
                                        })
                                    }),
                                    new HtmlElement("li", new Dictionary<string, string>(), new List<HtmlElement>
                                    {
                                        new HtmlElement("a", new Dictionary<string, string>(), new List<HtmlElement>
                                        {
                                            new HtmlElement("About")
                                        })
                                    })
                                })
                            })
                        })
                    }),
                    new HtmlElement("div", new Dictionary<string, string>
                    {
                        {"class", "container"}
                    }, new List<HtmlElement>
                    {
                        new HtmlElement("div", new Dictionary<string, string>
                        {
                            {"ui-view", " "}
                        })
                    })
                };
                return body;
            };

            //to modify meta etc
            // GetPageObject =
        }
    }
}