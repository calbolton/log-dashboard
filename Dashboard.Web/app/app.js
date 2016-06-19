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
        ])
        .directive('modalShow', ['$parse', modalShow]);

    function modalShow($parse) {
        return {
            restrict: "A",
            link: function (scope, element, attrs) {

                //Hide or show the modal
                scope.showModal = function (visible, elem) {
                    if (!elem)
                        elem = element;

                    if (visible)
                        $(elem).modal("show");
                    else
                        $(elem).modal("hide");
                    console.log("SHOWING|HIDING MODAL");
                }

                //Watch for changes to the modal-visible attribute
                scope.$watch(attrs.modalShow, function (newValue) {
                    scope.showModal(newValue, attrs.$$element);
                });

                //Update the visible value when the dialog is closed through UI actions (Ok, cancel, etc.)
                $(element).bind("hide.bs.modal", function () {
                    if ($parse(attrs.modalShow).assign) {
                        $parse(attrs.modalShow).assign(scope, false);
                    }

                    scope.$emit('modal.closed.outside', element);

                    if (!scope.$$phase && !scope.$root.$$phase)
                        scope.$apply();
                });
            }
        };
    };;
})();