using System;
using System.Collections.Generic;

namespace uig.lib
{
    public class SampleNgComponent : AnNgComponent
    {
        public override List<string> GetFactories()
        {
            return new List<string> {
                Module +@".factory('myfactory', function () {
                                  return {};
                                });" };
        }

        public override List<string> GetServices()
        {
            return new List<string> {
                Module +@".service('myservice', function (myfactory) {
                                  this.myself= {};
                                });" };
        }

        public override List<string> GetDirectives()
        {
            return new List<string> {
                Module +@".directive('dynamic', function ($compile) {
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
        }

        public override List<string> GetControllers()
        {
            return new List<string>
            {

                Module+@".controller('PageCtrl', function($scope) { 
                                              console.log('ready in controller');
                                              $scope.values=[1,2];
                                              $scope.click = function(arg) {
                                                alert('Clicked ' + arg);
                                              }
                                              $scope.html = '<a class=""btn btn-primary"" ng-click=""click(value)"" >Click me {{value}}</a>'  
                                         });
                                        "
            };
        }

        public override List<Tuple<string, string>> GetViews()
        {
            return new List<Tuple<string, string>>()
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
        }

        public override List<NgStateObject> GetStates()
        {
            return new List<NgStateObject>()
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
        }

        public SampleNgComponent(string appName) : base(appName)
        {
        }
    }
}