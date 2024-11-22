var dataTable;

$(document).ready(function () {
    loadDataTable();``
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "responsive": true,
        "ajax": { url: '/UserManagement/GetAll' },
        "columns": [
            {
                data: "id",
                "render": function (data) {
                    return `<input class="form-check-input text-info row-checkbox" type="checkbox" value="${data}">`
                },
                "width": "5%"
            },
            {
                data: 'name',
                "render": function (data) {
                    return `<span class="name-field">${data}</span>`;
                },
                "width": "30%"
            },
            { data: 'userName', "width": "35%" },
            {
                data: 'interval',
                "render": function (data, type, row) {
                    return `<span class="interval-field" title="Last seen: ${row.lastSeen}">${data}</span>`;
                },
                "width": "30%"
            }
            
        ] ,
        "rowCallback": function (row, data) {
            // Check if the user is blocked and add the 'blocked-row' class
            if (data.isBlocked) {
                $(row).addClass('blocked-row');
            }
        }
    });
    // Add "Select All" logic for the table header checkbox
    $('#flexCheckBoxes').on('change', function () {
        const isChecked = $(this).is(':checked'); // Check if the header checkbox is checked
        $('.row-checkbox').prop('checked', isChecked); // Set all row checkboxes to the same state
    });
}

// Handle bulk delete when the button is clicked
$(document).on('click', '#bulkDeleteButton', function () {
    const selectedIds = []; // Initialize an array to store selected IDs

    // Collect all checked checkboxes in the table body
    $('.row-checkbox:checked').each(function () {
        selectedIds.push($(this).val()); // Add each selected ID to the array
    });

    if (selectedIds.length === 0) {
        // Show a warning if no users are selected
        toastr.warning("No users selected!");
        return;
    }

    // Pass the selected IDs to the Delete function for bulk deletion
    Delete('/UserManagement/BulkDelete', selectedIds);
});

function Delete(url, selectedIds = []) {
    // Show a confirmation dialog
    Swal.fire({
        title: "Are you sure?",
        text: selectedIds.length > 0
            ? `You are about to delete ${selectedIds.length} user(s).` // Bulk delete message
            : "You won't be able to revert this!", // Single delete message
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            // Perform an AJAX request for deletion
            $.ajax({
                url: url, // Endpoint for deletion
                type: selectedIds.length > 0 ? "POST" : "DELETE", // Use POST for bulk delete
                contentType: "application/json", // Necessary for JSON payload
                data: selectedIds.length > 0 ? JSON.stringify(selectedIds) : null, // Send IDs if bulk delete
                success: function (data) {
                    location.reload(); // Reload the current page
                    toastr.success(data.message); // Show success notification
                },
                error: function () {
                    toastr.error("Error occurred while deleting."); // Show error notification
                    location.reload(); // Reload the current page
                }
            });
        }
    });
}

// Handle Bulk Lock
$(document).on("click", "#bulkLockButton", function () {
    const selectedIds = [];
    $(".row-checkbox:checked").each(function () {
        selectedIds.push($(this).val());
    });

    if (selectedIds.length === 0) {
        toastr.warning("No users selected for locking!");
        return;
    }

    // Pass the selected IDs to the Lock function
    BulkAction("/UserManagement/BulkLock", selectedIds, "lock");
});

// Handle Bulk Unlock
$(document).on("click", "#bulkUnlockButton", function () {
    const selectedIds = [];
    $(".row-checkbox:checked").each(function () {
        selectedIds.push($(this).val());
    });

    if (selectedIds.length === 0) {
        toastr.warning("No users selected for unlocking!");
        return;
    }

    // Pass the selected IDs to the Unlock function
    BulkAction("/UserManagement/BulkUnlock", selectedIds, "unlock");
});

function BulkAction(url, selectedIds, actionName) {
    // Show a confirmation dialog
    Swal.fire({
        title: `Are you sure?`,
        text: `You are about to ${actionName} ${selectedIds.length} user(s).`,
        icon: actionName === "unlock" ? "info" : "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: `Yes, ${actionName} them!`
    }).then((result) => {
        if (result.isConfirmed) {
            // Perform an AJAX request for locking/unlocking
            $.ajax({
                url: url,
                type: "POST",
                contentType: "application/json", // Send data as JSON
                data: JSON.stringify(selectedIds), // Pass the selected IDs
                success: function (data) {                    
                    toastr.success(data.message); // Show success notification
                    location.reload(); // Reload the current page
                },
                error: function () {
                    toastr.error(`Error occurred while trying to ${actionName} users.`);
                    location.reload(); // Reload the current page
                }
            });
        }
    });
}


