// Token helper functions for localStorage
function getAccessToken() {
    return localStorage.getItem('accessToken');
}

function setAccessToken(token) {
    localStorage.setItem('accessToken', token);
}

function removeAccessToken() {
    localStorage.removeItem('accessToken');
}

// Token'ı her request'te header'a eklemek için interceptor
document.addEventListener('DOMContentLoaded', function() {
    // Fetch interceptor - tüm fetch isteklerine token ekle
    const originalFetch = window.fetch;
    window.fetch = function(...args) {
        const token = getAccessToken();
        if (token && args[1]) {
            args[1].headers = args[1].headers || {};
            if (!args[1].headers['Authorization']) {
                args[1].headers['Authorization'] = 'Bearer ' + token;
            }
        }
        return originalFetch.apply(this, args);
    };
});
