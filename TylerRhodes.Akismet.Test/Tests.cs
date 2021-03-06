using System;
using Xunit;
using TylerRhodes.Akismet;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace TylerRhodes.Akismet.Test
{
  public class Tests
  {
    private readonly string _apiKey;
    private readonly HttpClient _http;

    public Tests()
    {
      var builder = new ConfigurationBuilder();

      builder.AddUserSecrets<Tests>();

      var config = builder.Build();

      _apiKey = config["ApiKey"];

      _http = new HttpClient();
    }

    [Fact]
    public async Task CommentIsNotSpamAsync()
    {
      var comment = new AkismetComment()
      {
        UserIp = "127.0.0.1",
        UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36",
        Blog = "http://www.bagombo.org",
        Referrer = "http://www.google.com",
        Permalink = "http://www.bagombo.org/not-spam",
        Author = "Tyler Rhodes",
        AuthorEmail = "tylerlrhodes@gmail.com",
        CommentType = "comment",
        Content = "This is a test that should not be marked as spam due to the author value.  Sometimes it seems to come back as spam... Not sure why"
      };

      var akismetClient = new AkismetClient(comment.Blog, _apiKey, _http);

      var isSpam = await akismetClient.IsCommentSpam(comment);

      Assert.False(isSpam);
    }

    [Fact]
    public async Task CommentIsSpamAsync()
    {
      var comment = new AkismetComment()
      {
        UserIp = "127.0.0.1",
        UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36",
        Blog = "http://www.bagombo.org",
        Author = "viagra-test-123",
        AuthorEmail = "test@test.com",
        CommentType = "comment",
        Content = "This is a test that should be marked as spam due to the author value."
      };

      var akismetClient = new AkismetClient(comment.Blog, _apiKey, _http);

      var isSpam = await akismetClient.IsCommentSpam(comment);
      
      Assert.True(isSpam);
    }
  }
}
