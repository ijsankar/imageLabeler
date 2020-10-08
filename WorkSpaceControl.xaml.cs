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
using System.Diagnostics;
using System.Threading.Tasks;

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
        private int currentBitmapWidth;
        private int currentBitmapHeight;
        private double shrinkRatio = 1;
        public WorkSpaceControl(ref List<SampleData> sampleDataList)
        {
            this.InitializeComponent();
            sampleList = sampleDataList;
            dataSize = sampleDataList.Count;
            NewSample();
        }
        private async void NewSample()
        {
            await SetWorkSpaceCanvasBg();
            SetNavButtonsVisibility();
            ShowBounds();
        }

        private void ShowBounds()
        {
            if (dataObjectList != null)
            {
                foreach (var obj in dataObjectList)
                {
                    WorkSpaceCanvas.Children.Remove(obj);
                }
            }
            dataObjectList = new List<ObjectBoundsControl>();
            //Debug.WriteLine("renderedimagewidth=" + WorkSpaceBgImage.ActualWidth);
            //Debug.WriteLine("imagewidth=" + WorkSpaceBgImage.Width);
            //Debug.WriteLine("canvasrendwidth=" + WorkSpaceCanvas.ActualWidth);
            //Debug.WriteLine("canvasrendheight=" + WorkSpaceCanvas.ActualHeight);
            //Debug.WriteLine("canvaswidth=" + WorkSpaceCanvas.Width);
            //Debug.WriteLine("gridwidth=" + WorkSpaceGrid.Width);
            //Debug.WriteLine("gridrendwidth=" + WorkSpaceGrid.ActualWidth);
            //Debug.WriteLine("gridrendheigth=" + WorkSpaceGrid.ActualHeight);
            GetShrinkRatio();
            foreach (var t in sampleList[sampleDataIndex].SampleBoundsList)
            {
                var x = t.Item1 / shrinkRatio;
                var y = t.Item2 / shrinkRatio;
                var w = t.Item3 / shrinkRatio - x;
                var h = t.Item4 / shrinkRatio - y;
                var s = t.Item5;
                AddObjectBoundsControl(x, y, w, h, s);
            }
        }
        private void GetShrinkRatio()
        {
            var w = WorkSpaceGrid.ActualWidth-80;
            var h = WorkSpaceGrid.ActualHeight;
            Debug.WriteLine(w);
            Debug.WriteLine(h);

            if ((w/h)< (currentBitmapWidth / currentBitmapHeight))
            {
                shrinkRatio = currentBitmapWidth / w;
                Debug.WriteLine("yes");
            }
            else
            {
                shrinkRatio = currentBitmapHeight / h;
            }
            Debug.WriteLine(shrinkRatio);
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
            Canvas.SetTop(obc, y>=0?y:0);
            Canvas.SetLeft(obc, x>=0?x:0);
        }
        private async Task SetWorkSpaceCanvasBg()
        {
            using (var stream = await sampleList[sampleDataIndex].File.OpenReadAsync())
            {
                var bitmapImage = new BitmapImage();
                WorkSpaceBgImage.Width = WorkSpaceGrid.Width;
                WorkSpaceBgImage.Height = WorkSpaceGrid.Height;
                shrinkRatio = bitmapImage.PixelWidth;
                await bitmapImage.SetSourceAsync(stream);
                //Debug.WriteLine("pixelwidth=" + bitmapImage.PixelWidth.ToString());
                currentBitmapWidth = bitmapImage.PixelWidth;
                currentBitmapHeight = bitmapImage.PixelHeight;
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

        
        private void RightNavButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToNextWS();
        }

        public void NavigateToNextWS()
        {
            if (sampleDataIndex<dataSize-1)
            {
                sampleDataIndex += 1;
                
                NewSample();
            }
        }
        private void LeftNavButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPrevWS();
        }
        public void NavigateToPrevWS()
        {
            if (sampleDataIndex > 0)
            {
                sampleDataIndex -= 1;
                
                NewSample();
            }
        }
        
        public void saveData()
        {
            var x = Canvas.GetLeft(obc)*shrinkRatio;
            var y = Canvas.GetTop(obc) * shrinkRatio;
            Debug.WriteLine("y1=" + Canvas.GetLeft(obc));
            Debug.WriteLine("yh=" + obc.h);
            var w = x+obc.w * shrinkRatio;
            var h = y+obc.h * shrinkRatio;
            Debug.WriteLine("y2=" + h);
            var s = obc.Label==null?"":obc.Label;
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
        public void DeleteOBC()
        {
            var obcIndex = dataObjectList.FindIndex(item => item == obc);
            var list = sampleList[sampleDataIndex].SampleBoundsList;
            list.RemoveAt(obcIndex);
            dataObjectList.RemoveAt(obcIndex);
            WorkSpaceCanvas.Children.Remove(obc);
            this.obc = null;
        }

        private void WorkSpaceCanvas_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var key = e.Key;
            if ((key == Windows.System.VirtualKey.Left)&(sampleDataIndex > 0))
            {
                NavigateToPrevWS();
                
            }
            else if((key == Windows.System.VirtualKey.Right)&(sampleDataIndex<dataSize-1))
            {
                NavigateToNextWS();
            }
            
        }

        private void WorkSpaceBgImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WorkSpaceCanvas.Width = WorkSpaceBgImage.ActualWidth;
            WorkSpaceCanvas.Height = WorkSpaceBgImage.ActualHeight;
            ShowBounds();
        }
    }
}
