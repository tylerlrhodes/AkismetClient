using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace TylerRhodes.Akismet
{
  /// <summary>
  ///   Akismet client for interfacing with Akismet public anti-spam API over http
  /// </summary>
  public class AkismetClient
  {
    /// <summary>
    ///   Akismet Key Verification url
    /// </summary>
    private const string KeyVerificationUrl = "https://rest.akismet.com/1.1/verify-key";

    /// <summary>
    ///   Akismet comment check url format string
    /// </summary>
    private const string CommentCheckUrlFormat = "https://{0}.rest.akismet.com/1.1/comment-check";

    /// <summary>
    ///   Akismet Submit Spam url format string
    /// </summary>
    private const string SubmitSpamUrlFormat = "https://{0}.rest.akismet.com/1.1/submit-spam";

    /// <summary>
    ///   Akismet Submit Ham url format string
    /// </summary>
    private const string SubmitHamUrlFormat = "https://{0}.rest.akismet.com/1.1/submit-ham";

    /// <summary>
    ///   HttpClient to use, submitted through constructor
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    ///   Akismet key verification uri
    /// </summary>
    private readonly Uri _keyVerificationUri = new Uri(KeyVerificationUrl);

    /// <summary>
    ///   api key
    /// </summary>
    public readonly string ApiKey;

    /// <summary>
    ///   blog url
    /// </summary>
    public readonly string BlogUrl;

    /// <summary>
    ///   Akistmet comment check uri
    /// </summary>
    private Uri _commentCheckUri;

    /// <summary>
    ///   Akismet submit ham uri
    /// </summary>
    private Uri _submitHamUri;

    /// <summary>
    ///   Akismet submit spam uri
    /// </summary>
    private Uri _submitSpamUri;

    /// <summary>
    ///   Option to throw an exception on a non 2xx http response
    /// </summary>
    private readonly bool _throwOnInvalidHttpStatus;

    /// <inheritdoc />
    /// <summary>
    ///   AkismetClient constructor
    /// </summary>
    /// <param name="blogUrl">url of blog</param>
    /// <param name="apiKey">Akismet API Key</param>
    /// <param name="httpClient">HttpClient to use</param>
    public AkismetClient(string blogUrl, string apiKey, HttpClient httpClient)
      : this(blogUrl, apiKey, httpClient, false)
    {
    }

    /// <summary>
    ///   Akismet Constructor with option to throw on non 2xx http response
    /// </summary>
    /// <param name="blogUrl">url of blog</param>
    /// <param name="apiKey">Akismet API Key</param>
    /// <param name="httpClient">HttpClient to use</param>
    /// <param name="throwOnInvalidHttpStatus">Whether to throw an exception on non 2xx http responses</param>
    public AkismetClient(string blogUrl, string apiKey, HttpClient httpClient, bool throwOnInvalidHttpStatus)
    {
      BlogUrl = blogUrl ?? throw new ArgumentNullException(nameof(blogUrl));
      ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

      _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

      _throwOnInvalidHttpStatus = throwOnInvalidHttpStatus;

      GenerateUris();
    }

    /// <summary>
    ///   Generates URIs for use by AkismetClient
    /// </summary>
    private void GenerateUris()
    {
      _commentCheckUri = new Uri(string.Format(CommentCheckUrlFormat, ApiKey));
      _submitSpamUri = new Uri(string.Format(SubmitSpamUrlFormat, ApiKey));
      _submitHamUri = new Uri(string.Format(SubmitHamUrlFormat, ApiKey));
    }

    /// <summary>
    ///   Verifies API Key
    /// </summary>
    /// <returns>true for valid key</returns>
    public async Task<bool> CheckApiKey()
    {
      var parameters = new Dictionary<string, string>
      {
        {"key", HttpUtility.UrlEncode(ApiKey)},
        {"blog", HttpUtility.UrlEncode(BlogUrl)}
      };

      var content = new FormUrlEncodedContent(parameters);

      var httpResponseMessage = await _httpClient.PostAsync(_keyVerificationUri, content).ConfigureAwait(false);

      if (_throwOnInvalidHttpStatus)
        httpResponseMessage.EnsureSuccessStatusCode();

      var result = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

      return result == "valid";
    }

    /// <summary>
    ///   Submits a comment as spam to Akismet
    /// </summary>
    /// <param name="comment">Spam comment to submit</param>
    /// <returns></returns>
    public async Task SubmitSpam(AkismetComment comment)
    {
      await Submit(comment, _submitSpamUri).ConfigureAwait(false);
    }

    /// <summary>
    ///   Submits a comment as ham to Akismet
    /// </summary>
    /// <param name="comment">Comment to submit as ham</param>
    /// <returns></returns>
    public async Task SubmitHam(AkismetComment comment)
    {
      await Submit(comment, _submitHamUri).ConfigureAwait(false);
    }

    /// <summary>
    ///   Checks a comment against Akismet to see if it is spam
    /// </summary>
    /// <param name="comment">Comment to check</param>
    /// <returns>true if comment is spam</returns>
    public async Task<bool> IsCommentSpam(AkismetComment comment)
    {
      return await Submit(comment, _commentCheckUri).ConfigureAwait(false) == "true";
    }

    /// <summary>
    ///   Submits a comment to the given URI
    /// </summary>
    /// <param name="comment">Comment to submit</param>
    /// <param name="uri">URI to submit to</param>
    /// <returns>string of response</returns>
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

      var parameters = new Dictionary<string, string>
      {
        {"blog", HttpUtility.UrlEncode(blog)},
        {"user_ip", HttpUtility.UrlEncode(user_ip)},
        {"user_agent", HttpUtility.UrlEncode(user_agent)},
        {"referrer", HttpUtility.UrlEncode(referrer)},
        {"permalink", HttpUtility.UrlEncode(permalink)},
        {"comment_type", HttpUtility.UrlEncode(comment_type)},
        {"comment_author", HttpUtility.UrlEncode(comment_author)},
        {"comment_author_email", HttpUtility.UrlEncode(comment_author_email)},
        {"comment_author_url", HttpUtility.UrlEncode(comment_author_url)},
        {"comment_content", HttpUtility.UrlEncode(comment_content)},
        {"comment_date_gmt", HttpUtility.UrlEncode(comment_date_gmt)},
        {"comment_post_modified_gmt", HttpUtility.UrlEncode(comment_post_modified_gmt)},
        {"blog_lang", HttpUtility.UrlEncode(blog_lang)},
        {"blog_charset", HttpUtility.UrlEncode(blog_charset)},
        {"user_role", HttpUtility.UrlEncode(user_role)},
        {"is_test", HttpUtility.UrlEncode(is_test)}
      };

      var content = new FormUrlEncodedContent(parameters);

      var httpResponseMessage = await _httpClient.PostAsync(uri, content).ConfigureAwait(false);

      if (_throwOnInvalidHttpStatus)
        httpResponseMessage.EnsureSuccessStatusCode();

      var result = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

      return result;
    }
  }
}