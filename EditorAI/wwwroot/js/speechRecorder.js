URL = window.URL || window.webkitURL;

var gumStream;
var rec;
var input;

var AudioContext = window.AudioContext || window.webkitAudioContext;
var audioContext;

var recordButton = document.getElementById("recordButton");
var stopButton = document.getElementById("stopButton");
var pauseButton = document.getElementById("pauseButton");
var recordingIndicator = document.getElementById("recordingIndicator");
var progressBar = document.getElementById("progressBar");
var recordingInProgressText = document.getElementById("recordingInProgressText");
var recordingPausedText = document.getElementById("recordingPausedText");


recordButton.addEventListener("click", startRecording);
stopButton.addEventListener("click", stopRecording);
pauseButton.addEventListener("click", pauseRecording);

recordButton.disabled = false;
stopButton.disabled = true;
pauseButton.disabled = true;

function startRecording() {
    const constraints  = {
        audio: true,
        video: false
    };

    /* Disable the record button until we get a success or fail from getUserMedia() */
    recordButton.disabled = true;
    stopButton.disabled = false;
    pauseButton.disabled = false

    console.log(navigator.mediaDevices.getUserMedia);
    console.log('start');
    navigator.mediaDevices.getUserMedia(constraints).then(function(stream) {
        recordingIndicator.classList.remove('d-none');
        recordingIndicator.classList.add('d-block');
        progressBar.classList.remove('bg-warning');
        progressBar.classList.add('bg-danger');
        recordingInProgressText.style.display = "block"; 
        recordingPausedText.style.display = "none";
            
        audioContext = new AudioContext();
        gumStream = stream;
        input = audioContext.createMediaStreamSource(stream);
        rec = new Recorder(input,{numChannels:1});
        
        //start the recording process
        rec.record();
        rec.recording = true;

        

    }).catch(function(err) {
        alert(err);
        recordButton.disabled = false;
        stopButton.disabled = true;
        pauseButton.disabled = true
    });

}

function pauseRecording() {
    if (rec.recording) {
        rec.stop();
        pauseButton.innerHTML = "Resume";
        progressBar.classList.remove('bg-danger');
        progressBar.classList.add('bg-warning');
        recordingInProgressText.style.display = "none"; 
        recordingPausedText.style.display = "block"; 
    } else {
        rec.record();
        pauseButton.innerHTML = "Pause";
        progressBar.classList.remove('bg-warning');
        progressBar.classList.add('bg-danger');
        recordingPausedText.style.display = "none";
        recordingInProgressText.style.display = "block";
    }
}

function stopRecording() {
    stopButton.disabled = true;
    recordButton.disabled = false;
    pauseButton.disabled = true;

    pauseButton.innerHTML="Pause";

    rec.stop();
    gumStream.getAudioTracks()[0].stop();

    rec.exportWAV(createDownloadLink);

    recordingIndicator.classList.remove('d-block');
    recordingIndicator.classList.add('d-none');
}

function createDownloadLink(blob) {
    const url = URL.createObjectURL(blob);
    console.log(url);

    const formData = new FormData();
    formData.append('audioFile', blob, uuidv4() + '.wav');

    var response = UploadAudio(formData);
    console.log(response);
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0,
            v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}