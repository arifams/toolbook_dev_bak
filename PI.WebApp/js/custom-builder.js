angular.module('newApp')
.factory('builderFactory', ['applicationService', function (applicationService) {

    var builderFactory = {};

    /* Main Color */
    function mainColor() {
       
        $('.theme-color').on('click', function (e) {
            e.preventDefault();
            var main_color = $(this).data('color');
            var main_name = $(this).attr('data-main');
            $('body').removeClass(function (profileInformation, css) {
                return (css.match(/(^|\s)color-\S+/g) || []).join(' ');
            });
            $('body').addClass('color-' + main_name);
            $('.theme-color').removeClass('active');
            $(this).addClass('active');
            
        });
    }

    function setColor(mColor, mName) {
        
        var main_color = mColor;
        var main_name = mName;

        $('body').removeClass(function (profileInformation, css) {
            return (css.match(/(^|\s)color-\S+/g) || []).join(' ');
        });
        $('body').addClass('color-' + main_name);
        $('.theme-color').removeClass('active');
        $(this).addClass('active');
        
    }
	  
    builderFactory = {
        init: function (mColor, mName) {
            "use strict";

            mainColor();

            if (mColor != undefined && mName != undefined)
                setColor(mColor, mName);
           
            applicationService.resetStyle();

            if ($('body').hasClass('sidebar-top')) {
                destroySideScroll();
            }
        },
        loadLineChart: function () 
        {
            var randomScalingFactor = function () { return Math.round(Math.random() * 100) };
            var lineChartData = {
                labels: ["January", "February", "March", "April", "May", "June", "July"],
                datasets: [
                  {
                      label: "DHL",
                      fillColor: "rgba(220,220,220,0.2)",
                      strokeColor: "rgba(220,220,220,1)",
                      pointColor: "rgba(220,220,220,1)",
                      pointStrokeColor: "#fff",
                      pointHighlightFill: "#fff",
                      pointHighlightStroke: "rgba(220,220,220,1)",
                      data: [randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor()]
                  },
                  {
                      label: "FedEx",
                      fillColor: "rgba(49, 157, 181,0.2)",
                      strokeColor: "#319DB5",
                      pointColor: "#319DB5",
                      pointStrokeColor: "#fff",
                      pointHighlightFill: "#fff",
                      pointHighlightStroke: "#319DB5",
                      data: [randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor(), randomScalingFactor()]
                  }
                ]
            }
            var ctx = document.getElementById("line-chart").getContext("2d");
            window.myLine = new Chart(ctx).Line(lineChartData, {
                responsive: true,
                tooltipCornerRadius: 0
            });
        },
        
        loadDougnutChart1: function () {

            $(function () {

                var gaugeOptions = {

                    'chart': {
                        'type': 'solidgauge'
                    },

                    'title': null,

                    'tooltip': {
                        'enabled': false
                    },

                    'pane': {
                        'center': ['50%', '50%'],
                        'size': '60px',
                        'startAngle': 0,
                        'endAngle': 360,
                        'background': {
                            'backgroundColor': '#EEE',
                            'innerRadius': '80%',
                            'outerRadius': '100%',
                            'borderWidth': 0
                        }
                    },

                    'yAxis': {
                        'min': 0,
                        'max': 100,
                        'labels': {
                            'enabled': false
                        },

                        'lineWidth': 0,
                        'minorTickInterval': null,
                        'tickPixelInterval': 400,
                        'tickWidth': 0
                    },

                    'plotOptions': {
                        'solidgauge': {
                            'innerRadius': '80%'
                        },
                        'showInLegend': true
                    },

                    'series': [{
                        'name': 'Sea',
                        'data': [30],
                        'dataLabels': {
                            'enabled': true
                        }
                    }]
                };

                $('#my-chart1').highcharts(gaugeOptions);

            });
        },
        loadDougnutChart2: function () {

            $(function () {

                var gaugeOptions = {

                    'chart': {
                        'type': 'solidgauge'
                    },

                    'title': null,

                    'tooltip': {
                        'enabled': false
                    },

                    'pane': {
                        'center': ['50%', '50%'],
                        'size': '60px',
                        'startAngle': 0,
                        'endAngle': 360,
                        'background': {
                            'backgroundColor': '#EEE',
                            'innerRadius': '80%',
                            'outerRadius': '100%',
                            'borderWidth': 0
                        }
                    },

                    'yAxis': {
                        'min': 0,
                        'max': 100,
                        'labels': {
                            'enabled': false
                        },

                        'lineWidth': 0,
                        'minorTickInterval': null,
                        'tickPixelInterval': 400,
                        'tickWidth': 0
                    },

                    'plotOptions': {
                        'solidgauge': {
                            'innerRadius': '80%'
                        },
                        'showInLegend': true
                    },

                    'series': [{
                        'name': 'Express',
                        'data': [50],
                        'dataLabels': {
                            'enabled': true
                        }
                    }]
                };

                $('#my-chart2').highcharts(gaugeOptions);

            });
        },
        loadDougnutChart3: function () {

            $(function () {

                var gaugeOptions = {

                    'chart': {
                        'type': 'solidgauge'
                    },

                    'title': null,

                    'tooltip': {
                        'enabled': false
                    },

                    'pane': {
                        'center': ['50%', '50%'],
                        'size': '60px',
                        'startAngle': 0,
                        'endAngle': 360,
                        'background': {
                            'backgroundColor': '#EEE',
                            'innerRadius': '80%',
                            'outerRadius': '100%',
                            'borderWidth': 0
                        }
                    },

                    'yAxis': {
                        'min': 0,
                        'max': 100,
                        'labels': {
                            'enabled': false
                        },

                        'lineWidth': 0,
                        'minorTickInterval': null,
                        'tickPixelInterval': 400,
                        'tickWidth': 0
                    },

                    'plotOptions': {
                        'solidgauge': {
                            'innerRadius': '80%'
                        },
                        'showInLegend': true
                    },

                    'series': [{
                        'name': 'Air',
                        'data': [80],
                        'dataLabels': {
                            'enabled': true
                        }
                    }]
                };

                $('#my-chart3').highcharts(gaugeOptions);

            });
        },
        loadDougnutChart4: function () {

            $(function () {

                var gaugeOptions = {

                    'chart': {
                        'type': 'solidgauge'
                    },

                    'title': null,

                    'tooltip': {
                        'enabled': false
                    },

                    'pane': {
                        'center': ['50%', '50%'],
                        'size': '60px',
                        'startAngle': 0,
                        'endAngle': 360,
                        'background': {
                            'backgroundColor': '#EEE',
                            'innerRadius': '80%',
                            'outerRadius': '100%',
                            'borderWidth': 0
                        }
                    },

                    'yAxis': {
                        'min': 0,
                        'max': 100,
                        'labels': {
                            'enabled': false
                        },

                        'lineWidth': 0,
                        'minorTickInterval': null,
                        'tickPixelInterval': 400,
                        'tickWidth': 0
                    },
                    

                    'plotOptions': {
                        'solidgauge': {
                            'innerRadius': '80%'
                        }, 'showInLegend': true
                    },
                   

                    'series': [{
                        //'name': 'Express',
                        'data': ['Express',70],
                        'dataLabels': {
                            'enabled': true
                        }
                    }]
                };

                $('#my-chart4').highcharts(gaugeOptions);

            });
        },
        loadMap: function () {

        }

        };
    return builderFactory;
}]);
