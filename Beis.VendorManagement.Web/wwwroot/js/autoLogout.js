var options = {
    autoLogoutDurationInMilliSeconds: 0
};

document.addEventListener("DOMContentLoaded", function () {
    if (options.autoLogoutDurationInMilliSeconds > 0) {

        setTimeout(callLogout, options.autoLogoutDurationInMilliSeconds);

        function callLogout() {
            var result = confirm('You are about to be logged out.\r\nPlease click OK to logout or Cancel to continue.');
            if (result) {
                window.location.href = "/MicrosoftIdentity/Account/SignOut";
                return true;
            }

            setTimeout(callLogout, options.autoLogoutDurationInMilliSeconds);
            return false;
        }
    }
});