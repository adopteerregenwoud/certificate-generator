using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace BulkToolUI;

public partial class MessageBox : Window
{
    public MessageBox()
    {
        InitializeComponent();
    }

    public MessageBox(string message) : this()
    {
        this.FindControl<TextBlock>("MessageTextBlock")!.Text = message;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnOkClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    public static async Task Show(Window parent, string message)
    {
        var msgBox = new MessageBox(message);
        await msgBox.ShowDialog(parent);
    }
}
