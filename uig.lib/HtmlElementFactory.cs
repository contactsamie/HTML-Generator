using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace uig.lib
{

    public static class HtmlConstants
    {
        public static string Html5Html = "<!DOCTYPE html>";
    }
    public static class HtmlElementTypes
    {
        public static string Script = "script";
        public static string Link = "link";
        public static string Head = "head";
        public static string Body = "body";
        public static string Html = "html";
        public static string Meta = "meta";
    }
    public static class HtmlAttributeTypes
    {
        public static string Class = "class";
        public static string Src = "src";
        public static string Href = "href";
        public static string Name = "name";
        public static string Content = "content";
        public static string Rel = "rel";
    }

    public class HtmlElementFactory
    {
        public static List<HtmlElement> CreateScripts(List<string> src)
        {
            return src.Select(x => new HtmlElement(HtmlElementTypes.Script, new Dictionary<string, string>
            {
                {HtmlAttributeTypes.Src, x}
            })).ToList();
        }

        public static List<HtmlElement> CreateStyles(List<string> src)
        {
            return src.Select(x => new HtmlElement(true, HtmlElementTypes.Link, new Dictionary<string, string>
            {
                {HtmlAttributeTypes.Href, x},
                {HtmlAttributeTypes.Rel, "stylesheet"}
            })).ToList();
        }

        public static List<HtmlElement> CreateMeta(List<Dictionary<string, string>> meta)
        {
            return meta.Select(x => new HtmlElement(true, HtmlElementTypes.Meta, x)).ToList();
        }


        public static HtmlElement CreatePageHead(List<string> styles, List<string> scripts,
            List<Dictionary<string, string>> meta)
        {
            var head = CreateStyles(styles);
            head.AddRange(CreateScripts(scripts));
            head.AddRange(CreateMeta(meta));
            return new HtmlElement(HtmlElementTypes.Head, null, head);
        }

        public static HtmlElement CreatePageBody(List<string> styles, List<string> scripts,
            List<HtmlElement> bodyElements)
        {
            var body = CreateStyles(styles);
            body.AddRange(CreateScripts(scripts));
            if (bodyElements != null)
            {
                body.AddRange(bodyElements);
            }
            return new HtmlElement(HtmlElementTypes.Body, null, body);
        }

        public static HtmlElement CreatePage(HtmlElement head, HtmlElement body)
        {
            return new HtmlElement(HtmlElementTypes.Html, new Dictionary<string, string> { { "lang", "en" } }, new List<HtmlElement>
            {
                head,
                body
            }, HtmlConstants.Html5Html);
        }

        public static HtmlElement CreateFullWebPagePage(PageObject pageObject)
        {
            var head = CreatePageHead(pageObject.HeadStyles, pageObject.HeadScripts, pageObject.HeadMeta);
            var body = CreatePageBody(pageObject.BodyStyles, pageObject.BodyScripts, pageObject.BodyContents);
            return CreatePage(head, body);
        }

        public static HtmlElement CreateBasicFullHtmlPage(SourcePaths paths, List<string> appScripts, List<string> appStyles, List<HtmlElement> bodyContentElements,string outPutFolder)
        {
            var pageObj = new PageObject
            {
                BodyScripts = new List<string>
                {
                    paths.Libraries + "/jquery.min.js",
                    paths.Libraries + "/bootstrap.min.js",
                    paths.Libraries + "/ie10-viewport-bug-workaround.js",
                    paths.Libraries + "/angular.min.js",
                    paths.Libraries + "/angular-ui-router.js",
                    //  paths.Scripts + "/app.js"
                },
                BodyStyles = new List<string>(),
                BodyContents = bodyContentElements.ToList(),
                HeadMeta = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        {"http-equiv", "Content-Type"},
                        {HtmlAttributeTypes.Content, "text/html; charset=UTF-8"}
                    },
                    new Dictionary<string, string>
                    {
                        {"http-equiv", "X-UA-Compatible"},
                        {HtmlAttributeTypes.Content, "IE=edge"}
                    },
                    new Dictionary<string, string>
                    {
                        {HtmlAttributeTypes.Name, "viewport"},
                        {HtmlAttributeTypes.Content, "width=device-width, initial-scale=1"}
                    },
                    new Dictionary<string, string>
                    {
                        {HtmlAttributeTypes.Name, "description"},
                        {HtmlAttributeTypes.Content, ""}
                    },
                    new Dictionary<string, string>
                    {
                        {HtmlAttributeTypes.Name, "author"},
                        {HtmlAttributeTypes.Content, ""}
                    }
                },
                HeadScripts = new List<string>
                {
                    paths.Libraries + "/ie-emulation-modes-warning.js"
                },
                HeadStyles = new List<string>
                {
                    paths.Styles + "/bootstrap.min.css",
                    paths.Styles + "/ie10-viewport-bug-workaround.css",
                    paths.Styles + "/sticky-footer-navbar.css"
                }
            };

            appScripts?.ForEach(x => pageObj.BodyScripts.Add(paths.Scripts + "/" + x.TrimStart('/').TrimStart('\\')));
            appStyles?.ForEach(x => pageObj.BodyStyles.Add(paths.Styles + "/" + x.TrimStart('/').TrimStart('\\')));
            
            var filePaths=new List<string>();
            filePaths.AddRange(pageObj.BodyScripts);
            filePaths.AddRange(pageObj.HeadScripts);
            filePaths.AddRange(pageObj.HeadStyles);
            filePaths.AddRange(pageObj.BodyStyles);

            filePaths.ForEach(x =>
            {
                var path = outPutFolder + "/" + x;
                var dir = Path.GetDirectoryName(path);
                if (dir == null || !File.Exists(x)) return;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
               
                File.Copy(x,path,true);
            });

            var page = CreateFullWebPagePage(pageObj);
            File.WriteAllText(outPutFolder + "/" + "index.html", page.HtmlString());
            return page;
        }


        public static SampleResult GenerateSampleResult(string app, List<Tuple<string, string>> views)
        {
            var body = new List<HtmlElement>()
                       {
                            new HtmlElement("h1", null, null, "Hello"),
                            new HtmlElement("script",null,new List<HtmlElement>()
                            {
                                new HtmlElement(app)
                            })
                       };

            views?.ForEach(x =>
            {
                body.Add(new HtmlElement("script", new Dictionary<string, string>()
                {
                    { "type","text/ng-template"},
                    { "id",x.Item1}
                }, new List<HtmlElement>()
                {
                    new HtmlElement(x.Item2)
                }));
            });

            var page = HtmlElementFactory.CreateBasicFullHtmlPage(
                       new SourcePaths("files/js", "files/css", "files/images", "files/lib"),
                       new List<string>() { "app.js" },
                       new List<string>() { "app.css" },
                       body,System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"/!outputweb");

            var json = JsonConvert.SerializeObject(page, Formatting.Indented);
            var html = page.HtmlString();
            return new SampleResult()
            {
                HtmlObject = page,
                Json = json,
                Html = html
            };
        }
    }
}