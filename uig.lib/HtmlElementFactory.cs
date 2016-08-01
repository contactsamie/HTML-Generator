using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static HtmlElement CreateBasicFullHtmlPage(SourcePaths paths, List<string> appScripts, List<string> appStyles, List<HtmlElement> bodyContentElements,string sourceFolder,string outPutFolder)
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
                if (dir == null || !File.Exists(sourceFolder + "/" + x)) return;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
               
                File.Copy(sourceFolder + "/" + x, path,true);
            });

            var page = CreateFullWebPagePage(pageObj);
            var indexPath = outPutFolder + "/" + "index.html";
            File.WriteAllText(indexPath, page.HtmlString());
            Process.Start(indexPath);

            return page;
        }



     
        public static SampleResult GenerateSampleResult(string app,  string outPutFolder, List<Tuple<string, string>> views)
        {
            var body = new List<HtmlElement>()
                       {
                            new HtmlElement("h1", null, null, "Hello"),
                            new HtmlElement("script",null,new List<HtmlElement>()
                            {
                                new HtmlElement(app)
                            }),
                             new HtmlElement("div", new Dictionary<string, string>()
                             {
                                 { "ui-view"," "}
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
                       new SourcePaths("js", "css", "images", "lib"),
                       new List<string>() { "app.js" },
                       new List<string>() { "app.css" },
                       body,"files", outPutFolder);

            var json = JsonConvert.SerializeObject(page, Formatting.Indented);
            var html = page.HtmlString();
            return new SampleResult()
            {
                HtmlObject = page,
                HtmlJson = json,
                HtmlString = html
            };
        }
       
        public static SampleResult GenerateSampleResult(string outPutFolder)
        {
            var appName = "myapp";
            var defaultRoute = "/";

            var moduleHandle = @"angular.module('" + appName + @"')";
          
            var services = new List<string>{};
            var factories = new List<string> { };
            var directives = new List<string> {
                moduleHandle +@".directive('dynamic', function ($compile) {
                                  return {
                                    restrict: 'A',
                                    replace: true,
                                    link: function (scope, ele, attrs) {
                                      scope.$watch(attrs.dynamic, function(html) {
                                        ele.html(html);
                                        $compile(ele.contents())(scope);
                                      });
                                    }
                                  };
                                });" };
            var controllers = new List<string>
            {

                    moduleHandle+@".controller('PageCtrl', function($scope) { 
                                              console.log('ready in controller');
                                              $scope.values=[1,2];
                                              $scope.click = function(arg) {
                                                alert('Clicked ' + arg);
                                              }
                                              $scope.html = '<a class=""btn btn-primary"" ng-click=""click(value)"" >Click me {{value}}</a>'  
                                         });
                                        "
            };
            var views=new List<Tuple<string,string>>()
            {
                new Tuple<string, string>("views/sample.html",
                 @"
                                        <ul>
                                            <li ng-repeat='value in values'>
                                                    <a>
                                                    <div dynamic='html'></div>
                                                </a>
                                            </li>
                                        </ul>"
                )
            };

            var states = new List<NgStateObject>()
            {
                new NgStateObject()
                {
                    Controller="PageCtrl",
                    Route = "/list?page" ,
                    TemplateUrl = "views/sample.html",
                    StateName = "list",
                    ControllerCode="",
                    TemplateString = ""
                }
                                           
            };

           var components = new List<string>();
            components.AddRange(services);
            components.AddRange(directives);
            components.AddRange(controllers);
            components.AddRange(factories);

         
            var result = GenerateAngularApp(new AppDefinitionObject()
            {
                OutPutFolder = outPutFolder,
                States = states,
                AppName = appName,
                DefaultRoute = defaultRoute,
                Components = components,
                Views = views
            });

            return result;
        }


      

        private static SampleResult GenerateAngularApp(AppDefinitionObject appDefinitionObject)
        {
            var app = BuildAppCode(appDefinitionObject.States, appDefinitionObject.AppName, appDefinitionObject.DefaultRoute, appDefinitionObject.Components);
            var result = HtmlElementFactory.GenerateSampleResult(app, appDefinitionObject.OutPutFolder, appDefinitionObject.Views);
            result.AppDefinitionObject = appDefinitionObject;
            result.AppDefinitionObjectJson = JsonConvert.SerializeObject(appDefinitionObject, Formatting.Indented);
            return result;
        }

        private static string BuildAppCode(List<NgStateObject> states, string appName, string defaultRoute, List<string> services)
        {
            //views
            var stateString = "";
            var controllerCodes = "";
            states.ForEach(x =>
            {
                stateString += @"
                 .state('" + x.StateName + @"', {
                    url: '" + x.Route + @"',
		            templateUrl: '" + x.TemplateUrl + @"'
                    " + (string.IsNullOrEmpty(x.TemplateString) ? "" : ",template: '" + x.TemplateString + "'") + @"
                    " + (string.IsNullOrEmpty(x.Controller) ? "" : ",controller: '" + x.Controller + "'") + @"
                 })";

                controllerCodes += x.ControllerCode;
            });

            var servicesString = string.Join(" ", services);
            var modules = new List<string>() {"ui.router"};
            var modulesString = string.Join("", modules);

            var moduleDefinitionCode = @"angular.module('" + appName + @"', [ '" + modulesString + @"']);";
            var moduleHandle = @"angular.module('" + appName + @"')";
            var initRunCode = moduleHandle + @".run(function($rootScope, $state) {  console.log('ready in run'); });";

            var configCode =
                moduleHandle + @".config(function($stateProvider, $urlRouterProvider, $httpProvider) {

                startupRoute = '" + defaultRoute + @"';
	            $urlRouterProvider.otherwise(startupRoute);
	            $httpProvider.defaults.useXDomain = true;
                delete $httpProvider.defaults.headers.common['X-Requested-With'];

	            $stateProvider" + stateString + @";

	            $httpProvider.defaults.useXDomain = true;
                delete $httpProvider.defaults.headers.common['X-Requested-With'];
                })
                ";

            var manualBoostrapingCode = @"
                 //manual bootstrap process 
                 angular.element(document).ready(function () { angular.bootstrap(document, ['" + appName + @"']); });
                ";
            var app = moduleDefinitionCode + configCode + initRunCode + servicesString + controllerCodes + manualBoostrapingCode;
            return app;
        }
    }
}