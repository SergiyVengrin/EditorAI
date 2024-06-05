// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('.dropdown-item').on('click', function (e) {
    e.preventDefault();
    var languageId = $(this).attr('id');
    var languageValue = $(this).text();
    $('#languageDropdown').text(languageValue);
    // Perform actions based on the selected language
    console.log('Selected language:', languageValue);
});
