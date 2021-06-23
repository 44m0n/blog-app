using BlogApp.Dotnet.API.IntegrationTests.Helpers;
using BlogApp.Dotnet.ApplicationCore.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BlogApp.Dotnet.API.IntegrationTests
{
    //To Do After Autorization and Authentication:
    //Add tests for authenticated user (create/edit/delete post)
    //Add new tests for forbiden/redirects for put/post/delete -check if a user can delete another post, etc
    //Add tests for roles: admin/user/owner
    //be careful not to delete a post as admin/user/etc that has already deleted before
    //and last but not least, repair tests and delete this comment

    public class PostsTests : IClassFixture<CustomWebApplicationFactoryPosts<Startup>>
    {
        private readonly CustomWebApplicationFactoryPosts<Startup> _factory;
        private readonly HttpClient _client;

        public PostsTests(CustomWebApplicationFactoryPosts<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Get_GetBlogPosts_ReturnsAllPaginatedFirstPage()
        {
            string request = "/api/posts";
            var response = await _client.GetAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            var content = await response.Content.ReadAsAsync<PaginatedDTO<BlogPostDTO>>();
            Assert.Equal(5, content.Items.Count());
            Assert.True(content.HasNextPage);
            Assert.False(content.HasPreviousPage);
            Assert.Equal("TestTitle8", content.Items.ToList<BlogPostDTO>()[0].Title);
            Assert.Equal("TestTitle12", content.Items.ToList<BlogPostDTO>()[4].Title);
        }

        [Fact]
        public async Task Get_GetBlogPosts_ReturnsAllPaginatedSecondPage()
        {
            string request = "/api/posts?page=2";
            var response = await _client.GetAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            var content = await response.Content.ReadAsAsync<PaginatedDTO<BlogPostDTO>>();
            Assert.Equal(5, content.Items.Count());
            Assert.True(content.HasNextPage);
            Assert.True(content.HasPreviousPage);
            Assert.Equal("TestTitle13", content.Items.ToList<BlogPostDTO>()[0].Title);
            Assert.Equal("TestTitle17", content.Items.ToList<BlogPostDTO>()[4].Title);
        }

        [Fact]
        public async Task Get_GetBlogPosts_SearchFilterReturnsPaginated()
        {
            string request = "/api/posts?search=TestContent5";
            var response = await _client.GetAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            var content = await response.Content.ReadAsAsync<PaginatedDTO<BlogPostDTO>>();
            Assert.Single(content.Items);
            Assert.False(content.HasNextPage);
            Assert.False(content.HasPreviousPage);
            Assert.Equal("TestTitle5", content.Items.ToList<BlogPostDTO>()[0].Title);
        }

        [Fact]
        public async Task Get_BlogPostDetails()
        {
            string request = "/api/posts/1";
            var response = await _client.GetAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            var content = await response.Content.ReadAsAsync<BlogPostDTO>();
            Assert.Equal("TestTitle1", content.Title);
            Assert.Equal("/Assets/Uploads/1.jpg", content.ImageURL);
        }

        [Fact]
        public async Task Get_BlogPostDetalis_ReturnsNotFound()
        {
            string request = "/api/posts/500";
            var response = await _client.GetAsync(request);

            Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Put_PutBlogPost_Success()
        {
            var responsePost = await _client.GetAsync("/api/posts/1");
            var post = await responsePost.Content.ReadAsAsync<BlogPostDTO>();

            Assert.Equal("TestTitle1", post.Title);
            post.Title = "ModifiedTitle";

            var response = await _client.PutAsJsonAsync("/api/posts/1", post);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            
            var updatedPost = await _client.GetAsync("/api/posts/1");
            post = await updatedPost.Content.ReadAsAsync<BlogPostDTO>();
            Assert.Equal("ModifiedTitle", post.Title);
            Assert.NotEqual(post.CreatedAt, post.ModifiedAt);
        }

        [Fact]
        public async Task Put_PutBlogPost_NotFound_EditedPostDifferentFromRequest()
        {
            var responsePost = await _client.GetAsync("/api/posts/1");
            var post = await responsePost.Content.ReadAsAsync<BlogPostDTO>();

            Assert.Equal("TestTitle1", post.Title);
            post.Title = "ModifiedTitle";

            var response = await _client.PutAsJsonAsync("/api/posts/100", post);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Put_PutBlogPost_NotFound_WhenPostDoesntExistsInDB()
        {
            var responsePost = await _client.GetAsync("/api/posts/1");
            var post = await responsePost.Content.ReadAsAsync<BlogPostDTO>();

            Assert.Equal("TestTitle1", post.Title);
            post.Title = "ModifiedTitle";
            post.ID = 100;

            var response = await _client.PutAsJsonAsync("/api/posts/100", post);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Put_PutBlogPost_BadRequest_ModelInvalid()
        {
            var responsePost = await _client.GetAsync("/api/posts/1");
            var post = await responsePost.Content.ReadAsAsync<BlogPostDTO>();

            Assert.Equal("TestTitle1", post.Title);
            post.Title = null;

            var response = await _client.PutAsJsonAsync("/api/posts/1", post);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Post_PostBlogPost_ReturnsCreatedPost()
        {
            var post = new BlogPostDTO()
            {
                Title = "Integration Title Post",
                Content = "Integration Test Content"
            };

            var response = await _client.PostAsJsonAsync("/api/posts", post);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            var content = await response.Content.ReadAsAsync<BlogPostDTO>();
            Assert.Equal("Integration Title Post", content.Title);
            Assert.Equal("Integration Test Content", content.Content);
        }

        [Fact]
        public async Task Post_PostBlogPost_BadRequestWhenModelIsInvalid()
        {
            var post = new BlogPostDTO()
            {
                Title = null,
                Content = "Integration Test Content"
            };

            var response = await _client.PostAsJsonAsync("/api/posts", post);

            Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteBlogPost_Success()
        {
            int postID = 18;
            var response = await _client.DeleteAsync($"/api/posts/{postID}");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var request = await _client.GetAsync($"/api/posts/{postID}");
            Assert.Equal("application/problem+json", request.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.NotFound, request.StatusCode);

        }

        [Fact]
        public async Task Delete_DeleteBlogPost_NotFound()
        {
            int postID = 200;
            var response = await _client.DeleteAsync($"/api/posts/{postID}");

            Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
