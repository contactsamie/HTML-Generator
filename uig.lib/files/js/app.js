angular
.module("myapp", [ 'ui.router'])
.config(function ($stateProvider, $urlRouterProvider, $httpProvider) {
	startupRoute = '/';
	$urlRouterProvider.otherwise(startupRoute);

	$httpProvider.defaults.useXDomain = true;

	delete $httpProvider.defaults.headers.common['X-Requested-With'];

	$stateProvider.state('list', {
		url : '/list?page&search&orderBy&descending&dueThisWeek&dueToday&pastDueDate&loadAllShows&dueThisMonth&dueNextTwoWeeks&notPastDueDate',
		templateUrl : 'views/pages/_list.html'
	});

	$httpProvider.defaults.useXDomain = true;

	delete $httpProvider.defaults.headers.common['X-Requested-With'];

})
.run(function ($rootScope, $state) {
	})
.controller("MyCtrl", function ($scope) {
});
