angular.module('occ.common_services').service('serviceRequests', function Orderdetailservice($timeout,EndPoints, MockEndPoints, $http, $q, $rootScope, ngDialog, lodash, $state, $compile, Utility, $interval) {
    $rootScope.allCurrentHttpPromises = $rootScope.allCurrentHttpPromises || [];
    var context = this;
 

    this.POST = function(url, item) {
        var deferred = $q.defer();
        var load = JSON.stringify(item);
        $http.post(url, load, {
                headers: {
                    'Content-Type': 'application/json'
                }
            }).
            success(deferred.resolve).
            error(deferred.reject);
        $rootScope.allCurrentHttpPromises.push(deferred.promise);
        return EndPoints.test ? MockEndPoints.resolve(url) : deferred.promise;

    };
    this.GET = function(url) {

        var deferred = $q.defer();
        $http({
                method: 'GET',
                url: url
            }).
            success(deferred.resolve).
            error(deferred.reject);
        $rootScope.allCurrentHttpPromises.push(deferred.promise);
        return EndPoints.test ? MockEndPoints.resolve(url) : deferred.promise;
    };
    this.loadPayeeList = function(take, skip) {
        var query = EndPoints.GetPayees + "?take=" + (take || 100) + "&skip=" + (skip || 0);
        return context.GET(query);
    };
    this.searchPayeeList = function(searchString, take, skip) {

        //var query = EndPoints.searchPayees + "/" + searchString + "?take=" + (take || 100) + "&skip=" + (skip || 0);
        //return context.GET(query);

        return context.POST(EndPoints.searchPayees, {
            Skip: skip || 0,
            Take: take || 100,
            SearchString: searchString
        });


    };
    //this.searchPaymentList = function(searchString, take, skip,options) {
    //    return context.POST(EndPoints.searchPayments, {
    //        skip: skip,
    //        take: take,
    //        orderBy:options&& options.orderBy,
    //        descending: options && options.descending,
    //        dueThisWeek: options && options.dueThisWeek,
    //        loadAllShows:options&&options.loadAllShows,
    //        dueToday: options && options.dueToday,
    //        pastDueDate: options && options.pastDueDate,

    //        dueThisMonth: options && options.dueThisMonth,
    //        dueNextTwoWeeks: options && options.dueNextTwoWeeks,
    //        notPastDueDate: options && options.notPastDueDate,
    //        reduce: true,
    //        search: searchString
    //    });
    //};
    //this.strictSearchPaymentList = function(searchString, take, skip) {
    //    var query = EndPoints.strictSearchPayments + "/" + searchString + "?take=" + (take || 100) + "&skip=" + (skip || 0);
    //    return context.GET(query);
    //};
    //this.loadPaymentList = function(take, skip) {

    //    var query = EndPoints.GetLiveEventPaymentsForListView + "?take=" + (take || 100) + "&skip=" + (skip || 0) + "&reduce=true";
    //    return context.GET(query);
    //};
    var methodOnSaveChanges = function(confirm) {
        var plugin = methodOnSaveChanges.plugin || {};

        plugin.onSaveChanges.state = plugin.onSaveChanges.state || {};
        plugin.onSaveChanges.validate = plugin.onSaveChanges.validate || function() {
            return true;
        };
        var validationMessage = plugin.onSaveChanges.validate(plugin.onSaveChanges.data);
        var handleState = function(type) {
            if (lodash.isArray(plugin.onSaveChanges.state[type]) && plugin.onSaveChanges.state[type].length !== 0) {
                if (plugin.onSaveChanges.state[type][0] === true) {

                    if (plugin.onSaveChanges.state[type][1] === true) {


                        var backParams = plugin.onSaveChanges.state[type][2] || false;


                        var params = (backParams && lodash.isObject(backParams) && $rootScope.previousState_params) ? lodash.assign($rootScope.previousState_params, backParams) : $rootScope.previousState_params;
                        $state.go($rootScope.previousState, params);
                    } else {
                        window.location.reload();
                    }


                } else {
                    if (plugin.onSaveChanges.state[type].length > 1) {
                        $state.go(plugin.onSaveChanges.state[type][0], plugin.onSaveChanges.state[type][1]);

                    }

                    if (plugin.onSaveChanges.state[type].length === 1) {
                        $state.go(plugin.onSaveChanges.state[type][0]);
                    }
                }
            }

        };
        if (validationMessage !== true) {
            $rootScope.alertError(validationMessage || 'There are validation errors');
            return false;
        } else {
            if (plugin.onSaveChanges.confirmationMessage) {
                var user = confirm(plugin.onSaveChanges.confirmationMessage || "Are you sure?", plugin.onSaveChanges.confirmationTitle, plugin.onSaveChanges.confirmationIcon);
            } else {

                var promise = methodOnSaveChanges.that[plugin.onSaveChanges.method || 'POST'](plugin.onSaveChanges.url, plugin.onSaveChanges.data);
                context.promiseHandler(promise, function(response) {

                    $rootScope.alertSuccess(plugin.onSaveChanges.showSuccessResponse ? response : 'Operation completed successfully');

                    handleState('success');
                }, function(response) {

                    $rootScope.alertError(plugin.onSaveChanges.showErrorResponse ? response : 'Operation failed');
                    handleState('error');
                });

                return false;
            }

        }


//returns buttons
        return {
            Ok: function() {
                var promised = methodOnSaveChanges.that[plugin.onSaveChanges.method || 'POST'](plugin.onSaveChanges.url, plugin.onSaveChanges.data);
                context.promiseHandler(promised, function(response) {
                    user.success(plugin.onSaveChanges.showSuccessResponse ? response : 'Operation completed successfully');
                    handleState('success');
                }, function(response) {
                    user.error(plugin.onSaveChanges.showErrorResponse ? response : 'Operation failed!');
                    handleState('error');
                });
            },
            Cancel: function(data) {

            }
        };
    };
    this.promiseHandler = function(promise, success, failure,preventProgress) {

        var progressTemplate = '<div id="serviceProgress" class="do-not-print"><div  id="ajax-loader-progress"></div><div  class="ajax-loader-data-common do-not-print" id="ajax-loader-data-center"><div  class="text-success do-not-print">' +
            'Processing ...'
            + '</div><span><i style="font-size:45px" class="fa fa-cog fa-spin do-not-print"></i></span>&nbsp;&nbsp;<strong style="color:#fff" class="do-not-print"></strong></div></div>';

        preventProgress||  $('body').append(progressTemplate);
        promise.then(function(response) {
            preventProgress || $('#serviceProgress').remove();
            console.info("promiseHandler: Request succeeded");
            console.log(response);
            (typeof success === 'function') && success(response);

        }, function(response) {
            preventProgress || $('#serviceProgress').remove();
            console.info("promiseHandler: Request failed");
            console.error(response);
            (typeof failure === 'function') && failure(response);

        });
    };
    //stateTransition:
    //if 1=true reload, if 2 is true go to previous state
    //if 1 is false check type[0] and go to the speciied state, if type[1] exists go to the stte with the parameter 
    //if type is not specified, dont do anything
    /*

   { success: [true] }  // on success just reload
   { success: [true,true] }  // on success just reload to previous state
   { success: [false] }  // on success just stay on the page
   { success: ['paymentlist'] }  // on success just go to paymentlist route
   { success: ['viewpayment',{id:22}] }  // on success just go to viewpayment route with param 22
     
     if message is not supplied, it goes straight to action without confimation
    */
    this.saveChanges = function(message, data, url, validate, stateTransition, afterChangesSaved) {
        $rootScope.cancelPreviousAlerts();
        var onSaveChanges = false;
        var internalPlugin = {};
        if (url) {
            internalPlugin = {
                onSaveChanges: {
                    validate: validate || function() { return true; },
                    url: url,
                    confirmationMessage: message,
                    data: data,
                    showErrorResponse: true,
                    showSuccessResponse: false,
                    state: stateTransition || {
                        success: [],
                        error: []
                    }
                }
            };

        } else {
            internalPlugin = message;
        }
        var that = this;
        if (internalPlugin) {

            if (lodash.isObject(internalPlugin.onSaveChanges) && typeof internalPlugin.onSaveChanges !== 'function') {
                onSaveChanges = methodOnSaveChanges;
                onSaveChanges.plugin = internalPlugin;
                onSaveChanges.that = that;
            } else if (typeof internalPlugin.onSaveChanges === 'function') {
                onSaveChanges = internalPlugin.onSaveChanges;
            }


            if (typeof onSaveChanges === 'function') {
                var resultObject = {};
                var curentMessage = "";
                var curentTitle = "";
                var curentIconClass = "";
                var validationAction = function(msg, title, iconClass) {
                    curentMessage = msg || 'Are you sure?';
                    curentTitle = title || 'Confirmation';
                    curentIconClass = iconClass || 'fa fa-info-circle';
                    resultObject = {
                        success: function(m, showDialog, preventToast) {
                            internalPlugin.successes = [];
                            internalPlugin.successes.push(m || 'Operation completed successfully');
                            preventToast || $rootScope.alertSuccess(m || 'Operation completed successfully');
                            showDialog && ngDialog.open({
                                template: m || 'Operation completed successfully',
                                plain: true
                            });
                            internalPlugin.edit = false;
                            afterChangesSaved && afterChangesSaved(true, m || 'Operation completed successfully');
                        },
                        error: function(m, showDialog, preventToast) {
                            internalPlugin.errors = [];
                            internalPlugin.errors.push(m || 'Operation failed');
                            preventToast || $rootScope.alertError(m || 'Operation failed');
                            showDialog && ngDialog.open({
                                template: m || 'Operation failed',
                                plain: true
                            });

                            afterChangesSaved && afterChangesSaved(false, m || 'Operation failed');
                        }
                    };
                    return resultObject;
                };
                var handler = onSaveChanges(validationAction);
                var buttonTemplates = '';

                if (handler) {
                    lodash.forIn(handler, function(value, key) {
                        if (typeof value === 'function') {

                            var winFunctionName = 'ng_dialog_' + key;
                            var winFunctionHandlerName = 'ng_dialog_handle_' + key;

                            window[winFunctionName] = value;
                            window[winFunctionHandlerName] = function() {
                                window[winFunctionName]();
                            };
                            buttonTemplates = buttonTemplates + '&nbsp;&nbsp;' + '<input type="button" class="pop-confimation-button  " value="' + key + '" response="yes" onclick="' + winFunctionHandlerName + '()"  ng-click="closeThisDialog()"/>';
                        }
                    });

                    ngDialog.open({
                        template: '<div class="row"><div class="col-sm-12"><i class="fa ' + curentIconClass + '  "></i><strong>&nbsp;' + curentTitle + '</strong></div></div><div class="row"><div class="col-sm-offset-1"> </div><div class="col-sm-12"> <p>' + curentMessage + '</p>' + '<div  style="text-align:right">' + buttonTemplates + '</div></div> </div>',
                        plain: true,
                        className: 'ngdialog-theme-default',
                        showClose: internalPlugin.showClose || true,
                        closeByDocument: internalPlugin.closeByDocument || false,
                        closeByEscape: internalPlugin.closeByEscape || false
                    });
                }


            } else {
                console.warn("onSaveChanges not well defined");
            }

        } else {
            console.warn('save changes not found for model ');
        }

    };

    this.ocEasyGrid = function(param) {
         
        param = param || {};
        var definition = {
            scope: {},
            polling: 0,
            columnOrder:[],
            tableId: 'listtable',
            routeName: '',
            loadHandler: function () { },
            searchHandler: function () { },
            searchShortCuts: {},
            dataPreprocessor: function (data) { return data; },
            buildHtmlTableEvents: {},
            excludeColumnsFromView: [],
            types: {},
            tableStyleClass: '',
            tableHeaderStyleClass: '',
            element: {},
            actions: {}
        };
        param = lodash.assign(definition, param);
        param.scope.$tableStyleClass = param.tableStyleClass;
        param.scope.$tableHeaderStyleClass = param.tableHeaderStyleClass;

        param.scope.$searchShortCuts = param.searchShortCuts;
        var refresh = function (preventProgress,preventForceRefresh) {

            param.scope.currentPage = parseInt($state.params.page || 1, 10);
            param.scope.searchString = $state.params.search;
            param.scope.take = 50;
            param.scope.skip = 0;
           


            param.scope.totalPages = param.scope.totalPages || 1;
            var take = param.scope.take;
            var skip = (param.scope.currentPage * take) - take;
              $state.params = $state.params || {};
              var loadPromise = (param.scope.searchString || $state.params.orderBy || $state.params.descending || $state.params.dueThisWeek || $state.params.loadAllShows || $state.params.dueToday || $state.params.pastDueDate || $state.params.notPastDueDate || $state.params.dueNextTwoWeeks || $state.params.dueThisMonth) ? param.searchHandler(param.scope.searchString, take, skip, {
                orderBy:   $state.params.orderBy,
                descending: $state.params.descending,
                dueThisWeek: $state.params.dueThisWeek,
                loadAllShows:$state.params.loadAllShows,
                dueToday: $state.params.dueToday,
                pastDueDate: $state.params.pastDueDate,

                dueThisMonth: $state.params.dueThisMonth,
                dueNextTwoWeeks: $state.params.dueNextTwoWeeks,
                notPastDueDate: $state.params.notPastDueDate,

            }) : param.loadHandler(take, skip);

            context.promiseHandler(loadPromise, function (response) {
                var shouldRefresh = true;
                if (preventForceRefresh) {
                    shouldRefresh = !lodash.isEqual(response, param.scope.lastResponse);

                }

                if (shouldRefresh) {
                param.scope.lastResponse = response;
                // $rootScope.alertSuccess(' loaded successfully');
                param.scope.totalPages = Math.ceil(response.total / param.scope.take);

                param.scope.paginationText = response.total ? 'Page ' + param.scope.currentPage + ' of ' + param.scope.totalPages + ' [' + (param.scope.take > response.total ? response.total : param.scope.take) + ' of ' + response.total + ']' : '';

                var flat = param.dataPreprocessor(response.data) || response.data;

                var data = Utility.buildHtmlTable(flat, false, param.buildHtmlTableEvents || {}, param.excludeColumnsFromView, param.types, param.actions,param.columnOrder, param.actions);
                param.scope.routeName = param.routeName;
                param.scope.data = data;
                }
               
            }, function (data) {
                $rootScope.alertInfo('Unable to load data');
            }, preventProgress);
        };
        refresh();
        var template = "<oc-list id='" + param.tableId + "'></oc-list>";
        var linkFn = $compile(template);
        var content = linkFn(param.scope);
        param.element.append(content);

        param.polling && $interval(function () { refresh(true,true); }, param.polling);

        return {
            refresh: refresh,
            remove: function() {
                $('oc-list#' + param.tableId).remove();
            }
        };

    };

    this.printPageCurrentPage = function(onCompleted) {

       // $rootScope.alertInfo('Preparing print. Please wait ....');
      
        $timeout(function () {
           
            window.document.close();
            window.focus();
            window.print();
            window.close();
            window.onfocus = function() {
                window.close();
                window.document.close();
            };
            $rootScope.alertSuccess('Completed');
           typeof onCompleted ==="function" && onCompleted();
        },100);
    };

    this.printPageSource = function (url, onCompleted) {
        $("#printer").size() && $("#printer").remove();
        $timeout(function () {
            $("<div id='printer'>").hide().appendTo("body");

            $timeout(function() {
                
            $("<iframe>").hide().attr("src",url).appendTo("#printer");
            $('#printer iframe').load(function () {
                $rootScope.alertSuccess('Print view loaded');
                typeof onCompleted === "function" && onCompleted();
            });

            },1000);

        }, 0);
    };
});