/*
    Name: Ross Taggart
    ID: S1828840
*/

//! Section: Contents

    /*
     * - Functions
     *      - Submit_GET_Request
     *      - Submit_AJAX_POST_Request
     *      - Display_Sweet_Alert
    */

//! END Section: Contents

//! Section: Functions

/**
 * OnReady method called as soon as the file is loaded
*/
$(function () {
    // Register the handlers to the correct DOM elements
    Register_Request_DOM_Handlers();

    // Toggle tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Replicate the "Dark Unica" theme from highcharts (https://www.highcharts.com/demo/line-basic/dark-unica)
    Highcharts.theme = {
        colors: ['#2b908f', '#90ee7e', '#f45b5b', '#7798BF', '#aaeeee', '#ff0066',
            '#eeaaee', '#55BF3B', '#DF5353', '#7798BF', '#aaeeee'],
        chart: {
            backgroundColor: {
                color: '#424242'
            },
            style: {
                fontFamily: '\'Unica One\', sans-serif'
            },
            plotBorderColor: '#606063'
        },
        title: {
            style: {
                color: '#E0E0E3',
                textTransform: 'uppercase',
                fontSize: '20px'
            }
        },
        subtitle: {
            style: {
                color: '#E0E0E3',
                textTransform: 'uppercase'
            }
        },
        xAxis: {
            gridLineColor: '#707073',
            labels: {
                style: {
                    color: '#E0E0E3'
                }
            },
            lineColor: '#707073',
            minorGridLineColor: '#505053',
            tickColor: '#707073',
            title: {
                style: {
                    color: '#A0A0A3'
                }
            }
        },
        yAxis: {
            gridLineColor: '#707073',
            labels: {
                style: {
                    color: '#E0E0E3'
                }
            },
            lineColor: '#707073',
            minorGridLineColor: '#505053',
            tickColor: '#707073',
            tickWidth: 1,
            title: {
                style: {
                    color: '#A0A0A3'
                }
            }
        },
        tooltip: {
            backgroundColor: 'rgba(0, 0, 0, 0.85)',
            style: {
                color: '#F0F0F0'
            }
        },
        plotOptions: {
            series: {
                dataLabels: {
                    color: '#F0F0F3',
                    style: {
                        fontSize: '13px'
                    }
                },
                marker: {
                    lineColor: '#333'
                }
            },
            boxplot: {
                fillColor: '#505053'
            },
            candlestick: {
                lineColor: 'white'
            },
            errorbar: {
                color: 'white'
            }
        },
        legend: {
            backgroundColor: 'rgba(0, 0, 0, 0.5)',
            itemStyle: {
                color: '#E0E0E3'
            },
            itemHoverStyle: {
                color: '#FFF'
            },
            itemHiddenStyle: {
                color: '#606063'
            },
            title: {
                style: {
                    color: '#C0C0C0'
                }
            }
        },
        credits: {
            style: {
                color: '#666'
            }
        },
        labels: {
            style: {
                color: '#707073'
            }
        },
        drilldown: {
            activeAxisLabelStyle: {
                color: '#F0F0F3'
            },
            activeDataLabelStyle: {
                color: '#F0F0F3'
            }
        },
        navigation: {
            buttonOptions: {
                symbolStroke: '#DDDDDD',
                theme: {
                    fill: '#505053'
                }
            }
        },
        rangeSelector: {
            buttonTheme: {
                fill: '#505053',
                stroke: '#000000',
                style: {
                    color: '#CCC'
                },
                states: {
                    hover: {
                        fill: '#707073',
                        stroke: '#000000',
                        style: {
                            color: 'white'
                        }
                    },
                    select: {
                        fill: '#000003',
                        stroke: '#000000',
                        style: {
                            color: 'white'
                        }
                    }
                }
            },
            inputBoxBorderColor: '#505053',
            inputStyle: {
                backgroundColor: '#333',
                color: 'silver'
            },
            labelStyle: {
                color: 'silver'
            }
        },
        navigator: {
            handles: {
                backgroundColor: '#666',
                borderColor: '#AAA'
            },
            outlineColor: '#CCC',
            maskFill: 'rgba(255,255,255,0.1)',
            series: {
                color: '#7798BF',
                lineColor: '#A6C7ED'
            },
            xAxis: {
                gridLineColor: '#505053'
            }
        },
        scrollbar: {
            barBackgroundColor: '#808083',
            barBorderColor: '#808083',
            buttonArrowColor: '#CCC',
            buttonBackgroundColor: '#606063',
            buttonBorderColor: '#606063',
            rifleColor: '#FFF',
            trackBackgroundColor: '#404043',
            trackBorderColor: '#404043'
        }
    };

    // Apply the theme
    Highcharts.setOptions(Highcharts.theme);
});

/**
 * Name: Submit_GET_Request
 * Description: Generic method for performing GET requests to the API
 * @param {any} url The base URL to target
 * @param {any} params The key-value pairs of parameters to append to the URL
 * @param {any} callback The function to call when the data is fetched
 */
function Submit_GET_Request(url, params, callback) {
    // Create a variable to track the URL
    let formattedUrl = url;

    // Iterate through each parameter passed in
    $.each(params, function (key, val) {
        // Add the parameter to the formatted URL
        formattedUrl += val.value + '/';
    });

    // Perform a GET request, capture the data and call the callback passed in
    $.getJSON(formattedUrl, function (data) {
        callback(data);
    });
}

/**
 * Name: Submit_AJAX_POST_Request
 * Description: Generic method for performing AJAX POST requests to the API
 * @param {any} url The base URL to target
 * @param {any} requestData Object to pass to the backend
 * @param {any} callback The function to call when the data is fetched
 */
function Submit_AJAX_POST_Request(url, requestData, callback) {
    // Perform an AJAX request
    $.ajax({
        type: 'POST',
        url: url,
        data: JSON.stringify(requestData),
        contentType: 'application/json',
        success: function (result) {
            // Call the callback and pass in the fetched data
            callback(result);
        },
        error: function (result) {
            callback(result);
        }
    })
}

/**
 * Name: Display_Sweet_Alert
 * Description: Method to display a SweetAlert
 * @param {any} swalType The type of SweetAlert to display
 * @param {any} basicOptions Options to be used for basic SweetAlerts
 * @param {any} advancedOptions Options to be used for advanced SweetAlerts
 */
function Display_Sweet_Alert(swalType, basicOptions, advancedOptions) {
    // If a basic SweetAlert should be displayed
    if (basicOptions.use) {
        // Use the basic options to display the SweetAlert
        Swal.fire({
            title: basicOptions.title,
            text: basicOptions.text,
            icon: basicOptions.type
        })
    }
    // If an advanced SweetAlert should be displayed
    else {
        // Use the advanced options to display the SweetAlert
        Swal.fire({
            title: advancedOptions.title,
            icon: advancedOptions.type,
            html: advancedOptions.html
        })
    }
}

//! END Section: Functions

