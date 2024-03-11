var Details = function (id) {
    var url = "/FamilyInfo/Details?id=" + id;
    $('#titleBigModal').html("Family Info Details");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/FamilyInfo/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleBigModal').html("Edit Family Info");
    }
    else {
        $('#titleBigModal').html("Add Family Info");
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
                url: "/FamilyInfo/Delete?id=" + id,
                success: function (result) {
                    var message = "Family has been deleted successfully. Family ID: " + result.Id;
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
