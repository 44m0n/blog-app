using BlogApp.Dotnet.API.IntegrationTests.Helpers;
using BlogApp.Dotnet.ApplicationCore.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlogApp.Dotnet.API.IntegrationTests
{
    public class CommentsTests : IClassFixture<CustomWebApplicationFactoryComments<Startup>>
    {
        private readonly CustomWebApplicationFactoryComments<Startup> _factory;
        private readonly HttpClient _client;

        public CommentsTests(CustomWebApplicationFactoryComments<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Get_GetComments_ReturnsPaginatedDTO()
        {
            string request = "/api/comments?postID=1";
            var response = await _client.GetAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            var content = await response.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();
            Assert.Equal(5, content.Items.Count());
            Assert.True(content.HasNextPage);
            Assert.False(content.HasPreviousPage);
            Assert.Equal(7, content.Items.ToList()[4].ID);
            Assert.Equal(1, content.Items.ToList()[0].ID);
        }

        [Fact]
        public async Task Get_GetComments_ReturnsPaginatedDTO_SecondPage()
        {
            string request = "/api/comments?postID=1&page=2";
            var response = await _client.GetAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            var content = await response.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();
            Assert.Equal(5, content.Items.Count());
            Assert.False(content.HasNextPage);
            Assert.True(content.HasPreviousPage);
            Assert.Equal(8, content.Items.ToList()[0].ID);
            Assert.Equal(12, content.Items.ToList()[4].ID);
        }

        [Fact]
        public async Task Get_GetComments_ReturnsPaginatedDTO_WithSearch()
        {
            string request = "/api/comments?postID=1&search=special";
            var response = await _client.GetAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            var content = await response.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();
            Assert.Single(content.Items);
            Assert.False(content.HasNextPage);
            Assert.False(content.HasPreviousPage);
            Assert.Equal(8, content.Items.ToList()[0].ID);
        }

        [Fact]
        public async Task Get_GetReplies_ReturnsEmpty()
        {
            string request = "/api/comments/1?postid=1";
            var response = await _client.GetAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            var content = await response.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();
            Assert.Empty(content.Items);
        }

        [Fact]
        public async Task Get_GetReplies_ReturnsPaginated()
        {
            string request = "/api/comments/3?postid=1";
            var response = await _client.GetAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            var content = await response.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();

            Assert.Equal(2, content.Items.Count());
            Assert.False(content.HasNextPage);
            Assert.False(content.HasPreviousPage);
            Assert.Equal(4, content.Items.ToList()[0].ID);
        }

        [Fact]
        public async Task Put_PutComment_ReturnsSuccess()
        {
            string request = "/api/comments?postID=1";
            var response = await _client.GetAsync(request);
            var content = await response.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();
            var comm = content.Items.ToList()[0];
            comm.Content = "Update";

            var sendEdited = await _client.PutAsJsonAsync("/api/comments/1", comm);

            sendEdited.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.NoContent, sendEdited.StatusCode);

            response = await _client.GetAsync(request);
            content = await response.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();
            comm = content.Items.ToList()[0];

            Assert.Equal("Update", comm.Content);

        }

        [Fact]
        public async Task Put_PutComment_ReturnsBadRequestWhenIDisWrong()
        {
            string request = "/api/comments?postID=1";
            var response = await _client.GetAsync(request);
            var content = await response.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();
            var comm = content.Items.ToList()[0];
            comm.Content = "Update";

            var sendEdited = await _client.PutAsJsonAsync("/api/comments/100", comm);
            Assert.Equal("application/problem+json", sendEdited.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.BadRequest, sendEdited.StatusCode);
        }

        [Fact]
        public async Task Put_PutComment_ReturnsBadRequestWhenModelIsInvalid()
        {
            string request = "/api/comments?postID=1";
            var response = await _client.GetAsync(request);
            var content = await response.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();
            var comm = content.Items.ToList()[0];
            comm.Content = null;

            var sendEdited = await _client.PutAsJsonAsync("/api/comments/1", comm);
            Assert.Equal("application/problem+json", sendEdited.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.BadRequest, sendEdited.StatusCode);
        }

        [Fact]
        public async Task Post_PostComment_ReturnsSuccess()
        {
            string request = "/api/comments";
            var comm = new CommentsDTO()
            {   
                PostID = 1,
                Content = "This is an added comment"
            };

            var response = await _client.PostAsJsonAsync(request, comm);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var updatedResponse = await _client.GetAsync("/api/comments?postid=1&page=3");
            var content = await updatedResponse.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();
            var addedComment = content.Items.ToList().Last();

            Assert.Equal(comm.Content, addedComment.Content);
        }

        [Fact]
        public async Task Post_PostCommentReply_ReturnsSuccess()
        {
            string request = "/api/comments";
            var comm = new CommentsDTO()
            {
                PostID = 1,
                Content = "This is a reply comment",
                ParentID = 1
            };

            var response = await _client.PostAsJsonAsync(request, comm);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var updatedResponse = await _client.GetAsync("/api/comments/1?postid=1");
            var content = await updatedResponse.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();
            var addedComment = content.Items.ToList().Last();

            Assert.Equal(comm.Content, addedComment.Content);
        }

        [Fact]
        public async Task Post_PostComment_ReturnsBadRequest()
        {
            string request = "/api/comments";
            var comm = new CommentsDTO()
            {
                PostID = 1,
                Content = null
            };

            var response = await _client.PostAsJsonAsync(request, comm);

            Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteComment_ReturnsSuccess()
        {
            string request = "/api/comments/13";
            var response = await _client.DeleteAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var updatedResponse = await _client.GetAsync("/api/comments?postid=1&page=2");
            var content = await updatedResponse.Content.ReadAsAsync<PaginatedDTO<CommentsDTO>>();
            var comm = content.Items.ToList().Last();

            Assert.Equal(12, comm.ID);
            Assert.False(content.HasNextPage);
        }
    }
}
