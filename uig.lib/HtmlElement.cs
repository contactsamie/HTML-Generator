using System;
using System.Collections.Generic;
using System.Linq;

namespace uig.lib
{
    public class HtmlElement
    {

        public HtmlElement(string content)
        {
            Construct(false, null, null, null, content,"");
        }
        public HtmlElement(bool hasClosing, string name, Dictionary<string, string> attributes ,
            List<HtmlElement> htmlElements = null, string append = "", string prepend = "")
        {
            Construct(hasClosing, name, attributes, htmlElements, append, prepend);
        }

        public HtmlElement(string name, Dictionary<string, string> attributes ,
            List<HtmlElement> htmlElements = null, string append = "", string prepend = "")
        {
            Construct(false, name, attributes, htmlElements, append, prepend);
        }

        public string Append { set; get; }
        public string Prepend { set; get; }
        public bool HasNoClosing { set; get; }
        public string Name { private set; get; }
        public Dictionary<string, string> Attributes { private set; get; }

        public List<HtmlElement> HtmlElements { private set; get; }

        private void Construct(bool hasNoClosing, string name, Dictionary<string, string> attributes,
            List<HtmlElement> htmlElements, string append, string prepend)
        {
            Append = append ?? "";
            Prepend = prepend ?? "";
            HasNoClosing = hasNoClosing;
            Name = name;
            HtmlElements = htmlElements;
            Attributes = attributes ?? new Dictionary<string, string>();
        }

      

        public  string HtmlString()
        {var str = "";

            if (string.IsNullOrWhiteSpace(Name))
            {
                if (HtmlElements != null)
                {
                    str = str + string.Join("", HtmlElements.Select(x => x.HtmlString()));
                }
            }
            else
            {
 var attr = Attributes.Aggregate("", (current, keyValuePair) => current + " " + keyValuePair.Key + "=\"" + keyValuePair.Value + "\"");
            str = str + " <" + Name + "  " + attr + "  ";
            if (HasNoClosing)
            {
                str = str + "  />";
            }
            else
            {
                str = str + " > ";
                var ending = " </" + Name + ">";
                if (HtmlElements != null)
                {
                    str = str+ string.Join("", HtmlElements.Select(x => x.HtmlString())) ;
                }
                str = str + ending;
            }
            }


           
            str = Append + str + Prepend;
            return str;
        }
    }
}