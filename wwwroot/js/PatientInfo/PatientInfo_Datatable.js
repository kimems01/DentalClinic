$(document).ready(function () {
    document.title = 'Patient Info';

    $("#tblPatientInfo").DataTable({
        paging: true,
        select: true,
        "order": [[0, "desc"]],
        dom: 'Bfrtip',


        buttons: [
            'pageLength',
        ],


        "processing": true,
        "serverSide": true,
        "filter": true, //Search Box
        "orderMulti": false,
        "stateSave": true,

        "ajax": {
            "url": "/PatientInfo/GetDataTabelData",
            "type": "POST",
            "datatype": "json"
        },


        "columns": [
            { "data": "Id", "name": "Id" },
            {
                data: "OtherNames", "name": "OtherNames", render: function (data, type, row) {
                    return "<a href='#' onclick=DetailsPatientInfo('" + row.Id + "');>" + row.OtherNames + "</a>";
                }
            },
            { "data": "Surname", "name": "Surname" },
            { "data": "Gender", "name": "Gender" },
            {
                "data": "DateOfBirth",
                "name": "DateOfBirth",
                "render": function (data) {
                    var birthDate = new Date(data);
                    var currentDate = new Date();

                    var age = currentDate.getFullYear() - birthDate.getFullYear();
                    var monthDifference = currentDate.getMonth() - birthDate.getMonth();

                    // Adjust the age if the current date hasn't reached the birth month and day yet
                    if (monthDifference < 0 || (monthDifference === 0 && currentDate.getDate() < birthDate.getDate())) {
                        age--;
                    }

                    return age;
                }
            },
            { "data": "Phone", "name": "Phone" },
            { "data": "NationalID", "name": "NationalID" },
            { "data": "Residence", "name": "Residence" },
            { "data": "PaymentMode", "name": "PaymentMode" },
            {
                data: null, render: function (data, type, row) {
                    return "<a href='#' class='btn btn-info btn-xs' onclick=AcceptPatient('" + row.Id + "');>Accept</a>";
                }
            },
            {
                data: null, render: function (data, type, row) {
                    return "<a href='#' class='btn btn-info btn-xs' onclick=AddEditPatientInfo('" + row.Id + "');>Edit</a>";
                }
            },
            {
                data: null, render: function (data, type, row) {
                    return "<a href='#' class='btn btn-danger btn-xs' onclick=DeletePatientInfo('" + row.Id + "'); >Delete</a>";
                }
            }
        ],

        'columnDefs': [{
            'targets': [9, 10, 11],
            'orderable': false,
        }],

        "lengthMenu": [[20, 10, 15, 25, 50, 100, 200], [20, 10, 15, 25, 50, 100, 200]]
    });

});

