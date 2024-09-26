


function settingAlertWinUncloseClass() {
    let ovsers = document.getElementsByClassName("OverCloseArea");
    for (let i = 0; i < ovsers.length; i++) {
        document.getElementsByClassName("OverCloseArea")[i].onclick = function (event) {event.stopPropagation(); }
    };
}
settingAlertWinUncloseClass();

function AlertWin(txt) {
    txt = txt == "" ? "無訊息!!" : txt;
    document.querySelector('.WinOverlay-content').innerHTML = txt;
    document.getElementById("fullScreenMsg").style.display = "block";
}
function OpenAlertWin() {
    document.getElementById("fullScreenMsg").style.display = "block";
}
function AlertWinClose() {
    document.querySelector(".WinOverlay-content").textContent = "";
    document.querySelector(".WinOverlay-buttons").classList.add("Hidden");
    document.getElementById("fullScreenMsg").style.display = "none";
}


var WinOverlay_buttons_Listening = false;
const confirmWin = (text, callback) => {
    AlertWin(text);
    document.querySelector('.WinOverlay-buttons').classList.remove('Hidden');     
    if (!WinOverlay_buttons_Listening) {
        document.querySelector('.WinOverlay-buttons').addEventListener('click', (e) => {
            const target = e.target;
            //console.log("click = " + target.className);
            if (target.className === "confirm-affirmative w3-right") { WinOverlay_buttons_Listening = false; callback(true); }
            else { callback(false); }
        });
        WinOverlay_buttons_Listening = true;
    }
    
}


//const KeyQuery = document.getElementById("KeyQuery")
//KeyQuery.addEventListener("click", (e) => {
//    confirmWin("TEST", (confirmed) => {
//        console.log(confirmed);
//        return confirmed;
//    })
//})

//confirmWin("TEST", (confirmed) => {
//    console.log(confirmed);
//})