using System.Collections.Generic;

namespace uig.lib
{
    public class SampleWebAppGeneratorFactory : AWebAppGeneratorFactory
    {
        public SampleWebAppGeneratorFactory(string appName, List<AnNgComponent> components) : base(appName, components)
        {
            IndexPageBuilderIn = () =>
            {
                var body = new List<HtmlElement>
                {
                    new HtmlElement("div", new Dictionary<string, string>
                    {
                        {"ui-view", " "}
                    })
                };
                return body;
            };
        }
    }
}