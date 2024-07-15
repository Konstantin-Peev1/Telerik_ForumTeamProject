document.addEventListener('DOMContentLoaded', function () {
    let currentPage = 1;
    const pageSize = 10;

    // Load posts on the home page
    if (document.getElementById('postFilter')) {
        loadPosts('latest', currentPage, pageSize);

        document.getElementById('postFilter').addEventListener('change', function () {
            currentPage = 1; // Reset to the first page
            loadPosts(this.value, currentPage, pageSize);
        });

        document.getElementById('pagination-next').addEventListener('click', function () {
            currentPage++;
            loadPosts(document.getElementById('postFilter').value, currentPage, pageSize);
        });

        document.getElementById('pagination-prev').addEventListener('click', function () {
            if (currentPage > 1) {
                currentPage--;
                loadPosts(document.getElementById('postFilter').value, currentPage, pageSize);
            }
        });
    }

    // Handle login form submission
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', async function (event) {
            event.preventDefault();
            const formData = new FormData(event.target);
            const data = {
                userName: formData.get('username'),
                password: formData.get('password')
            };

            const response = await fetch('/api/user/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                const token = await response.text();
                localStorage.setItem('jwtToken', token);
                alert('Login successful');
                window.location.href = '/'; // Redirect to home page or any other page
            } else {
                const error = await response.text();
                alert('Login failed: ' + error);
            }
        });
    }

    // Handle register form submission
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', async function (event) {
            event.preventDefault();
            const formData = new FormData(event.target);
            const data = {
                userName: formData.get('username'),
                firstName: formData.get('firstName'),
                lastName: formData.get('lastName'),
                email: formData.get('email'),
                password: formData.get('password')
            };

            const response = await fetch('/api/user/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                alert('Registration successful. Please login.');
                window.location.href = '/User/Login'; // Redirect to login page
            } else {
                const error = await response.text();
                alert('Registration failed: ' + error);
            }
        });
    }
});

async function loadPosts(filter, page, pageSize) {
    let url = '';
    if (filter === 'latest') {
        url = `/api/post/latest?page=${page}&pageSize=${pageSize}`;
    } else {
        url = `/api/post/most-commented?page=${page}&pageSize=${pageSize}`;
    }

    const response = await fetch(url);
    const posts = await response.json();
    const postsContainer = document.getElementById('posts');
    postsContainer.innerHTML = '';

    posts.forEach(post => {
        const postDiv = document.createElement('div');
        postDiv.classList.add('card', 'mb-4');
        postDiv.innerHTML = `
            <div class="card-body">
                <h2 class="card-title">${post.title}</h2>
                <p class="card-text">${post.content}</p>
                <div class="small text-muted">Likes: ${post.likes}</div>
                <a href="/Home/Login" class="btn btn-primary">Read more →</a>
            </div>
        `;
        postsContainer.appendChild(postDiv);
    });
}
