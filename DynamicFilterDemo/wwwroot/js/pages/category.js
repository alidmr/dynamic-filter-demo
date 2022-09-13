
$(document).ready(function () {
    LoadGrid();
});

function LoadGrid() {
    $('#CategoryGrid').DataTable({
        processing: true,
        serverSide: true,
        "searching": false,
        destroy: true,
        pageLength: 10,
        "order": [[1, "desc"]],
        responsive: true,
        "lengthMenu": [5, 10, 25, 50, 100, 1000],
        ajax: {
            url: '/Category/GetCategories',
            type: 'POST',
            error: function (xhr, error, thrown) {
                console.log("Error occurred!");
                console.log(xhr, error, thrown);
            },
            "statusCode": {
                401: function (xhr, error, thrown) {
                    console.log("Status Code 401!");
                    console.log(xhr, error, thrown);

                    window.location = '/home/error';
                    return false;
                }
            },
            dataType: 'json',
            data: function (data) {
                var colName;
                var sort = data.order[0].column;
                if (!data['columns'][sort]['data'] == '')
                    colName = data['columns'][sort]['data'] + ' ' + data.order[0].dir;
                else colName = data['columns'][1]['name'] + " desc";  // Id desc

                var searchForm = $('form.SearchForm');

                var formItems = searchForm.serializeArray();

                var filterItems = {};
                var filterList = new Array();

                $.each(formItems, function () {
                    if (this.value != undefined && this.value != '' && this.value != 'select') {
                        if (!this.name.includes('_ConditionType')) {
                            var inputValue = this.value;
                            var conditionTypeValue = $('#' + this.name + '_ConditionType').val();
                            var filterItem = {
                                'propertyName': this.name,
                                'operatorType': conditionTypeValue,
                                'value': inputValue
                            };
                            filterList.push(filterItem);
                        }

                        //filterItems[this.name] = this.value
                    }
                    //filterItems[this.name] = this.value || '';
                });

                var colFilter, col;
                var arr = {
                    'draw': data.draw,
                    'length': data.length,
                    'start': data.start,
                    'search': data.search.value,
                    'sort': colName,
                    'sortColumn': data['columns'][sort]['name'],
                    'sortBy': data.order[0].dir
                };

                if (filterList.length > 0) {
                    var filterParameters = JSON.stringify(filterList);
                    arr['parameters'] = filterParameters;
                }

                data['columns'].forEach(function (items, index) {
                    col = data['columns'][index]['name'];
                    colFilter = data['columns'][index]['search']['value'];
                    if (colFilter) {
                        arr[col] = colFilter;
                    } else {
                        var item = $('#' + col).val();
                        arr[col] = item;
                    }
                    //arr[col] = colFilter;
                });
                return arr;
            }
        },
        /*
         public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
         */
        columns: [
            { data: 'id', name: 'Id', 'autowidth': true, 'orderable': false },
            { data: 'id', name: 'Id', 'autowidth': true },
            { data: 'name', name: 'Name', 'orderable': true },
            { data: 'isActive', name: 'IsActive', 'orderable': true },
            { data: 'isDeleted', name: 'IsDeleted', 'orderable': true },
            { data: 'createdDate', name: 'CreatedDate', 'orderable': true },
            { data: 'updatedDate', name: 'UpdatedDate', 'orderable': true }
        ],
        language: {
            url: '/js/dataTables/dataTables.tr.json'
        },
        columnDefs: [
            {
                targets: 0,
                render: function (data, type, row, meta) {
                    var view = '<div class="btn-group">';
                    view += DeleteButtonFormatter(data);
                    view += ChangeActiveOrPassiveButtonFormatter(row);
                    view += EditButtonFormatter(data);
                    view += '</div>';

                    return view;
                }
            },
            {
                targets: 3,
                render: function (data, type, row, meta) {
                    return CheckFormatter(data);
                }
            },
            {
                targets: 4,
                render: function (data, type, row, meta) {
                    return CheckFormatter(data);
                }
            },
            {
                targets: 5,
                render: function (data, type, row, meta) {
                    return DateFormat(data);
                }
            },
            {
                targets: 6,
                render: function (data, type, row, meta) {
                    return DateFormat(data);
                }
            }
        ]
    });
}


function ChangeActiveOrPassiveButtonFormatter(row) {
    var view = '';
    view += '<button class="btn-success btn btn-sm" title="Sil" onclick="ChangeActiveOrPassiveItem(' + row.id + ',' + row.isActive + ')">';
    if (row.isActive) {
        view += '<i class="fa fa-toggle-on"></i>';
    }
    else {
        view += '<i class="fa fa-toggle-off"></i>';
    }
    view += '</button>';

    return view;
}

function DeleteButtonFormatter(data) {
    return '<button class="btn-danger btn btn-sm" title="Sil" onclick="DeleteItem(' + data + ')"><i class="fa fa-trash-o"></i></button>';
}

function EditButtonFormatter(data) {
    // asp-controller="Test" asp-action="TestPage" asp-route-id="1"
    return '<a class="btn-warning btn btn-sm" title="Düzenle" href="/Category/Edit/' + data + '"><i class="fa fa-pencil"></i></a>';
}

function CheckFormatter(item) {
    var view = '';
    if (item) {
        view = '<span class="badge bg-primary">Evet</span>';
    }
    if (!item) {
        view = '<span class="badge bg-danger">Hayır</span>';
    }
    return view;
}

function DateFormat(date) {
    if (date == null) {
        return "";
    }
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    var hour = '' + d.getHours();
    var minutes = '' + d.getMinutes();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;
    if (hour.length < 2) {
        hour = '0' + hour;
    }
    if (minutes.length < 2) {
        minutes = '0' + minutes;
    }
    var time1 = [day, month, year].join('.');
    var time2 = [hour, minutes].join(':');
    return [time1, time2].join(' ');
}

function DeleteItem(itemId) {
    Notiflix.Confirm.Show('Emin misiniz?', 'Seçilen kayıt silinecektir!', 'Evet', 'Hayır',
        function () {
            return;
            $.ajax({
                url: '/Category/Delete',
                method: 'DELETE',
                data: { 'itemId': itemId }
            }).done(function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                }
                else {
                    toastr.error(response.message);
                }
            });
        }, function () {
            return;
        });
}

function ChangeActiveOrPassiveItem(itemId, isActive) {
    var message = isActive === true ? 'Seçilen kayıt pasif yapılacaktır!' : 'Seçilen kayıt aktif yapılacaktır!'
    Notiflix.Confirm.Show('Emin misiniz?', message, 'Evet', 'Hayır',
        function () {
            return;
            $.ajax({
                url: '/Category/ChangeActiveOrPassiveItem',
                method: 'POST',
                data: { 'itemId': itemId }
            }).done(function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                }
                else {
                    toastr.error(response.message);
                }
            });
        }, function () {
            return;
        });
}

$(document).ready(function () {
    $('#BtnSearch').click(function (event) {
        event.preventDefault();
        LoadGrid();
    });

    $('#BtnClear').click(function (event) {
        event.preventDefault();
        $('form.SearchForm').find("input[type=text], input[type=email], input[type=number], textarea ").each(function () {
            $(this).val('');
        });

        $('form.SearchForm').find("select ").each(function () {
            $(this).val('select');
        });

        LoadGrid();
    });

    $('#BtnRefresh').click(function (event) {
        event.preventDefault();
        LoadGrid();
    });
});