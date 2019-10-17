  //gauge(活動-報名錄取狀態-右側圓形進度)
$(function () {
    
    
    var gaugeOptions = {

        chart: {
            type: 'solidgauge'
        },

        title: null,

        pane: {
            center: ['50%', '85%'],
            size: '100%',
            startAngle: -90,
            endAngle: 90,
            background: {
                backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || '#f1f1f1',
                innerRadius: '60%',
                outerRadius: '100%',
                shape: 'arc'
            }
        },

        tooltip: {
            enabled: false
        },

        // the value axis
        yAxis: {
            stops: [
                [0.1, '#84bcee'], // start
                [0.5, '#7BB8EF'], // durtion
                [0.9, '#6db1ef'] // end
            ],
            lineWidth: 0,
            minorTickInterval: null,
            tickPixelInterval: 400,
            tickWidth: 0,
            title: {
                y: -70
            },
            labels: {
                y: 16
            }
        },

        plotOptions: {
            solidgauge: {
                dataLabels: {
                    y: 5,
                    borderWidth: 0,
                    useHTML: true
                }
            }
        }
    };

    // The speed gauge
    $('#apply-result-circle').highcharts(Highcharts.merge(gaugeOptions, {
        yAxis: {
            min: 0,
            max: 200,
            title: {
                text: '報名率'
            }
        },

        credits: {
            enabled: false
        },

        series: [{
            name: '報名率',
            data: [80],
            dataLabels: {
                format: '<div style="text-align:center"><span style="font-size:16px;color:' +
                    ((Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black') + '">{y}</span><br/>' +
                       '<span style="font-size:12px;color:silver">%</span></div>'
            },
            tooltip: {
                valueSuffix: '%'
            }
        }]

    }));

    // Bring life to the dials
    setTimeout(function () {
        // Speed
        var chart = $('#apply-result-circle').highcharts(),
            point,
            newVal,
            inc;

        if (chart) {
            point = chart.series[0].points[0];
            inc = Math.round((Math.random() - 0.5) * 100);
            newVal = point.y + inc;

            if (newVal < 0 || newVal > 200) {
                newVal = point.y - inc;
            }

            point.update(newVal);
        }

    }, 2000);

    
});

  //gauge(活動-繳費狀態-右側圓形進度)
$(function () {
    
    
    var gaugeOptions = {

        chart: {
            type: 'solidgauge'
        },

        title: null,

        pane: {
            center: ['50%', '85%'],
            size: '100%',
            startAngle: -90,
            endAngle: 90,
            background: {
                backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || '#f1f1f1',
                innerRadius: '60%',
                outerRadius: '100%',
                shape: 'arc'
            }
        },

        tooltip: {
            enabled: false
        },

        // the value axis
        yAxis: {
            stops: [
                [0.1, '#84bcee'], // start
                [0.5, '#7BB8EF'], // durtion
                [0.9, '#6db1ef'] // end
            ],
            lineWidth: 0,
            minorTickInterval: null,
            tickPixelInterval: 400,
            tickWidth: 0,
            title: {
                y: -70
            },
            labels: {
                y: 16
            }
        },

        plotOptions: {
            solidgauge: {
                dataLabels: {
                    y: 5,
                    borderWidth: 0,
                    useHTML: true
                }
            }
        }
    };

    // The speed gauge
    $('#pay-circle').highcharts(Highcharts.merge(gaugeOptions, {
        yAxis: {
            min: 0,
            max: 200,
            title: {
                text: '確定參與率'
            }
        },

        credits: {
            enabled: false
        },

        series: [{
            name: '確定參與率',
            data: [80],
            dataLabels: {
                format: '<div style="text-align:center"><span style="font-size:16px;color:' +
                    ((Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black') + '">{y}</span><br/>' +
                       '<span style="font-size:12px;color:silver">%</span></div>'
            },
            tooltip: {
                valueSuffix: '%'
            }
        }]

    }));

    // Bring life to the dials
    setTimeout(function () {
        // Speed
        var chart = $('#pay-circle').highcharts(),
            point,
            newVal,
            inc;

        if (chart) {
            point = chart.series[0].points[0];
            inc = Math.round((Math.random() - 0.5) * 100);
            newVal = point.y + inc;

            if (newVal < 0 || newVal > 200) {
                newVal = point.y - inc;
            }

            point.update(newVal);
        }

    }, 2000);

    
});

  //gauge(活動-報到狀態-右側圓形進度)
$(function () {
    
    
    var gaugeOptions = {

        chart: {
            type: 'solidgauge'
        },

        title: null,

        pane: {
            center: ['50%', '85%'],
            size: '100%',
            startAngle: -90,
            endAngle: 90,
            background: {
                backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || '#f1f1f1',
                innerRadius: '60%',
                outerRadius: '100%',
                shape: 'arc'
            }
        },

        tooltip: {
            enabled: false
        },

        // the value axis
        yAxis: {
            stops: [
                [0.1, '#84bcee'], // start
                [0.5, '#7BB8EF'], // durtion
                [0.9, '#6db1ef'] // end
            ],
            lineWidth: 0,
            minorTickInterval: null,
            tickPixelInterval: 400,
            tickWidth: 0,
            title: {
                y: -70
            },
            labels: {
                y: 16
            }
        },

        plotOptions: {
            solidgauge: {
                dataLabels: {
                    y: 5,
                    borderWidth: 0,
                    useHTML: true
                }
            }
        }
    };

    // The speed gauge
    $('#check-circle').highcharts(Highcharts.merge(gaugeOptions, {
        yAxis: {
            min: 0,
            max: 200,
            title: {
                text: '報到率'
            }
        },

        credits: {
            enabled: false
        },

        series: [{
            name: '報到率',
            data: [80],
            dataLabels: {
                format: '<div style="text-align:center"><span style="font-size:16px;color:' +
                    ((Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black') + '">{y}</span><br/>' +
                       '<span style="font-size:12px;color:silver">%</span></div>'
            },
            tooltip: {
                valueSuffix: '%'
            }
        }]

    }));

    // Bring life to the dials
    setTimeout(function () {
        // Speed
        var chart = $('#check-circle').highcharts(),
            point,
            newVal,
            inc;

        if (chart) {
            point = chart.series[0].points[0];
            inc = Math.round((Math.random() - 0.5) * 100);
            newVal = point.y + inc;

            if (newVal < 0 || newVal > 200) {
                newVal = point.y - inc;
            }

            point.update(newVal);
        }

    }, 2000);

    
});


//bar
$(function () {
    $('.chart-bar').highcharts({
        chart: {
            type: 'bar'
        },
        title: {
            text: null
        },
        subtitle: {
            text: null
        },
        xAxis: {
            categories: ['選項一', '選項二', '選項三', '選項四', '選項五','選項六' ],
            lineColor: '#FFFFFF',
            tickColor: '#FFFFFF'
        },
        yAxis: {
            min: 0,
            title: {
                text: null
            },
            gridLineWidth: 0,
            lineColor: '#FFFFFF',
            tickColor: '#FFFFFF',
            labels:{
                enabled: false
            },
            floor: 0,
            ceiling: 100
        },
        legend: {
            reversed: false,
            enabled: false
        },

        plotOptions: {
            series: {
                stacking: 'normal'
            }
        },
        series: [{
            name: 'John',
            data: [50, 30, 40, 70, 80, 10],

        }]
    });
});
//3d pie for forms
$(function () {
    $('.chart-pie2').highcharts({
        chart: {
            height: 500,
            /*style: {
                padding: '30px 0 20px'
            },*/
            type:'pie',
            spacingTop: 50,
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0
            }
        },
        title: {
            text: null,
        },
        subtitle: {
            text: null,
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                depth: 35,
                dataLabels: {
                    enabled: true,
                    format: '{point.name}'
                },
                showInLegend: true
            }
        },
        legend: {
            reversed: false,
            enabled: false
        },
        series: [{
            type: 'pie',
            name: '百分比',
            data: [
                {
                    name: '選項一',
                    y: 50,
                    sliced: true,
                    selected: true,
                    color:'#28a0d4'
                },

                {
                    name: '選項二',
                    y: 30,
                    color:'#4db5cb'
                },

                {
                    name: '選項三',
                    y: 40,
                    color:'#71bdb7'
                },
                {
                    name: '選項四',
                    y: 70,
                    color:'#6fe2bf'
                },
                {
                    name: '選項五',
                    y: 80,
                    color:'#81bfab'
                },
                {
                    name: '選項六',
                    y: 10,
                    color:'#7bb2a5'
                }
            ]
        }]
    });
});


//3d pie
$(function () {
    $('.chart-pie').highcharts({
        chart: {
            height: 500,
            /*style: {
                padding: '30px 0 20px'
            },*/
            type:'pie',
            spacingTop: 50,
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0
            }
        },
        title: {
            text: null,
        },
        subtitle: {
            text: null,
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                depth: 35,
                dataLabels: {
                    enabled: true,
                    format: '{point.name}'
                },
                showInLegend: true
            }
        },
        series: [{
            type: 'pie',
            name: '瀏覽量',
            data: [
            	{
                    name: '首頁',
                    y: 200,
                    sliced: true,
                    selected: true,
                    color:'#ec5f12'
                },

                {
                    name: '最新消息',
                    y: 200,
                    color:'#579347'
                },

                {
                    name: '單筆主標題…',
                    y: 200,
                    color:'#684b89'
                }
            ]
        }]
    });
});


//line
$(function () {
    $('.chart-line').highcharts({
        chart: {
            height: 500,
            /*style: {
                padding: '30px 0 20px'
            },*/
            type:'line',
            spacingTop: 50
        },
        title: {
            text: null,
        },
        subtitle: {
            text: null,
        },
        /*legend: {
            enabled: false
        },*/
        xAxis: {
            title: {
                text: '日期',
                margin:20
            },
            categories: ['7/29', '7/30', '7/31', '8/1', '8/2', '8/3',
                '8/4', '8/5', '8/6', '8/7', '8/8', '8/9', '8/10', '8/11'],
            tickmarkPlacement: 'on',
            labels: {
                style: {
                    fontSize:'18px'
                },
            },
            plotLines: [{
                color: '#cccccc',
                width: 2,
                value: 5,
                label: {
                    text: '國樂夜未眠<br/>(售票開始)',
                    verticalAlign: 'top',
                    textAlign: 'center',
                    rotation: 0,
                    y:-20,
                }
            }, {
                color: '#cccccc',
                width: 2,
                value: 7,
                label: {
                    text: '國樂夜未眠<br/>(售票結束)',
                    verticalAlign: 'top',
                    textAlign: 'center',
                    rotation: 0,
                    y:-20,
                }
            }, {
                color: '#cccccc',
                width: 2,
                value: 8,
                label: {
                    text: '國樂夜未眠<br/>(活動開始)',
                    verticalAlign: 'top',
                    textAlign: 'center',
                    rotation: 0,
                    y:-20,
                }
            }]
        },
        yAxis: {
            min: 0,
            max: 1200,
            tickInterval: 200,
            title: {
                text: '瀏覽量',
                rotation:0,
                align: 'high',
                offset: 0,
                y: -20
            },
            ceiling: 1200,
            categories: ['0', '200', '400', '600', '800', '1000', '1200'],
            labels: {
                style: {
                    fontSize:'18px'
                }
            },
            maxPadding: 0.05
        },
        series: [
        {
            name: '首頁瀏覽量',
            data: [200, 509, 1000, 750, 680, 600, 988, 653, 420, 609, 950, 350, 583, 600],
            marker: {
                enabled: false
            },
            lineWidth:5,
            color:'#ec5f12',
            zIndex: 2
        },

        {
            name: '最新消息瀏覽量',
            data: [150, 250, 400, 500, 483, 620, 700, 453, 600, 580, 550, 650, 900, 500],
            marker: {
                enabled: false
            },
            color:'#579347'
        },

        {
            name: '單筆主標題…瀏覽量',
            data: [0, 200, 550, 650, 583, 700, 788, 500, 820, 409, 350, 600, 780, 400],
            marker: {
                enabled: false
            },
            color:'#684b89'
        }
        ]
    });
});


//column
$(function () {
    $('.chart-column').highcharts({
        chart: {
        	height: 500,
            type:'column',
            spacingTop: 50
        },
        title: {
            text: null
        },
        subtitle: {
            text: null
        },
        /*legend: {
            enabled: false
        },*/
        xAxis: {
            title: {
                text: '日期',
                margin:20
            },
            categories: ['7/29', '7/30', '7/31', '8/1', '8/2', '8/3',
                '8/4', '8/5', '8/6', '8/7', '8/8', '8/9', '8/10', '8/11'],
            tickmarkPlacement: 'on',
            labels: {
                style: {
                    fontSize:'18px'
                },
            },
            plotLines: [{
                color: '#cccccc',
                width: 2,
                value: 5,
                label: {
                    text: '國樂夜未眠<br/>(售票開始)',
                    verticalAlign: 'top',
                    textAlign: 'center',
                    rotation: 0,
                    y:-20,
                }
            }, {
                color: '#cccccc',
                width: 2,
                value: 7,
                label: {
                    text: '國樂夜未眠<br/>(售票結束)',
                    verticalAlign: 'top',
                    textAlign: 'center',
                    rotation: 0,
                    y:-20,
                }
            }, {
                color: '#cccccc',
                width: 2,
                value: 8,
                label: {
                    text: '國樂夜未眠<br/>(活動開始)',
                    verticalAlign: 'top',
                    textAlign: 'center',
                    rotation: 0,
                    y:-20,
                }
            }]
        },
        yAxis: {
            min: 0,
            max: 1200,
            tickInterval: 200,
            title: {
                text: '瀏覽量',
                rotation:0,
                align: 'high',
                offset: 0,
                y: -20
            },
            ceiling: 1200,
            categories: ['0', '200', '400', '600', '800', '1000', '1200'],
            labels: {
                style: {
                    fontSize:'18px'
                }
            },
            maxPadding: 0.05
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
        },
        series: [
        {
            name: '首頁瀏覽量',
            data: [200, 509, 1000, 750, 680, 600, 988, 653, 420, 609, 950, 350, 583, 600],
            marker: {
                enabled: false
            },
            lineWidth:5,
            color:'#ec5f12',
            zIndex: 2
        },

        {
            name: '最新消息瀏覽量',
            data: [150, 250, 400, 500, 483, 620, 700, 453, 600, 580, 550, 650, 900, 500],
            marker: {
                enabled: false
            },
            color:'#579347'
        },

        {
            name: '單筆主標題…瀏覽量',
            data: [0, 200, 550, 650, 583, 700, 788, 500, 820, 409, 350, 600, 780, 400],
            marker: {
                enabled: false
            },
            color:'#684b89'
        }
        ]
    });
});


//area
$(function () {
    $('.chart-area').highcharts({
        chart: {
        	height: 500,
            type:'area',
            spacingTop: 50
        },
        title: {
            text: null
        },
        subtitle: {
            text: null
        },
        /*legend: {
            enabled: false
        },*/
        xAxis: {
            title: {
                text: '日期',
                margin:20
            },
            categories: ['7/29', '7/30', '7/31', '8/1', '8/2', '8/3',
                '8/4', '8/5', '8/6', '8/7', '8/8', '8/9', '8/10', '8/11'],
            tickmarkPlacement: 'on',
            labels: {
                style: {
                    fontSize:'18px'
                },
                formatter: function () {
                    return this.value; // clean, unformatted number for year
                }
            },
            plotLines: [{
                color: '#cccccc',
                width: 2,
                value: 5,
                label: {
                    text: '國樂夜未眠<br/>(售票開始)',
                    verticalAlign: 'top',
                    textAlign: 'center',
                    rotation: 0,
                    y:-20,
                }
            }, {
                color: '#cccccc',
                width: 2,
                value: 7,
                label: {
                    text: '國樂夜未眠<br/>(售票結束)',
                    verticalAlign: 'top',
                    textAlign: 'center',
                    rotation: 0,
                    y:-20,
                }
            }, {
                color: '#cccccc',
                width: 2,
                value: 8,
                label: {
                    text: '國樂夜未眠<br/>(活動開始)',
                    verticalAlign: 'top',
                    textAlign: 'center',
                    rotation: 0,
                    y:-20,
                }
            }]
        },
        yAxis: {
            min: 0,
            max: 1200,
            tickInterval: 200,
            title: {
                text: '瀏覽量',
                rotation:0,
                align: 'high',
                offset: 0,
                y: -20
            },
            ceiling: 1200,
            categories: ['0', '200', '400', '600', '800', '1000', '1200'],
            labels: {
                style: {
                    fontSize:'18px'
                },
                formatter: function () {
                    return this.value / 1000 + 'k';
                }
            },
            maxPadding: 0.05
        },
        plotOptions: {
            area: {
                /*pointStart: 1940,*/
                marker: {
                    enabled: false,
                    symbol: 'circle',
                    radius: 2,
                    states: {
                        hover: {
                            enabled: true
                        }
                    }
                }
            },
            series: {
                fillOpacity: 0.3
            }
        },
        series: [
        {
            name: '首頁瀏覽量',
            data: [200, 509, 1000, 750, 680, 600, 988, 653, 420, 609, 950, 350, 583, 600],
            marker: {
                enabled: false
            },
            lineWidth:5,
            color:'#ec5f12',
            zIndex: 2
        },

        {
            name: '最新消息瀏覽量',
            data: [150, 250, 400, 500, 483, 620, 700, 453, 600, 580, 550, 650, 900, 500],
            marker: {
                enabled: false
            },
            color:'#579347'
        },

        {
            name: '單筆主標題…瀏覽量',
            data: [0, 200, 550, 650, 583, 700, 788, 500, 820, 409, 350, 600, 780, 400],
            marker: {
                enabled: false
            },
            color:'#684b89'
        }
        ]
    });
});

