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
            ConsoleBox.Text = "using System;\n\npublic class Program\n{\n    public static void Main()\n    {\n        Console.WriteLine(\"Hello World!\");\n    }\n}";
            Initialize();
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

        private async void Initialize()
        {
            await localFolder.CreateFileAsync("Result.txt", CreationCollisionOption.ReplaceExisting);
        }

        private async void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunBtn.IsEnabled = false;
                StorageFile cSharpFile = await localFolder.CreateFileAsync("Program.cs", CreationCollisionOption.ReplaceExisting);
                StorageFile resultFile = await localFolder.GetFileAsync("Result.txt");
                await FileIO.WriteTextAsync(cSharpFile, ConsoleBox.Text);
                await tClient.Send($"{Package.Current.Installed­Location.Path}\\C#DE-Mobile.bat \"{cSharpFile.Path}\" >>\"{resultFile.Path}\"");
                var temp = string.Empty;
                while (true)
                {
                    await Task.Delay(200);
                    var output = await FileIO.ReadTextAsync(resultFile);
                    if (temp != output)
                    {
                        temp = output;
                        OutputScroll.ChangeView(0.0f, OutputScroll.ScrollableHeight, 1.0f);
                        OutputBox.Text = output;
                    }
                    var result = output.Split('\n')[output.Split('\n').Length - 2];
                    if (result.Contains("Excecuted successfully.") || result.Contains("Excecuted unsuccessfully."))
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                OutputBox.Text += "Excecuted unsuccessfully.\n\n";
                _ = new MessageDialog(ex.Message + "\n" + ex.StackTrace).ShowAsync();
            }
            RunBtn.IsEnabled = true;
        }
    }
}
