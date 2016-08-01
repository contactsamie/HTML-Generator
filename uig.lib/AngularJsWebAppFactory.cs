using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace uig.lib
{
    public class AngularJsWebAppFactory
    {
       
        public static SampleResult GenerateWebApp(AWebAppGeneratorFactory appFactory,string outPutFolder, string appName ,string defaultRoute )
        {
            
            var services = appFactory.GetServices();
            var factories = appFactory.GetFactories();
            var directives = appFactory.GetDirectives();
            var controllers = appFactory.GetControllers();
            var views = appFactory.GetViews();
            var states = appFactory.GetStates();

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
            },appFactory.IndexPageBuilder);

            return result;
        }

        
        private static SampleResult GenerateAngularApp(AppDefinitionObject appDefinitionObject , Func<string, string, List<Tuple<string, string>>, HtmlElement> indexPageBuilder)
        {
            var app = BuildAppCode(appDefinitionObject.States, appDefinitionObject.AppName, appDefinitionObject.DefaultRoute, appDefinitionObject.Components);
          var page=  indexPageBuilder(app, appDefinitionObject.OutPutFolder, appDefinitionObject.Views);

            var json = JsonConvert.SerializeObject(page, Formatting.Indented);
            var html = page.HtmlString();
            var result = new SampleResult
            {
                HtmlObject = page,
                HtmlJson = json,
                HtmlString = html,
                AppDefinitionObject = appDefinitionObject,
                AppDefinitionObjectJson = JsonConvert.SerializeObject(appDefinitionObject, Formatting.Indented)
            };
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