using BlogApp.Dotnet.API.IntegrationTests.Helpers;
using BlogApp.Dotnet.ApplicationCore.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BlogApp.Dotnet.API.IntegrationTests
{
    //To Do After Autorization and Authentication:
    //Add tests for authenticated user (create/edit/delete post)
    //Add new tests for forbiden/redirects for put/post/delete -check if a user can modify another image, etc
    //Add tests for roles: admin/user/owner
    //and last but not least, repair tests and delete this comment

    public class ImageTests : IClassFixture<CustomWebApplicationFactoryImage<Startup>>
    {
        private readonly CustomWebApplicationFactoryImage<Startup> _factory;
        private readonly HttpClient _client;

        public ImageTests(CustomWebApplicationFactoryImage<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Put_Put_ReturnsSuccess()
        {
            var imageGenerator = new HttpClient();
            var randomImage = await imageGenerator.GetAsync("https://picsum.photos/200");
            var imageFile = await randomImage.Content.ReadAsStreamAsync();

            var post = new BlogPostDTO()
            {
                Title = "Post with Image",
                Content = "Some text here"
            };

            var createPostRequest = await _client.PostAsJsonAsync("/api/posts", post);
            var createdPost = await createPostRequest.Content.ReadAsAsync<BlogPostDTO>();
            var postID = createdPost.ID;

            Assert.Null(createdPost.ImageURL);

            var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent($"{postID}"), "PostID");
            formContent.Add(new StreamContent(imageFile), "File", "test.jpg");
            var response = await _client.PutAsync($"/api/image/{postID}", formContent);
            response.EnsureSuccessStatusCode();

            var postRequest = await _client.GetAsync($"/api/posts/{postID}");
            var updatedPost = await postRequest.Content.ReadAsAsync<BlogPostDTO>();

            Assert.Equal($"/Assets/Uploads/{postID}.jpg", updatedPost.ImageURL);
            await _client.DeleteAsync($"/api/posts/{postID}");
        }

        [Fact]
        public async Task Put_Put_ReturnsNotFound()
        {
            var postID = 100;

            var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent($"{postID}"), "PostID");
            var response = await _client.PutAsync($"/api/image/{postID}", formContent);

            Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Put_Put_BadRequest()
        {
            var postID = 1;

            var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent($"{postID}"), "PostID");
            var response = await _client.PutAsync($"/api/image/{postID}", formContent);

            Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}