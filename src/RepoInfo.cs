namespace GitHubLanguageStats;

struct RepoInfo
{
    public string Name { get; } = "";

    public KeyValuePair<string, ulong>[] Languages
    { get => languages.ToArray(); }

    private List<KeyValuePair<string, ulong>> languages =
        new List<KeyValuePair<string, ulong>>();

    public RepoInfo() { }

    public RepoInfo(string name) => Name = name;

    private void Update()
    {
        languages = languages.OrderByDescending(x => x.Value).ToList();
    }

    public void Add(string key, ulong value)
    {
        var search = Languages.FirstOrDefault(x => x.Key == key);
        var index = Languages.IndexOf(search);

        if (index < 0)
        {
            languages.Add(new KeyValuePair<string, ulong>(key, value));
        }
        else
        {
            var newValue = Languages[index].Value + value;
            languages[index] = new KeyValuePair<string, ulong>(key, newValue);
        }

        Update();
    }

    public void Join(RepoInfo repo)
    {
        foreach (var lang in repo.languages)
            Add(lang.Key, lang.Value);
    }
}
