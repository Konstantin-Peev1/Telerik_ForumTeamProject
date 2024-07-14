﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository postRepository;

        public PostService(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public Post CreatePost(Post post, User user)
        {
            if(user.IsBlocked)
            {
                throw new AuthorisationExcpetion("Blocked users can't upload posts");
            }
            post.UserID = user.ID;
            Post postCreated = this.postRepository.CreatePost(post);
            return postCreated;
        }

        public bool DeletePost(int id, User user)
        {
            Post post = this.postRepository.GetPost(id);
            if(post.UserID != user.ID && !user.IsAdmin) 
            {
                throw new AuthorisationExcpetion("You can't delete other people's posts!");
            }
            return this.postRepository.DeletePost(post);
        }

        public List<Post> FilterBy(PostQueryParamteres filterParameters)
        {
            return this.postRepository.FilterBy(filterParameters);
        }

        public Post GetPost(int id)
        {
            return this.postRepository.GetPost(id);
        }

        public List<Post> GetTop10Commented()
        {
            return this.postRepository.GetTop10Commented();
        }

        public List<Post> GetTop10Recent()
        {
            return this.postRepository.GetTop10Recent();
        }
     

        public Post UpdatePost(int id, Post updatedPost, User user)
        {
            var post = this.postRepository.GetPost(id);
            if(post.UserID != user.ID )
            {
                throw new AuthorisationExcpetion("You can't edit posts of other people!");
            }
            var postUpdate = this.postRepository.UpdatePost(post, updatedPost);
            return postUpdate;
        }
        public List<Post> GetAll()
        {
            return this.postRepository.GetAll();
        }
        //Have to create a block user command.



    }
}
