using System.Collections.Generic;

namespace uig.lib
{
    public class PageObject
    {
        public List<string> HeadStyles { set; get; }
        public List<string> HeadScripts { set; get; }
        public List<Dictionary<string, string>> HeadMeta { set; get; }
        public List<string> BodyStyles { set; get; }
        public List<string> BodyScripts { set; get; }

        public List<HtmlElement> BodyContents { set; get; }
    }
}