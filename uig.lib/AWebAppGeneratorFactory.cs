using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace uig.lib
{
    public abstract class AWebAppGeneratorFactory
    {
        public Func<string, string, List<Tuple<string, string>>, HtmlElement> IndexPageBuilder =
            (app, outPutFolder, views) =>
            {
                var body = IndexPageBuilderIn();
                body.Add(new HtmlElement("script", null, new List<HtmlElement>
                {
                    new HtmlElement(app)
                }));

                views?.ForEach(x =>
                {
                    body.Add(new HtmlElement("script", new Dictionary<string, string>
                    {
                        {"type", "text/ng-template"},
                        {"id", x.Item1}
                    }, new List<HtmlElement>
                    {
                        new HtmlElement(x.Item2)
                    }));
                });


                var page = CreateWebSite(
                    new SourcePaths("js", "css", "images", "lib"),
                    new List<string> {"app.js"},
                    new List<string> {"app.css"},
                    body, "files", outPutFolder);

                return page;
            };

        protected AWebAppGeneratorFactory(string appName, List<AnNgComponent> components)
        {
            Components = components ?? new List<AnNgComponent>();
        }

        public List<AnNgComponent> Components { get; }

        protected static Func< List<HtmlElement>> IndexPageBuilderIn { set;
            get; }

        /// <summary>
        /// Ovveride this to update libraries etc
        /// </summary>
        protected static Func<SourcePaths, List<HtmlElement>, PageObject> GetPageObject = (paths, bodyContentElements) => new PageObject
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

        public List<string> GetFactories()
        {
            return Components.SelectMany(x => x.GetFactories()).ToList();
        }

        public List<string> GetServices()
        {
            return Components.SelectMany(x => x.GetServices()).ToList();
        }

        public List<string> GetDirectives()
        {
            return Components.SelectMany(x => x.GetDirectives()).ToList();
        }

        public List<string> GetControllers()
        {
            return Components.SelectMany(x => x.GetControllers()).ToList();
        }

        public List<Tuple<string, string>> GetViews()
        {
            return Components.SelectMany(x => x.GetViews()).ToList();
        }

        public List<NgStateObject> GetStates()
        {
            return Components.SelectMany(x => x.GetStates()).ToList();
        }

        protected static HtmlElement CreateWebSite(SourcePaths paths, List<string> appScripts, List<string> appStyles,
            List<HtmlElement> bodyContentElements, string sourceFolder, string outPutFolder)
        {
            var pageObj = GetPageObject(paths, bodyContentElements);

            appScripts?.ForEach(x => pageObj.BodyScripts.Add(paths.Scripts + "/" + x.TrimStart('/').TrimStart('\\')));
            appStyles?.ForEach(x => pageObj.BodyStyles.Add(paths.Styles + "/" + x.TrimStart('/').TrimStart('\\')));

            var filePaths = new List<string>();
            filePaths.AddRange(pageObj.BodyScripts);
            filePaths.AddRange(pageObj.HeadScripts);
            filePaths.AddRange(pageObj.HeadStyles);
            filePaths.AddRange(pageObj.BodyStyles);

            filePaths.ForEach(x =>
            {
                var path = outPutFolder + "/" + x;
                var dir = Path.GetDirectoryName(path);
                if (dir == null || !File.Exists(sourceFolder + "/" + x)) return;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                File.Copy(sourceFolder + "/" + x, path, true);
            });

            var page = CreateFullWebPagePage(pageObj);
            var indexPath = outPutFolder + "/" + "index.html";
            File.WriteAllText(indexPath, page.HtmlString());
            Process.Start(indexPath);

            return page;
        }


        protected static List<HtmlElement> CreateScripts(List<string> src)
        {
            return src.Select(x => new HtmlElement(HtmlElementTypes.Script, new Dictionary<string, string>
            {
                {HtmlAttributeTypes.Src, x}
            })).ToList();
        }

        protected static List<HtmlElement> CreateStyles(List<string> src)
        {
            return src.Select(x => new HtmlElement(true, HtmlElementTypes.Link, new Dictionary<string, string>
            {
                {HtmlAttributeTypes.Href, x},
                {HtmlAttributeTypes.Rel, "stylesheet"}
            })).ToList();
        }

        protected static List<HtmlElement> CreateMeta(List<Dictionary<string, string>> meta)
        {
            return meta.Select(x => new HtmlElement(true, HtmlElementTypes.Meta, x)).ToList();
        }


        protected static HtmlElement CreatePageHead(List<string> styles, List<string> scripts,
            List<Dictionary<string, string>> meta)
        {
            var head = CreateStyles(styles);
            head.AddRange(CreateScripts(scripts));
            head.AddRange(CreateMeta(meta));
            return new HtmlElement(HtmlElementTypes.Head, null, head);
        }

        protected static HtmlElement CreatePageBody(List<string> styles, List<string> scripts,
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

        protected static HtmlElement CreatePage(HtmlElement head, HtmlElement body)
        {
            return new HtmlElement(HtmlElementTypes.Html, new Dictionary<string, string> {{"lang", "en"}},
                new List<HtmlElement>
                {
                    head,
                    body
                }, HtmlConstants.Html5Html);
        }

        protected static HtmlElement CreateFullWebPagePage(PageObject pageObject)
        {
            var head = CreatePageHead(pageObject.HeadStyles, pageObject.HeadScripts, pageObject.HeadMeta);
            var body = CreatePageBody(pageObject.BodyStyles, pageObject.BodyScripts, pageObject.BodyContents);
            return CreatePage(head, body);
        }
    }
}