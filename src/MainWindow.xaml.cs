using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GitHubLanguageStats;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<RepoInfo> repositories = new List<RepoInfo>();

    private List<KeyValuePair<StackPanel, Rectangle>> langItems =
        new List<KeyValuePair<StackPanel, Rectangle>>();

    public MainWindow()
    {
        InitializeComponent();

        var refresh = new Thread(Refresh);
        refresh.IsBackground = true;
        refresh.Start();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        UpdateLanguageInfo();
    }

    public async void Refresh()
    {
        Application.Current.Dispatcher.Invoke(() =>
        { refreshButton.IsEnabled = false; });

        var json = await GitHubGetRequest(REQUEST_URL);
        var obj = JObject.Parse(json);
        var array = obj.Value<JArray>("items");

        Application.Current.Dispatcher.Invoke(() =>
        {
            repoFilter.Items.Clear();
        });

        repositories.Clear();

        foreach (var item in array)
        {
            var repo = new RepoInfo(item.Value<string>("name"));

            var languageURL = item.Value<string>("languages_url");
            var langJson = await GitHubGetRequest(languageURL);
            var langs = JObject.Parse(langJson);

            foreach (var language in langs)
            {
                var value = language.Value.ToObject<ulong>();

                var keySearch = repo.Languages.Where(x => x.Key == language.Key);

                if (keySearch.Count() > 0)
                {
                    var i = keySearch.First();
                    var index = repo.Languages.IndexOf(i);

                    repo.Languages[index] = new KeyValuePair<string, ulong>(i.Key, i.Value + value);
                }
                else repo.Add(language.Key, value);
            }

            repositories.Add(repo);

            Application.Current.Dispatcher.Invoke(() =>
            {
                repoFilter.Items.Add(repo.Name);
                repoFilter.SelectedItems.Add(repo.Name);
            });

            //Application.Current.Dispatcher.Invoke(UpdateLanguageInfo);
        }

        Application.Current.Dispatcher.Invoke(() =>
        { refreshButton.IsEnabled = true; });
    }

    public string REQUEST_URL { get => string.Format("https://api.github.com/search/repositories?q=user:{0}", AUTHOR); }

    private async Task<string> GitHubGetRequest(string url)
    {
        var handler = new HttpClientHandler();
        var client = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.UserAgent.TryParseAdd(AUTHOR);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GITHUB_AUTH_TOKEN);
        var response = await client.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK)
            return "";

        return await response.Content.ReadAsStringAsync();
    }

    private void UpdateLanguageInfo()
    {
        ulong total = 0;

        var repo = new RepoInfo();
        var items = repoFilter.SelectedItems.Cast<string>();
        var indexes = items.Select(x => repoFilter.Items.IndexOf(x));

        foreach (var index in indexes)
            repo.Join(repositories[index]);

        foreach (var lang in repo.Languages)
            total += lang.Value;

        var unitSize = ActualWidth / total;

        langItems.Clear();
        languageBar.Children.Clear();
        languageList.Children.Clear();

        foreach (var lang in repo.Languages)
        {
            var percentage = (100d / total) * lang.Value;
            percentage = Math.Round(percentage * 100) / 100;

            string hexColor;
            switch (lang.Key)
            {
                case "C": hexColor = "555555"; break;
                case "C++": hexColor = "F34B7D"; break;
                case "C#": hexColor = "178600"; break;
                case "F#": hexColor = "B845FC"; break;
                case "VisualBasic": hexColor = "945DB7"; break;
                case "JavaScript": hexColor = "F1E05A"; break;
                case "Python": hexColor = "3572A5"; break;
                case "Lua": hexColor = "000080"; break;
                case "Rust": hexColor = "DEA584"; break;
                case "Ruby": hexColor = "701516"; break;
                case "Squirrel": hexColor = "800000"; break;
                case "Haxe": hexColor = "DF7900"; break;
                case "Go": hexColor = "00ADD8"; break;
                case "Vue": hexColor = "2C3E50"; break;
                case "TypeScript": hexColor = "3178C6"; break;
                case "Assembly": hexColor = "6E4C13"; break;
                case "HTML": hexColor = "E34C26"; break;
                case "CSS": hexColor = "663399"; break;
                case "PHP": hexColor = "4F5D95"; break;
                default: hexColor = "000000"; break;
            }

            var color = (Color)ColorConverter.ConvertFromString("#" + hexColor);
            var colorBrush = new SolidColorBrush(hexColor == "000000" ? Colors.Transparent : color);

            var textStack = new StackPanel();
            textStack.Orientation = Orientation.Horizontal;
            textStack.MouseDown += SelectItem;
            languageList.Children.Add(textStack);

            var circle = new Ellipse();
            circle.Fill = colorBrush;
            circle.Width = 12;
            circle.Height = 12;
            circle.Margin = new Thickness(2);
            textStack.Children.Add(circle);

            var text = new Label();
            text.Content = string.Format("{0} ({1}%)", lang.Key, percentage);
            textStack.Children.Add(text);

            var box = new Rectangle();
            box.Fill = colorBrush;
            box.Width = unitSize * lang.Value;
            box.MouseDown += SelectItem;
            languageBar.Children.Add(box);

            langItems.Add(new KeyValuePair<StackPanel, Rectangle>(textStack, box));
        }
    }

    private const float UNSELECTED_OPACITY = 0.64f;

    private void SelectItem(object sender, MouseButtonEventArgs e)
    {
        foreach (var item in langItems)
        {
            bool selected = item.Value == sender || item.Key == sender;

            item.Value.Fill.Opacity = selected ? 1 : UNSELECTED_OPACITY;
            item.Key.Opacity = selected ? 1 : UNSELECTED_OPACITY;
        }
    }

    private void RepoFilter_Updated(object sender, SelectionChangedEventArgs e)
    {
        var selected = repoFilter.SelectedItems.Cast<string>();
        repoFilterDropdownBtn.Content = string.Join(", ", selected);

        if (repositories.Count > 0)
            UpdateLanguageInfo();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
        => Refresh();

    private void RepoFilterDropdown_Click(object sender, RoutedEventArgs e)
        => repoFilterDropdown.IsOpen = !repoFilterDropdown.IsOpen;
}
