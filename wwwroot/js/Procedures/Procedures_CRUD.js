var Details = function (id) {
    var url = "/Procedures/Details?id=" + id;
    $('#titleBigModal').html("Procedures Details");
    loadBigModal(url);
};




var AddEdit = function (id) {
    var url = "/Procedures/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleMediumModal').html("Edit Procedures");
    }
    else {
        $('#titleMediumModal').html("Add Procedures");
    }
    loadMediumModal(url);
};

var Delete = function (id) {
    Swal.fire({
        title: 'Do you want to delete this item?',
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "POST",
                url: "/Procedures/Delete?id=" + id,
                success: function (result) {
                    var message = "Lab Tests has been deleted successfully. Procedures ID: " + result.Id;
                    Swal.fire({
                        title: message,
                        text: 'Deleted!',
                        onAfterClose: () => {
                            location.reload();
                        }
                    });
                }
            });
        }
    });
};


var DeleteLabTestConfigurationItem = function (id) {
    Swal.fire({
        title: 'Do you want to delete this item?',
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "POST",
                url: "/LabTestConfiguration/Delete?id=" + id,
                success: function (result) {
                    var message = "Lab Test Configuration has been deleted successfully. Lab Test Configuration ID: " + result.Id;
                    Swal.fire({
                        title: message,
                        text: 'Deleted!',
                        onAfterClose: () => {
                            ConfigureTest(result.LabTestsId);
                        }
                    });
                }
            });
        }
    });
};