using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using uig.lib;

namespace uig.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var app =
           @"
angular

.module('myapp', [ 'ui.router'])

.config(function($stateProvider, $urlRouterProvider, $httpProvider) {

                startupRoute = '/';
	            $urlRouterProvider.otherwise(startupRoute);
	            $httpProvider.defaults.useXDomain = true;
                delete $httpProvider.defaults.headers.common['X-Requested-With'];

	            $stateProvider.state('list', {
                    url: '/list?page&search&orderBy&descending&dueThisWeek&dueToday&pastDueDate&loadAllShows&dueThisMonth&dueNextTwoWeeks&notPastDueDate',
		            templateUrl: 'views/pages/_list.html'
                 });

	            $httpProvider.defaults.useXDomain = true;
                delete $httpProvider.defaults.headers.common['X-Requested-With'];

                })

.run(function($rootScope, $state) {  })

.controller('MyCtrl', function($scope) {  });

";
            var views=new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("sample.html",
                                            @"
                                             <ul>
                                                    <li ng-repeat='value in values'>
                                                    <a ng-click='doSomething({id:value.id})'>
                                                                        {{value.name}}
                                                    </a>
                                                    </li>
                                                </ul>
                                            ")
            };
            var result=     HtmlElementFactory.GenerateSampleResult(app,views);
        }
    }
}
