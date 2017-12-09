using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace TylerRhodes.Akismet
{
  public class AkismetClient
  {
    private readonly HttpClient _httpClient;

    private const string KeyVerificationUrl = "https://rest.akismet.com/1.1/verify-key";
    private readonly Uri _keyVerificationUrl = new Uri(KeyVerificationUrl);

    private const string CommentCheckUrlFormat = "https://{0}.rest.akismet.com/1.1/comment-check";
    private Uri _commentCheckUri;

    private const string SubmitSpamUrlFormat = "https://{0}.rest.akismet.com/1.1/submit-spam";
    private Uri _submitSpamUri;

    private const string SubmitHamUrlFormat = "https://{0}.rest.akismet.com/1.1/submit-ham";
    private Uri _submitHamUri;

    public readonly string BlogUrl;
    public readonly string ApiKey;

    public AkismetClient(string blogUrl, string apiKey, HttpClient httpClient)
    {
      BlogUrl = blogUrl ?? throw new ArgumentNullException(nameof(blogUrl));
      ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

      _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

      GenerateUris();
    }

    private void GenerateUris()
    {
      _commentCheckUri = new Uri(string.Format(CommentCheckUrlFormat, ApiKey));
      _submitSpamUri = new Uri(string.Format(SubmitSpamUrlFormat, ApiKey));
      _submitHamUri = new Uri(string.Format(SubmitHamUrlFormat, ApiKey));
    }

    public async Task<bool> CheckApiKey()
    {
      var parameters = new Dictionary<string, string>
      {
        { "key", HttpUtility.UrlEncode(ApiKey) },
        { "blog", HttpUtility.UrlEncode(BlogUrl) }
      };

      var content = new FormUrlEncodedContent(parameters);

      var httpResponseMessage = await _httpClient.PostAsync(_keyVerificationUrl, content);

      var result = await httpResponseMessage.Content.ReadAsStringAsync();

      return result == "valid";
    }

    public async Task SubmitSpam(AkismetComment comment)
    {
      await Submit(comment, _submitSpamUri);
    }

    public async Task SubmitHam(AkismetComment comment)
    {
      await Submit(comment, _submitHamUri);
    }

    public async Task<bool> IsCommentSpam(AkismetComment comment)
    {
      return await Submit(comment, _commentCheckUri) == "true";
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private async Task<string> Submit(AkismetComment comment, Uri uri)
    {
      var blog = comment.Blog ?? BlogUrl;
      var user_ip = comment.UserIp ?? throw new ArgumentNullException(nameof(comment.UserIp));
      var user_agent = comment.UserAgent ?? throw new ArgumentNullException(nameof(comment.UserAgent));
      var referrer = comment.Referrer ?? "";
      var permalink = comment.Permalink ?? "";
      var comment_type = comment.CommentType ?? "";
      var comment_author = comment.Author ?? "";
      var comment_author_email = comment.AuthorEmail ?? "";
      var comment_author_url = comment.AuthorUrl ?? "";
      var comment_content = comment.Content ?? "";
      var comment_date_gmt = comment.DateGmt ?? "";
      var comment_post_modified_gmt = comment.PostModifiedTimeGmt ?? "";
      var blog_lang = comment.BlogLang ?? "";
      var blog_charset = comment.BlogCharset ?? "";
      var user_role = comment.UserRole ?? "";
      var is_test = comment.IsTest ?? "";

      var parameters = new Dictionary<string, string>()
      {
        { "blog", HttpUtility.UrlEncode(blog) },
        { "user_ip", HttpUtility.UrlEncode(user_ip) },
        { "user_agent", HttpUtility.UrlEncode(user_agent) },
        { "referrer", HttpUtility.UrlEncode(referrer) },
        { "permalink", HttpUtility.UrlEncode(permalink) },
        { "comment_type", HttpUtility.UrlEncode(comment_type) },
        { "comment_author", HttpUtility.UrlEncode(comment_author) },
        { "comment_author_email", HttpUtility.UrlEncode(comment_author_email) },
        { "comment_author_url", HttpUtility.UrlEncode(comment_author_url) },
        { "comment_content", HttpUtility.UrlEncode(comment_content) },
        { "comment_date_gmt", HttpUtility.UrlEncode(comment_date_gmt) },
        { "comment_post_modified_gmt", HttpUtility.UrlEncode(comment_post_modified_gmt) },
        { "blog_lang", HttpUtility.UrlEncode(blog_lang) },
        { "blog_charset", HttpUtility.UrlEncode(blog_charset) },
        { "user_role", HttpUtility.UrlEncode(user_role) },
        { "is_test", HttpUtility.UrlEncode(is_test) }
      };

      var content = new FormUrlEncodedContent(parameters);

      var httpResponseMessage = await _httpClient.PostAsync(uri, content);

      var result = await httpResponseMessage.Content.ReadAsStringAsync();

      return result;
    }
  }
}