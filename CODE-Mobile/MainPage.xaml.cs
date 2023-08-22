using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinUniversalTool;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CODE_Mobile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TelnetClient tClient = new TelnetClient(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;


        public MainPage()
        {
            this.InitializeComponent();
            ConsoleBox.Text = @"using System;

public class Program
{
    public static void Main()
    {
        Console.WriteLine(""Hello World!"");
    }
}";
            Connect();
        }

        private void Connect()
        {
            _ = tClient.Connect();
            long j = 0;
            while (tClient.IsConnected == false && j < 1000000)
            {
                j++;
            }
            if (tClient.IsConnected)
            {
                RunBtn.IsEnabled = true;
            }
            else
            {
                _ = new MessageDialog("Make sure you have injected CMD in your device using CMD Injecter by Fadil Fadz.").ShowAsync();
            }
        }

        private async void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunBtn.IsEnabled = false;
                OutputScroll.ChangeView(0.0f, OutputScroll.ScrollableHeight, 1.0f);
                OutputBox.Text += "Running...\n";
                var cSharpFile = await localFolder.CreateFileAsync("Program.cs", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(cSharpFile, ConsoleBox.Text);
                File.Delete($"{localFolder.Path}\\Result.txt");
                await tClient.Send($"{Package.Current.Installed­Location.Path}\\C#DE-Mobile.bat \"{cSharpFile.Path}\" >\"{localFolder.Path}\\Result.txt\"");
                string Output = string.Empty;
                while (!Output.Contains("Excecuted successfully.") && !Output.Contains("Excecuted unsuccessfully."))
                {
                    await Task.Delay(200);
                    if (File.Exists($"{localFolder.Path}\\Result.txt")) Output = await FileIO.ReadTextAsync(await localFolder.GetFileAsync("Result.txt"));
                }
                OutputBox.Text += Output + "\n";
                OutputScroll.ChangeView(0.0f, OutputScroll.ScrollableHeight, 1.0f);
                RunBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                OutputBox.Text += "Excecuted unsuccessfully.\n\n";
                RunBtn.IsEnabled = true;
                _ = new MessageDialog(ex.Message + "\n" + ex.StackTrace).ShowAsync();
            }
        }
    }
}
