(function () {
    'use strict';

    angular.module("logDashboard", [
            "ngRoute"
        ])
        .config([
            '$routeProvider', function($routeProvider) {

                $routeProvider
                    .when('/',
                    {
                        controller: 'logDashboardController',
                        templateUrl: '/app/views/logDashboard.html',
                    })
                    .otherwise({ redirectTo: '/' });
            }
        ]);
})();