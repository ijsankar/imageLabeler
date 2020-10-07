using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace imageLabeler
{
    public sealed partial class WorkSpaceControl : UserControl
    {
        public bool IsRootPointerPressed = false;
        public bool IsFreePointerPressed = false;
        private bool isPointerPressed = false;
        public ObjectBoundsControl obc;
        private int sampleDataIndex = 0;
        private int dataSize;
        private List<SampleData> sampleList;
        private List<ObjectBoundsControl> dataObjectList;
        public WorkSpaceControl(ref List<SampleData> sampleDataList)
        {
            this.InitializeComponent();
            sampleList = sampleDataList;
            dataSize = sampleDataList.Count;
            NewSample();
        }
        private void NewSample()
        {
            if (dataObjectList != null)
            {
                foreach (var obj in dataObjectList)
                {
                    WorkSpaceCanvas.Children.Remove(obj);
                }
            }
            dataObjectList = new List<ObjectBoundsControl>();
            foreach (var t in sampleList[sampleDataIndex].SampleBoundsList)
            {
                var x = t.Item1;
                var y = t.Item2;
                var w = t.Item3-x;
                var h = t.Item4-y;
                var s = t.Item5;
                AddObjectBoundsControl(x, y, w, h, s);
            }
        }
        public void AddObjectBoundsControl(double x, double y, double w =0, double h =0 ,string s ="")
        {
            obc = new ObjectBoundsControl(this,w,h,s);
            WorkSpaceCanvas.Children.Add(obc);
            MoveObjectBoundsControl(x, y);
            dataObjectList.Add(obc);
        }

        public void MoveObjectBoundsControl(double x, double y)
        {
            Canvas.SetTop(obc, y);
            Canvas.SetLeft(obc, x);
        }
        private async void SetWorkSpaceCanvasBg()
        {
            using (var stream = await sampleList[sampleDataIndex].File.OpenReadAsync())
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.DecodePixelHeight = 500;
                bitmapImage.DecodePixelWidth = 500;
                await bitmapImage.SetSourceAsync(stream);
                WorkSpaceBgImage.Source = bitmapImage;
            }
        }
        private void SetNavButtonsVisibility()
        {
            if (sampleDataIndex <= 0)
            {
                LeftNavButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                LeftNavButton.Visibility = Visibility.Visible;
            }
            if (sampleDataIndex >= dataSize - 1)
            {
                RightNavButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                RightNavButton.Visibility = Visibility.Visible;
            }
        }
        private void WorkSpaceCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isPointerPressed = true;
            var point = e.GetCurrentPoint(WorkSpaceCanvas).RawPosition;
            if (!(IsRootPointerPressed | IsFreePointerPressed))
            {
                AddObjectBoundsControl(point.X, point.Y);
            }
        }

        private void WorkSpaceCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isPointerPressed)
            {
                if (IsRootPointerPressed)
                {
                    var point = e.GetCurrentPoint(WorkSpaceCanvas).RawPosition;
                    MoveObjectBoundsControl(point.X, point.Y);
                }
                else
                {
                    var point = e.GetCurrentPoint(obc).RawPosition;
                    obc.ResizeFreeVertex(point.X, point.Y);
                }
            }
        }

        private void WorkSpaceCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPointerPressed = false;
            IsRootPointerPressed = false;
            IsFreePointerPressed = false;
            saveData();
            
        }

        private void WorkSpaceCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            SetWorkSpaceCanvasBg();
            SetNavButtonsVisibility();
        }

        private void RightNavButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataObjectList.Count!=0)
            {
              //  saveData();
            }
            sampleDataIndex += 1;
            SetWorkSpaceCanvasBg();
            SetNavButtonsVisibility();
            NewSample();
        }

        private void LeftNavButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataObjectList.Count != 0)
            {
               // saveData();
            }
            sampleDataIndex -= 1;
            SetWorkSpaceCanvasBg();
            SetNavButtonsVisibility();
            NewSample();
        }

        
        public void saveData()
        {
            var x = Canvas.GetLeft(obc);
            var y = Canvas.GetTop(obc);
            var w = obc.w+x;
            var h = obc.h+y;
            var s = obc.Label;
            var t = Tuple.Create(x, y, w, h, s);
            var obcIndex = dataObjectList.FindIndex(item=>item==obc);
            var list = sampleList[sampleDataIndex].SampleBoundsList;
            if (obcIndex>=list.Count)
            {
                list.Add(t);
            }
            else
            {
                list[obcIndex] = t;
            }
        }
    }
}
