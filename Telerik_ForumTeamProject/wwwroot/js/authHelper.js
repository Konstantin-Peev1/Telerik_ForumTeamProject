// authHelper.js

function getAuthToken() {
    return localStorage.getItem('jwtToken');
}

async function fetchWithAuth(url, options = {}) {
    const token = getAuthToken();
    if (!token) {
        throw new Error('No authentication token found');
    }

    options.headers = {
        ...options.headers,
        'Authorization': `Bearer ${token}`
    };

    return fetch(url, options);
}

console.log('authHelper.js loaded');
