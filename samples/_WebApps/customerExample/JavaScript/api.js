// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
"use-strict";
let messageId = 1
let TARGET_IP = "https://" + (window.location.hostname != "" ? window.location.hostname : "192.168.1.180") + "/api/jsonrpc"
let AUTH_TOKEN = null
function getToken() {
    cookieVal = getCookie("siemens_web_secure");
    AUTH_TOKEN = getCookie("token");
    if (cookieVal != "" && AUTH_TOKEN != "") {
        document.getElementById("notice").innerHTML = "You are logged in"
        getApiVersion()
        getTurbineNumber()
        bulkReadValues()
        showBtn("btn-logout")
        hideBtn("btn-login")
        closeModal()
        keepSession(AUTH_TOKEN);
    }
    else {
        location.href = "login.html";
    }
}
function keepSession(token, pingInterval) {
    var pingInterval = pingInterval || 10000;
    // send api ping every pingInterval (default: 10 seconds)
    setTimeout(function () {
        fetch(TARGET_IP, {
            method: "POST",
            headers: {
                "Content-type": "application/json",
                "X-Auth-Token": token
            },
            body: JSON.stringify({
                "id": messageId++,
                "jsonrpc": "2.0",
                "method": "Api.Ping",
            })
        })
        .then(response => response.json())
        .then((data) => {
            console.log("Ping RESPONSE:" + JSON.stringify(data));
            console.log("successfully extended session for: " + token);
            let exminutes = 2;
            setCookie("token", token, exminutes);
        })
        .catch(e => {
            console.log("ERROR: " + JSON.stringify(e));
              
        })
        keepSession(token);
    }, pingInterval);
}
function setCookie(cname, cvalue, exminutes) {
    //example for one working implementation can be found at: https://www.w3schools.com/js/js_cookies.asp
    alert("setcookie not implemented - can be found at: https://www.w3schools.com/js/js_cookies.asp");
}
function getCookie(cname) {
    //example for one working implementation can be found at: https://www.w3schools.com/js/js_cookies.asp
    alert("getcookie not implemented - can be found at: https://www.w3schools.com/js/js_cookies.asp");
    return "";
}
function updateTurbineSpeed() {
    if (AUTH_TOKEN !== null) {
        fetch(TARGET_IP, {
            method: "POST",
            headers: {
                "Content-type": "application/json",
                "X-Auth-Token": AUTH_TOKEN
            },
            body: JSON.stringify({
                "id": messageId++,
                "jsonrpc": "2.0",
                "method": "PlcProgram.Read",
                "params": {
                    "var": "\"turbine_speed\""
                }
            })
        })
        .then(response => response.json())
        .then((data) => {
            let turbNum = document.getElementById("turb-speed")
            turbNum.value = data.result
        })
        .catch(e => console.error(e))
    }
}
function turbSpeed() {
    if (AUTH_TOKEN !== null) {
        let turbSp = document.getElementById("turb-speed-sp")
        fetch(TARGET_IP, {
            method: "POST",
            headers: {
                "Content-type": "application/json",
                "X-Auth-Token": AUTH_TOKEN
            },
            body: JSON.stringify({
                "id": messageId++,
                "jsonrpc": "2.0",
                "method": "PlcProgram.Write",
                "params": {
                    "var": "\"turbine_speed_sp\"",
                    "value": parseFloat(turbSp.value)
                }
            })
        })
        .then(response => response.json())
        .then((data) => {
                if (data.result) {
                    console.log("Api responds with " + data.result)
                }
            })
        .catch(e => console.error(e))
    }
}
function getTurbineNumber() {
    let label = document.getElementById("turbine-number")
    fetch(TARGET_IP, {
        method: "POST",
        headers: {
            "Content-type": "application/json",
            "X-Auth-Token": AUTH_TOKEN
        },
        body: JSON.stringify({
            "id": messageId++,
            "jsonrpc": "2.0",
            "method": "PlcProgram.Read",
            "params": {
            "var": "\"turbineNumber\""
            }
        })
    })
    .then(response => response.json())
    .then((data) => {
        label.innerHTML = data.result
    })
    .catch(e => console.error(e))
}
function turbineSpeedOverride(element) {
    if (AUTH_TOKEN !== null) {
    fetch(TARGET_IP, {
        method: "POST",
        headers: {
            "Content-type": "application/json",
            "X-Auth-Token": AUTH_TOKEN
        },
        body: JSON.stringify({
            "id": messageId++, // id can be omitted, in that case it is a notification request
            "jsonrpc": "2.0",
            "method": "PlcProgram.Write",
            "params": {
                "var": "\"turbine_speed_override\"",
                "value": (element.value === "true")
            }
        })
    })
    .then(response => response.json())
    .then((data) => {
        if (data.result) {
            console.log("Api responds with " + data.result)
        }
    })
    .catch(e => console.error(e))
    }
}
function apiLogin() {
    let input_user = document.getElementById("username").value
    let input_password = document.getElementById("userpassword").value
    fetch(TARGET_IP, {
        method: "POST",
        headers: {
            "Content-type": "application/json",
        },
        body: JSON.stringify({
            "id": messageId++,
            "jsonrpc": "2.0",
            "method": "Api.Login",
            "params": {
                user: input_user,
                password: input_password,
                "include_web_application_cookie": true 
            }
        })
    })
    .then(response => response.json())
    .then((data) => {
        AUTH_TOKEN = data.result.token
        closeModal()
        setCookie("siemens_web_secure", data.result.web_application_cookie, 60 * 24 * 365);
        setCookie("token", AUTH_TOKEN, 2);
        location.href = "index.html";
        document.getElementById("notice").innerHTML = "You are logged in"
        getApiVersion()
        getTurbineNumber()
        bulkReadValues()
        showBtn("btn-logout")
        keepSession(AUTH_TOKEN);
    })
    .catch(e => console.error(e))
}
function bulkReadValues() {
    if (AUTH_TOKEN !== null) {
        let turbSpeedMax = document.getElementById("turb-speed-val")
        let turbAccelMax = document.getElementById("turb-accel-val")
        let turbJerkMax = document.getElementById("turb-jerk-val")
        fetch(TARGET_IP, {
            method: "POST",
            headers: {
                "Content-type": "application/json",
                "X-Auth-Token": AUTH_TOKEN
            },
            body: JSON.stringify([ {
                "id": messageId++,
                "jsonrpc": "2.0",
                "method": "PlcProgram.Read",
                "params": {
                    "var": "\"turbine_max_speed\""
                    }
                },
            {
                "id": messageId++,
                "jsonrpc": "2.0",
                "method": "PlcProgram.Read",
                "params": {
                    "var": "\"turbine_max_acceleration\""
                }
            },
            {
                "id": messageId++,
                "jsonrpc": "2.0",
                "method": "PlcProgram.Read",
                "params": {
                    "var": "\"turbine_max_jerk\""
                }
            }])
        })
        .then(response => response.json())
        .then((data) => {
        turbSpeedMax.innerHTML = data[0].result
        turbAccelMax.innerHTML = data[1].result
        turbJerkMax.innerHTML = data[2].result
        })
    }
} 　
function bulkWriteValues() {
    if (AUTH_TOKEN !== null) {
        let turbSpeedMaxSp = document.getElementById("turb-speed-max-sp")
        let turbAccelMaxSp = document.getElementById("turb-accel-max-sp")
        let turbJerkMaxSp = document.getElementById("turb-jerk-max-sp")
        fetch(TARGET_IP, {
            method: "POST",
            headers: {
            "Content-type": "application/json",
            "X-Auth-Token": AUTH_TOKEN
        },
        body: JSON.stringify([ {
            "id": messageId++,
            "jsonrpc": "2.0",
            "method": "PlcProgram.Write",
            "params": {
                "var": "\"turbine_max_speed\"",
                "value": parseFloat(turbSpeedMaxSp.value)
            }
        },
        {
            "id": messageId++,
            "jsonrpc": "2.0",
            "method": "PlcProgram.Write",
            "params": {
                "var": "\"turbine_max_acceleration\"",
                "value": parseFloat(turbAccelMaxSp.value)
            }
        },
        {
            "id": messageId++,
            "jsonrpc": "2.0",
            "method": "PlcProgram.Write",
            "params": {
                "var": "\"turbine_max_jerk\"",
                "value": parseFloat(turbJerkMaxSp.value)
            }
        }])
        })
        .then(response => response.json())
        .then((data) => {
            bulkReadValues()
        })
    }
}
function getApiVersion() {
    let label = document.getElementById("version-label")
    fetch(TARGET_IP, {
        method: "POST",
        headers: {
            "Content-type": "application/json",
            "X-Auth-Token": AUTH_TOKEN
        },
        body: JSON.stringify({
            "id": messageId++,
            "jsonrpc": "2.0",
            "method": "Api.Version"
        })
    })
    .then(response => response.json())
    .then((data) => {
        label.innerHTML = data.result
    })
    .catch(e => console.error(e))
}
function browseRoot() {
    let textarea = document.getElementById("browserarea")
        fetch(TARGET_IP, {
        method: "POST",
        headers: {
            "Content-type": "application/json",
            "X-Auth-Token": AUTH_TOKEN
        },
        body: JSON.stringify({
            "id": messageId++,
            "jsonrpc": "2.0",
            "method": "PlcProgram.Browse",
            "params": {
                "mode": "children"
            }
        })
    })
    .then(response => response.json())
    .then((data) => {
        textarea.value = JSON.stringify(data.result, null, 4)
    })
    .catch(e => console.error(e))
}
function apiLogout() {
    fetch(TARGET_IP, {
    method: "POST",
    headers: {
        "Content-type": "application/json",
        "X-Auth-Token": AUTH_TOKEN
    },
    body: JSON.stringify({
        "id": messageId++,
        "jsonrpc": "2.0",
        "method": "Api.Logout",
        })
    })
    .then(response => response.json())
    .then((data) => {
        AUTH_TOKEN = null
        setCookie("token", AUTH_TOKEN, 0);
        setCookie("siemens_web_secure", data.result.web_application_cookie, 60 * 24 * 365);
        location.reload();
    })
    .catch(e => console.error(e))
}
function changeIp() {
    let val = $("#target-ip").val();
    TARGET_IP = val + "/api/jsonrpc".replace("//api", "/api");
}
function closeModal() {
    let modal = document.getElementById("example-modal")
    if (modal)
        modal.style.display = "none"
}
function openModal() {
    let modal = document.getElementById("example-modal")
    modal.style.display = "block"
}
function hideBtn(btnId) {
    let btn = document.getElementById(btnId)
    btn.style.display = "none"
}
function showBtn(btnId) {
    let btn = document.getElementById(btnId)
    btn.style.display = "block"
}