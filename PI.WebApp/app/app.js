'use strict';

/**
 * @ngdoc overview
 * @name newappApp
 * @description
 * # newappApp
 *
 * Main module of the application.
 */
var MakeApp = angular
  .module('newApp', [
    'ngAnimate',
    'ngCookies',
    'ngResource',
    'ngRoute',
    'ngSanitize',
    'ngTouch',
    'ui.bootstrap',
    'ngStorage',
    'ngMessages',
    //'ngGrid',
    'smart-table',
    'ngCsv',
    'ngCsvImport',
    'customDirective',
    'angularjs-datetime-picker',
    'daterangepicker',
    'ngDialog',
    'ngFileUpload',
    'angular-jwt'
  ])
  .config(function ($routeProvider, $httpProvider, jwtInterceptorProvider) {
      $routeProvider
        //.when('/', {
        //    templateUrl: 'dashboard/dashboard.html',
        //    controller: 'dashboardCtrl'
        //})
        //.when('/frontend', {
        //    templateUrl: 'frontend/frontend.html',
        //    controller: 'frontendCtrl'
        //})
        //.when('/charts', {
        //    templateUrl: 'charts/charts/charts.html',
        //    controller: 'chartsCtrl'
        //})
        //.when('/financial-charts', {
        //    templateUrl: 'charts/financialCharts/financialCharts.html',
        //    controller: 'financialChartsCtrl'
        //})
        //.when('/ui-animations', {
        //    templateUrl: 'uiElements/animations/animations.html',
        //    controller: 'animationsCtrl'
        //})
        //.when('/ui-buttons', {
        //    templateUrl: 'uiElements/Buttons/buttons.html',
        //    controller: 'buttonsCtrl'
        //})
        //.when('/ui-components', {
        //    templateUrl: 'uiElements/components/components.html',
        //    controller: 'componentsCtrl'
        //})
        //.when('/ui-helperClasses', {
        //    templateUrl: 'uiElements/helperClasses/helperClasses.html',
        //    controller: 'helperClassesCtrl'
        //})
        //.when('/ui-icons', {
        //    templateUrl: 'uiElements/icons/icons.html',
        //    controller: 'iconsCtrl'
        //})
        //.when('/ui-modals', {
        //    templateUrl: 'uiElements/modals/modals.html',
        //    controller: 'modalsCtrl'
        //})
        //.when('/ui-nestableList', {
        //    templateUrl: 'uiElements/nestableList/nestableList.html',
        //    controller: 'nestableListCtrl'
        //})
        //.when('/ui-notifications', {
        //    templateUrl: 'uiElements/notifications/notifications.html',
        //    controller: 'notificationsCtrl'
        //})
        //.when('/ui-portlets', {
        //    templateUrl: 'uiElements/portlets/portlets.html',
        //    controller: 'portletsCtrl'
        //})
        //.when('/ui-tabs', {
        //    templateUrl: 'uiElements/Tabs/tabs.html',
        //    controller: 'tabsCtrl'
        //})
        //.when('/ui-treeView', {
        //    templateUrl: 'uiElements/treeView/treeView.html',
        //    controller: 'treeViewCtrl'
        //})
        //.when('/ui-typography', {
        //    templateUrl: 'uiElements/typography/typography.html',
        //    controller: 'typographyCtrl'
        //})
        //.when('/email-templates', {
        //    templateUrl: 'mailbox/mailbox-templates.html',
        //    controller: 'mailboxTemplatesCtrl'
        //})
        //  .when('/forms-elements', {
        //      templateUrl: 'forms/elements/elements.html',
        //      controller: 'elementsCtrl'
        //  })
        //     .when('/forms-validation', {
        //         templateUrl: 'forms/validation/validation.html',
        //         controller: 'elementsCtrl'
        //     })
        //    .when('/forms-plugins', {
        //        templateUrl: 'forms/plugins/plugins.html',
        //        controller: 'pluginsCtrl'
        //    })
        //  .when('/forms-wizard', {
        //      templateUrl: 'forms/wizard/wizard.html',
        //      controller: 'wizardCtrl'
        //  })
        //  .when('/forms-sliders', {
        //      templateUrl: 'forms/sliders/sliders.html',
        //      controller: 'slidersCtrl'
        //  })
        //  .when('/forms-editors', {
        //      templateUrl: 'forms/editors/editors.html',
        //      controller: 'editorsCtrl'
        //  })
        //    .when('/forms-input-masks', {
        //        templateUrl: 'forms/inputMasks/inputMasks.html',
        //        controller: 'inputMasksCtrl'
        //    })

        //medias
        //.when('/medias-croping', {
        //    templateUrl: 'medias/croping/croping.html',
        //    controller: 'cropingCtrl'
        //})
        //.when('/medias-hover', {
        //    templateUrl: 'medias/hover/hover.html',
        //    controller: 'hoverCtrl'
        //})
        //.when('/medias-sortable', {
        //    templateUrl: 'medias/sortable/sortable.html',
        //    controller: 'sortableCtrl'
        //})
        //  //pages
        //.when('/pages-blank', {
        //    templateUrl: 'pages/blank/blank.html',
        //    controller: 'blankCtrl'
        //})

        //profile Information Page
        .when('/profileInformation-profileInformation', {
            templateUrl: 'app/profileInformation/profileInformation.html',
            controller: 'profileInformationCtrl',
            controllerAs: 'profileCtrl'
        })
        .when('/saveAddress/:id', {
            templateUrl: 'app/addressBook/saveAddress.html',
            controller: 'saveAddressCtrl',
        })
        .when('/loadAddresses', {
            templateUrl: 'app/addressBook/loadAddresses.html',
            controller: 'loadAddressesCtrl',
        })
        //.when('/pages-contact', {
        //    templateUrl: 'pages/contact/contact.html',
        //    controller: 'contactCtrl'
        //})
        //.when('/pages-timeline', {
        //    templateUrl: 'pages/timeline/timeline.html',
        //    controller: 'timelineCtrl'
        //})
             //ecommerce
        //.when('/ecom-cart', {
        //    templateUrl: 'ecommerce/cart/cart.html',
        //    controller: 'cartCtrl'
        //})
        //.when('/ecom-invoice', {
        //    templateUrl: 'ecommerce/invoice/invoice.html',
        //    controller: 'invoiceCtrl'
        //})
        //.when('/ecom-pricingTable', {
        //    templateUrl: 'ecommerce/pricingTable/pricingTable.html',
        //    controller: 'pricingTableCtrl'
        //})
          //extra
        //.when('/extra-fullCalendar', {
        //    templateUrl: 'extra/fullCalendar/fullCalendar.html',
        //    controller: 'fullCalendarCtrl'
        //})
        //.when('/extra-google', {
        //    templateUrl: 'extra/google/google.html',
        //    controller: 'googleCtrl'
        //})
        //.when('/extra-slider', {
        //    templateUrl: 'extra/slider/slider.html',
        //    controller: 'sliderCtrl'
        //})
        //.when('/extra-vector', {
        //    templateUrl: 'extra/vector/vector.html',
        //    controller: 'vectorCtrl'
        //})
        //.when('/extra-widgets', {
        //    templateUrl: 'extra/widgets/widgets.html',
        //    controller: 'widgetsCtrl'
        //})
          //tables
        //.when('/tables-dynamic', {
        //    templateUrl: 'tables/dynamic/dynamic.html',
        //    controller: 'dynamicCtrl'
        //})
        //.when('/tables-editable', {
        //    templateUrl: 'tables/editable/editable.html',
        //    controller: 'editableCtrl'
        //})
        //.when('/tables-filter', {
        //    templateUrl: 'tables/filter/filter.html',
        //    controller: 'filterCtrl'
        //})
        //.when('/tables-styling', {
        //    templateUrl: 'tables/styling/styling.html',
        //    controller: 'stylingCtrl'
        //})
          //user
        //.when('/user-profile', {
        //    templateUrl: 'user/profile/profile.html',
        //    controller: 'profileCtrl'
        //})
        //.when('/user-sessionTimeout', {
        //    templateUrl: 'user/sessionTimeout/sessionTimeout.html',
        //    controller: 'sessionTimeoutCtrl'
        //})
          //layout
        //.when('/layout-api', {
        //    templateUrl: 'layout/api.html',
        //    controller: 'apiCtrl'
        //})
       .when('/loadDivisions', {
           templateUrl: 'app/divisions/loadDivisions.html',
           controller: 'loadDivisionsCtrl',
       })
        .when('/saveDivision/:id', {
            templateUrl: 'app/divisions/saveDivision.html',
            controller: 'saveDivisionCtrl',
        })
        .when('/saveCostcenter/:id', {
            templateUrl: 'app/costcenter/saveCostCenter.html',
            controller: 'saveCostCenterCtrl',
        })
        .when('/loadCostcenters', {
            templateUrl: 'app/costcenter/loadCostCenters.html',
            controller: 'loadCostCentersCtrl',
        })
        .when('/saveUserManagement/:id', {
            templateUrl: 'app/userManagement/saveUserManagement.html',
            controller: 'saveUserManagementCtrl',
            controllerAs: 'saveUserCtrl'
        })
        .when('/loadUserManagement', {
            templateUrl: 'app/userManagement/loadUserManagement.html',
            controller: 'loadUserManagementCtrl',
            controllerAs: 'loadUserCtrl'
        })
        .when('/addShipment/:id', {
            templateUrl: 'app/shipment/addShipment.html',
            controller: 'addShipmentCtrl',
            controllerAs: 'shipmentCtrl'
        })
         .when('/PaymentResult', {
             templateUrl: 'app/shipment/paymentResult.html',
             controller: 'paymentResultCtrl',
             controllerAs: 'resultCtrl'
         })
         .when('/loadShipments', {
             templateUrl: 'app/shipment/loadAllShipments.html',
             controller: 'loadShipmentsCtrl',
             controllerAs: 'loadShipCtrl'
         })
           .when('/ShipmentOverview', {
               templateUrl: 'app/shipment/ShipmentOverview.html',
               controller: 'shipmentOverviewCtrl',
               controllerAs: 'overviewShipCtrl'
           })
          .when('/TrackAndTrace', {
              templateUrl: 'app/shipment/TrackAndTrace.html',
              controller: 'trackAndTraceCtrl',
              controllerAs: 'trackCtrl'
          })
           .when('/PrintLabel', {
               templateUrl: 'app/shipment/printLabel.html',
               controller: 'printLabelCtrl',
               controllerAs: 'printCtrl'
           })
           .when('/PrintManifest', {
               templateUrl: 'app/shipment/printManifest.html',
               controller: 'printManifestCtrl',
               controllerAs: 'manifestCtrl'
           })
           .when('/AddressViewTemplate', {
               templateUrl: 'app/shipment/AddressViewTemplate.html',
           })
           .when('/PreviewLabelTemplate', {
               templateUrl: 'app/shipment/PreviewLabelTemplate.html',

           })
           .when('/DocumentRepository', {
               templateUrl: 'app/shipment/shipmentDocuments.html',
           })
           .when('/CustomerMangement', {
               templateUrl: 'app/admin/loadAllCompanies.html',
           })
            .when('/AdminMangement', {
                templateUrl: 'app/admin/ImportRateSheet.html',
            })
        .otherwise({
            redirectTo: '/loadShipments'
        });

      jwtInterceptorProvider.tokenGetter = function () {
          return localStorage.getItem('token');
      };

      $httpProvider.interceptors.push('jwtInterceptor');
  });


// Route State Load Spinner(used on page or content load)
MakeApp.directive('ngSpinnerLoader', ['$rootScope',
    function ($rootScope) {
        return {
            link: function (scope, element, attrs) {
                // by defult hide the spinner bar
                element.addClass('hide'); // hide spinner bar by default
                // display the spinner bar whenever the route changes(the content part started loading)
                $rootScope.$on('$routeChangeStart', function () {
                    element.removeClass('hide'); // show spinner bar
                });
                // hide the spinner bar on rounte change success(after the content loaded)
                $rootScope.$on('$routeChangeSuccess', function () {
                    setTimeout(function () {
                        element.addClass('hide'); // hide spinner bar
                    }, 500);
                    $("html, body").animate({
                        scrollTop: 0
                    }, 500);
                });
            }
        };
    }
])