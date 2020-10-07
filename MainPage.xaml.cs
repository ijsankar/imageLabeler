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
        public List<SampleData> SampleList;
        bool isDrawMode = true;
        int pointsCount = 4;
        StorageFolder dataFolder;
        StorageFile CSVFile;
        StorageFile newCSVFile;
        public MainPage()
        {
            this.InitializeComponent();
            SampleList = new List<SampleData>();
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
                startButton.IsEnabled = true;
            }
        }

        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            await GetSampleListAsync();
            foreach (var wsc in MainGrid.Children.OfType<WorkSpaceControl>())
            {
                MainGrid.Children.Remove(wsc);
            }
            var w = new WorkSpaceControl(ref SampleList);
            MainGrid.Children.Add(w);
            Grid.SetRow(w, 1);
            SaveButton.IsEnabled = true;
            startButton.IsEnabled = false;
        }
        private async Task  GetSampleListAsync()
        {
            foreach (var item in await dataFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByName))
            {
                if (item.FileType==".jpg")
                {
                    SampleList.Add(new SampleData(item, await GetSampleDataFromCSV(item.Name)));
                }
            }
        }

        private async void CSVPathBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.CommitButtonText = "select CSV file";
            picker.FileTypeFilter.Add(".csv");
            CSVFile=await picker.PickSingleFileAsync();
        }
        private async Task<List<Tuple<double,double,double,double,string>>> GetSampleDataFromCSV(string fileName)
        {
            var l = new List<Tuple<double, double, double, double, string>>();
            if (CSVFile == null)
            {
                return l;
            }
            var stream = await CSVFile.OpenStreamForReadAsync();
            var CSVReader = new StreamReader(stream);
            string sample;
            while ((sample = CSVReader.ReadLine())!=null)
            {
                if (sample.Contains(fileName))
                {
                    var x = sample.Split(",");
                    int i = 1;
                    while(i<x.Length)
                    {
                        l.Add(Tuple.Create(double.Parse(x[i]), double.Parse(x[i + 1]), double.Parse(x[i + 2]), double.Parse(x[i + 3]),x[i + 4]));
                        i += 5;
                    }
                    CSVReader.Close();
                    return l;
                } 
            }
            CSVReader.Close();
            return l;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //if (newCSVFile==null)
            //{
            //    var picker = new FileSavePicker();
            //    picker.CommitButtonText="Choose CSV file";
            //    picker.FileTypeChoices.Add("CSV", new List<string>() { ".csv"});
            //    newCSVFile = await picker.PickSaveFileAsync();
            //}
            
            newCSVFile = await dataFolder.CreateFileAsync("data" + DateTime.Now.ToString("yy-dd-MM--HH-mm-ss") + ".csv");
            if(newCSVFile!=null)
            {
                var stream = await newCSVFile.OpenStreamForWriteAsync();
                var writer = new StreamWriter(stream);
                foreach (var sample in SampleList)
                {
                    if (sample.SampleBoundsList.Count>0)
                    {
                        var l = new List<string>();
                        l.Add(sample.File.Name);
                        foreach (var data in sample.SampleBoundsList)
                        {
                            l.Add(data.Item1.ToString());
                            l.Add(data.Item2.ToString());
                            l.Add(data.Item3.ToString());
                            l.Add(data.Item4.ToString());
                            l.Add(data.Item5);
                        }
                        var s = String.Join(',', l);
                        writer.WriteLine(s);
                    }
                }
                writer.Close();
            }
        }
    }
}
