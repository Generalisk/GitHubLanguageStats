using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GitHubLanguageStats;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<KeyValuePair<string, ulong>> languages =
        new List<KeyValuePair<string, ulong>>();

    public MainWindow()
    {
        InitializeComponent();

        var refresh = new Thread(Refresh);
        refresh.IsBackground = true;
        refresh.Start();
    }

    public async void Refresh()
    {
        var json = await GitHubGetRequest(REQUEST_URL);
        var obj = JObject.Parse(json);
        var array = obj.Value<JArray>("items");

        languages.Clear();

        foreach (var item in array)
        {
            var languageURL = item.Value<string>("languages_url");
            var langJson = await GitHubGetRequest(languageURL);
            var langs = JObject.Parse(langJson);

            foreach (var language in langs)
            {
                var value = language.Value.ToObject<ulong>();

                var keySearch = languages.Where(x => x.Key == language.Key);

                if (keySearch.Count() > 0)
                {
                    var i = keySearch.First();
                    var index = languages.IndexOf(i);

                    languages[index] = new KeyValuePair<string, ulong>(i.Key, i.Value + value);
                }
                else
                {
                    languages.Add(new KeyValuePair<string, ulong>(language.Key, value));
                }
            }

            Application.Current.Dispatcher.Invoke(UpdateLanguageInfo);
        }
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

        foreach (var lang in languages)
            total += lang.Value;

        var unitSize = ActualWidth / total;

        languageBar.Children.Clear();
        textPanel.Children.Clear();

        languages = languages.OrderByDescending(x => x.Value).ToList();

        foreach (var lang in languages)
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

            var textStack = new StackPanel();
            textStack.Orientation = Orientation.Horizontal;
            textPanel.Children.Add(textStack);

            var circle = new Ellipse();
            circle.Fill = new SolidColorBrush(hexColor == "000000" ? Colors.Transparent : color);
            circle.Width = 12;
            circle.Height = 12;
            circle.Margin = new Thickness(2);
            textStack.Children.Add(circle);

            var text = new Label();
            text.Content = string.Format("{0} ({1}%)", lang.Key, percentage);
            textStack.Children.Add(text);

            var box = new Rectangle();
            box.Fill = new SolidColorBrush(color);
            box.Width = unitSize * lang.Value;
            languageBar.Children.Add(box);
        }
    }
}
