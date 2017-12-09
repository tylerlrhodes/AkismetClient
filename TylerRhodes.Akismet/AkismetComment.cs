using System;

namespace TylerRhodes.Akismet
{
  public class AkismetComment
  {
    public string Blog { get; set; }
    public string UserIp { get; set; }
    public string UserAgent { get; set; }
    public string Referrer { get; set; }
    public string Permalink { get; set; }
    public string CommentType { get; set; }
    public string Author { get; set; }
    public string AuthorEmail { get; set; }
    public string AuthorUrl { get; set; }
    public string Content { get; set; }
    public string DateGmt { get; set; } = DateTime.UtcNow.ToString("O");
    public string PostModifiedTimeGmt { get; set; }
    public string BlogLang { get; set; } = "en-us";
    public string BlogCharset { get; set; } = "UTF-16";
    public string UserRole { get; set; } = "user";
    public string IsTest { get; set; }
  }
}