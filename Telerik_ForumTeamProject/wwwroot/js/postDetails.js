console.log('postDetails.js loaded');

document.addEventListener("DOMContentLoaded", function () {
    let postId = document.getElementById('postId').value;
    console.log(`Post ID: ${postId}`);  // Add this line
    let commentsPage = 1;
    const commentsPerPage = 5;

    async function loadComments(page, postId) {
        try {
            console.log(`Fetching comments for post ${postId}, page ${page}`);
            const response = await fetchWithAuth(`/api/comment?postId=${postId}&skip=${(page - 1) * commentsPerPage}&take=${commentsPerPage}`);
            if (response.ok) {
                const comments = await response.json();
                console.log("Fetched comments:", comments);

                const commentsContainer = document.getElementById('comments');

                if (page === 1) {
                    commentsContainer.innerHTML = ''; // Clear previous comments only on the first page load
                }

                comments.forEach(comment => {
                    const commentDiv = document.createElement('div');
                    commentDiv.classList.add('comment');
                    commentDiv.id = `comment-${comment.id}`;
                    commentDiv.innerHTML = `
                        <div class="comment-header">
                            <strong>${comment.userName}</strong> - ${comment.created}
                        </div>
                        <div class="comment-body">
                            ${comment.content}
                        </div>
                        <div class="comment-actions">
                            <button class="openReplyBox" data-comment-id="${comment.id}">Reply</button>
                            <button class="showReplies" data-comment-id="${comment.id}">Show Replies</button>
                        </div>
                        <div class="replies" id="replies-${comment.id}" style="display: none;"></div>
                        <div class="reply-box" id="reply-box-${comment.id}" style="display: none;">
                            <textarea placeholder="Enter your reply" class="replyContent" data-comment-id="${comment.id}"></textarea>
                            <button class="submitReply" data-comment-id="${comment.id}">Submit Reply</button>
                        </div>
                    `;
                    commentsContainer.appendChild(commentDiv);
                });

                addCommentEvents(); // Add event listeners for reply and show replies buttons

                if (comments.length < commentsPerPage) {
                    document.getElementById('loadMoreComments').style.display = 'none';
                } else {
                    document.getElementById('loadMoreComments').style.display = 'block';
                }
            } else {
                const error = await response.text();
                console.log('Failed to load comments: ' + error);
                alert('Failed to load comments: ' + error);
            }
        } catch (err) {
            console.log('An error occurred: ' + err.message);
            alert('An error occurred: ' + err.message);
        }
    }

    function addCommentEvents() {
        const openReplyBoxButtons = document.querySelectorAll('.openReplyBox');
        openReplyBoxButtons.forEach(button => {
            button.addEventListener('click', function () {
                const commentId = this.getAttribute('data-comment-id');
                const replyBox = document.getElementById(`reply-box-${commentId}`);
                replyBox.style.display = 'block';
            });
        });

        const showRepliesButtons = document.querySelectorAll('.showReplies');
        showRepliesButtons.forEach(button => {
            button.addEventListener('click', function () {
                const commentId = this.getAttribute('data-comment-id');
                const repliesContainer = document.getElementById(`replies-${commentId}`);
                if (repliesContainer.style.display === 'none') {
                    repliesContainer.style.display = 'block';
                    loadReplies(commentId, 1);
                } else {
                    repliesContainer.style.display = 'none';
                }
            });
        });

        const submitReplyButtons = document.querySelectorAll('.submitReply');
        submitReplyButtons.forEach(button => {
            button.addEventListener('click', async function () {
                const commentId = this.getAttribute('data-comment-id');
                const replyContent = document.querySelector(`.replyContent[data-comment-id='${commentId}']`).value;
                console.log(`Submitting reply to comment ${commentId} with content: ${replyContent}`);
                if (replyContent) {
                    try {
                        const response = await fetchWithAuth(`/api/comment/${commentId}/reply`, {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify({ content: replyContent })
                        });
                        if (response.ok) {
                            const replyBox = document.getElementById(`reply-box-${commentId}`);
                            replyBox.style.display = 'none';
                            document.querySelector(`.replyContent[data-comment-id='${commentId}']`).value = '';
                            loadReplies(commentId, 1);
                        } else {
                            const error = await response.text();
                            console.log('Failed to submit reply: ' + error);
                            alert('Failed to submit reply: ' + error);
                        }
                    } catch (err) {
                        console.log('An error occurred: ' + err.message);
                        alert('An error occurred: ' + err.message);
                    }
                } else {
                    alert('Reply content cannot be empty.');
                }
            });
        });
    }

    async function loadReplies(commentId, page) {
        const repliesContainer = document.getElementById(`replies-${commentId}`);
        const repliesPerPage = 5;

        try {
            const response = await fetchWithAuth(`/api/comment/replies?parentCommentId=${commentId}&skip=${(page - 1) * repliesPerPage}&take=${repliesPerPage}`);
            if (response.ok) {
                const replies = await response.json();
                console.log("Fetched replies:", replies);

                if (page === 1) {
                    repliesContainer.innerHTML = ''; // Clear previous replies only on the first page load
                }

                replies.forEach(reply => {
                    const replyDiv = document.createElement('div');
                    replyDiv.innerHTML = `
                        <p><strong>${reply.userName}</strong> - ${reply.created}</p>
                        <p>${reply.content}</p>
                    `;
                    repliesContainer.appendChild(replyDiv);
                });

                // Add "Load More Replies" button if there are more replies
                if (replies.length === repliesPerPage) {
                    const loadMoreButton = document.createElement('button');
                    loadMoreButton.innerText = 'Load More Replies';
                    loadMoreButton.onclick = () => loadReplies(commentId, page + 1);
                    repliesContainer.appendChild(loadMoreButton);
                }
            } else {
                const error = await response.text();
                console.log('Failed to load replies: ' + error);
                alert('Failed to load replies: ' + error);
            }
        } catch (err) {
            console.log('An error occurred: ' + err.message);
            alert('An error occurred: ' + err.message);
        }
    }

    document.getElementById('loadMoreComments').addEventListener('click', function () {
        commentsPage++;
        loadComments(commentsPage, postId);
    });

    // Initial load
    loadComments(commentsPage, postId);

    // Comment Modal
    const commentModal = document.getElementById('commentModal');
    const openCommentModalButton = document.getElementById('openCommentModal');
    const closeCommentModalButton = document.getElementById('closeCommentModal');
    const submitCommentButton = document.getElementById('submitComment');

    openCommentModalButton.addEventListener('click', () => {
        commentModal.style.display = 'block';
    });

    closeCommentModalButton.addEventListener('click', () => {
        commentModal.style.display = 'none';
    });

    submitCommentButton.addEventListener('click', async () => {
        const content = document.getElementById('commentContent').value;
        console.log(`Submitting comment with content: ${content}`);
        if (content) {
            try {
                const response = await fetchWithAuth(`/api/comment?postId=${postId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ content })
                });
                if (response.ok) {
                    commentModal.style.display = 'none';
                    document.getElementById('commentContent').value = '';
                    commentsPage = 1;
                    loadComments(commentsPage, postId);
                } else {
                    const error = await response.text();
                    console.log('Failed to submit comment: ' + error);
                    alert('Failed to submit comment: ' + error);
                }
            } catch (err) {
                console.log('An error occurred: ' + err.message);
                alert('An error occurred: ' + err.message);
            }
        } else {
            alert('Comment content cannot be empty.');
        }
    });

    window.onclick = function (event) {
        if (event.target == commentModal) {
            commentModal.style.display = "none";
        }
    }
});
