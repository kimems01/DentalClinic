var Details = function (id) {
    var url = "/PatientTest/Details?id=" + id;
    $('#titleExtraBigModal').html("Patient Test Summary");
    loadExtraBigModal(url);
};


var AddEdit = function (id) {
    var url = "/PatientTest/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Edit Patient Test");
    }
    else {
        $('#titleExtraBigModal').html("Add Patient Test");
    }
    loadExtraBigModal(url);
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
                url: "/PatientTest/Delete?id=" + id,
                success: function (result) {
                    var message = "Patient Test has been deleted successfully. PatientTest ID: " + result.Id;
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
