# Akismet Client
The Akismet Client makes it easy to make use of the Akismet API in your .NET application and get started fighting comment spam.  It is currently released in alpha although is fairly stable.

You can get it from Nuget.org by using:

```
Install-Package TylerRhodes.Akismet -Version 1.0.0-alpha
```
From the package manager console in Visual Studio, or through the Manage Dependencies option of your project.

This implementation of the Akismet Client is released as a .NET reference assembly which targets .NET Standard 2.0.  Therefore it should be compatible with any version of .NET which supports .NET Standard 2.0, such as .NET Core 2.0, .NET Framework 4.6.1, and others.

You can read more about .NET Standard 2.0 [here.](https://blogs.msdn.microsoft.com/dotnet/2017/08/14/announcing-net-standard-2-0/)

Use of the client requires a valid Akismet API key, which you can get from Akismet.  There may be a cost for your key depending upon if it is a commercial site.  Free access is supported for personal sites.

You can get your key at [Akismet For Developers.](https://akismet.com/development/)

Using the client is as simple as the code shown below:

```C#

using (var client = new HttpClient())
{
    var akismet = new AkismetClient("http://www.blogurl.com", "your-api-key", client);
    
    var comment = new AkismetComment()
    {
        Blog = "http://www.blogurl.com",
        UserIp = "127.0.0.1",
        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36",
        Referrer = "http://www.google.com",
        Permalink = "http://www.blogurl.com/blogpost123",
        CommentType = "comment",
        Author = "viagra-test-123",
        AuthorEmail = "test@email.com",
        AuthorUrl = "http://www.somewhere.com",
        Content = "This is a test comment"
    };
    
    var isspam = await akismet.IsCommentSpam(comment);
    
    if (isspam)
    {
        // do something
    }
    else
    {
        // do something else
    }
}
```

