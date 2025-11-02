/* start DataTable */

var ConfigDataTableGlobal = function (controlName) {
    $('#' + controlName).DataTable({
        "paging": true,
        "lengthChange": false,
        "searching": false,
        "ordering": true,
        "info": false,
        "autoWidth": false,
        "pageLength": 10,
        "language": {
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "Ningún dato disponible en esta tabla",
            "sSearch": "Buscar:",
            "oPaginate": {
                "sFirst": "Primero",
                "sLast": "Último",
                "sNext": "Siguiente",
                "sPrevious": "Anterior"
            }
        }
    });
}

/* end DataTable */

/* start ShowToast */

function showToastMessage(text, type) {
    const toastLiveExample = document.getElementById("toast-message-head");
    const toast = new bootstrap.Toast(toastLiveExample);
    
    var myToast = $('#toast-message-head');
    $('#toast-message-text').html(text);

    switch (type) {
        case "error":
            myToast.removeClass('bg-primary bg-secondary bg-success bg-danger bg-warning bg-info bg-light bg-dark').addClass('bg-danger');
            break;
        case "success":
            myToast.removeClass('bg-primary bg-secondary bg-success bg-danger bg-warning bg-info bg-light bg-dark').addClass('bg-success');
            break;
        default:
            myToast.removeClass('bg-primary bg-secondary bg-success bg-danger bg-warning bg-info bg-light bg-dark').addClass('bg-primary');
            break;
    }
    toast.show();

    setTimeout(function () {
        toast.hide();
    }, 1000000);
}

/* end ShowToast */
