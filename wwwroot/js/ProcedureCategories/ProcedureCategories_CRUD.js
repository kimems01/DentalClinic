var Details = function (id) {
    var url = "/ProcedureCategories/Details?id=" + id;
    $('#titleBigModal').html("Procedure Category Details");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/ProcedureCategories/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleBigModal').html("Edit Procedure Category");
    }
    else {
        $('#titleBigModal').html("Add Procedure Category");
    }
    loadBigModal(url);
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
                url: "/ProcedureCategories/Delete?id=" + id,
                success: function (result) {
                    var message = "Procedure category has been deleted successfully. Category ID: " + result.Id;
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
