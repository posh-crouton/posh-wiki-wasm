using System.Reflection;
using Wiki.Posh.Wasm.Services;

namespace Wiki.Posh.Wasm.Pages.Blog;

public partial class BlogIndex
{
    public List<BlogPost> Posts { get; } = [];

    protected override void OnInitialized()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        IEnumerable<string> resourceNames = assembly.GetManifestResourceNames()
            .Where(x => x.Contains("Blog"));

        resourceNames = resourceNames.Reverse();
        
        foreach (string resourceName in resourceNames)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                continue;
            }

            string identifier = string.Join('.', resourceName.Split('.').TakeLast(2));
            Dictionary<string, string> headers = MarkdownHeaderReader.ReadFromStream(stream);
            string title = headers.GetValueOrDefault("title", "[Unknown title]");
            string[] tags = headers.GetValueOrDefault("tags", string.Empty).Split(' ');
            Posts.Add(new BlogPost
            {
                Title = title,
                Uri = $"/blog/{identifier}",
                Tags = tags
            });
        }

        base.OnInitialized();
    }

    public class BlogPost
    {
        public string Title { get; init; } = string.Empty;
        public string Uri { get; init; } = string.Empty;
        public string[] Tags { get; init; } = [];

        public string JoinedTags => string.Join(' ', Tags);
    }
}