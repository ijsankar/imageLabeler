using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace imageLabeler
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public List<SampleData> sampleList;
        bool isDrawMode = true;
        int pointsCount = 4;
        StorageFolder dataFolder;
        public MainPage()
        {
            this.InitializeComponent();
            sampleList = new List<SampleData>();
        }

        private async void browseButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FolderPicker();
            picker.CommitButtonText = "Select Data Location";
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.FileTypeFilter.Add("*");
            StorageFolder folder = await picker.PickSingleFolderAsync();
            configButtonFlyout.ShowAt(configButton);
            if (folder != null)
            {
                folderPathTextBlock.Text = folder.Path;
                dataFolder = folder;
                startButton.Visibility = Visibility.Visible;
            }
        }

        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            await GetSampleListAsync();
        }
        private async Task  GetSampleListAsync()
        {
            foreach (var item in await dataFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByName))
            {
                if (item.FileType==".jpg")
                {
                    sampleList.Add(new SampleData(item));
                }
            }
        }
    }
}
