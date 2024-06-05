
function UploadAudio(formData) {
    // Add the text from the textarea to the form data
    const textAreaValue = document.getElementById('textArea').value;
    formData.append('text', textAreaValue);

    fetch('/Home/UploadAudio', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(function(data) {
            if (data && data.responseContent) {
                // Set the response text in the textarea
                document.getElementById('textArea').value = data.responseContent;
            } else {
                console.log('File sent to API:', data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });
}
function changeLanguage(language) {
    $('#loadingCircle').show();

    $.ajax({
        type: 'GET',
        url: '/Home/ChangeLanguage',
        data: { language: language },
        success: function (result) {
            location.reload();
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}