// Admin product table — server-side DataTables backed by /Product/GetData
(function () {
    "use strict";

    function escapeHtml(value) {
        return $("<div>").text(value == null ? "" : String(value)).html();
    }

    function currency(value) {
        var number = Number(value) || 0;
        return "$" + number.toFixed(2);
    }

    function thumbnail(row) {
        if (row.img) {
            return '<img class="product-thumb" src="/' + escapeHtml(row.img) + '" alt="" />';
        }
        return '<span class="product-thumb d-inline-flex align-items-center justify-content-center">' +
            '<i class="fa-solid fa-image text-muted"></i></span>';
    }

    $(function () {
        var table = $("#mytable").DataTable({
            processing: true,
            serverSide: true,
            autoWidth: false,
            searching: true,
            ordering: false,
            pageLength: 10,
            lengthMenu: [10],
            ajax: {
                url: "/Product/GetData",
                type: "GET"
            },
            columns: [
                {
                    data: "name",
                    render: function (data, type, row) {
                        return '<div class="d-flex align-items-center gap-3">' +
                            thumbnail(row) +
                            '<span class="fw-semibold">' + escapeHtml(data) + '</span></div>';
                    }
                },
                {
                    data: "description",
                    render: function (data) {
                        return '<div class="clamp-2">' + escapeHtml(data) + '</div>';
                    }
                },
                {
                    data: "price",
                    render: function (data) {
                        return '<span class="fw-semibold">' + currency(data) + '</span>';
                    }
                },
                { data: "categoryName" },
                {
                    data: "id",
                    className: "text-end",
                    render: function (id) {
                        return '<div class="action-group justify-content-end">' +
                            '<a href="/Product/Edit/' + id + '" class="btn btn-sm btn-outline-primary" aria-label="Edit product">' +
                            '<i class="fa-solid fa-pen"></i></a>' +
                            '<button type="button" class="btn btn-sm btn-outline-danger js-delete-product" ' +
                            'data-id="' + id + '" aria-label="Delete product">' +
                            '<i class="fa-solid fa-trash"></i></button></div>';
                    }
                }
            ],
            language: {
                emptyTable: "No products found.",
                zeroRecords: "No products match your search.",
                processing: "Loading products..."
            }
        });

        $("#mytable").on("click", ".js-delete-product", function () {
            var id = $(this).data("id");

            Swal.fire({
                title: "Delete this product?",
                text: "This action cannot be undone.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Delete",
                confirmButtonColor: "#dc2626",
                cancelButtonText: "Cancel"
            }).then(function (result) {
                if (!result.isConfirmed) {
                    return;
                }

                fetch("/Product/Delete/" + id, { method: "DELETE" })
                    .then(function (response) { return response.json(); })
                    .then(function (data) {
                        if (data.success) {
                            toastr.success(data.message || "Product deleted.");
                            table.ajax.reload(null, false);
                        } else {
                            toastr.error(data.message || "Unable to delete product.");
                        }
                    })
                    .catch(function () {
                        toastr.error("Unable to delete product.");
                    });
            });
        });
    });
})();
