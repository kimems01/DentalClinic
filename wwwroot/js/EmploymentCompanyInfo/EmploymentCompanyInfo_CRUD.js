var Details = function (id) {
    var url = "/EmploymentCompanyInfo/Details?id=" + id;
    $('#titleBigModal').html("Employment Company Info Details");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/EmploymentCompanyInfo/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleBigModal').html("Edit Employment Company Info");
    }
    else {
        $('#titleBigModal').html("Add Employment Company Info");
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
                url: "/EmploymentCompanyInfo/Delete?id=" + id,
                success: function (result) {
                    var message = "Employment Company has been deleted successfully. Employment Company ID: " + result.Id;
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
