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
        public SampleData CurrentSampleData { get; set; }
        private List<ObjectBoundsControl> dataObjectList;
        public WorkSpaceControl()
        {
            this.InitializeComponent();
            dataObjectList = new List<ObjectBoundsControl>();
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
                    obc.Resize(point.X, point.Y);
                }
            }
        }

        private void WorkSpaceCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPointerPressed = false;
            IsRootPointerPressed = false;
            IsFreePointerPressed = false;
        }

        private async void WorkSpaceCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            using (var stream = await CurrentSampleData.File.OpenReadAsync())
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.DecodePixelHeight = 500;
                bitmapImage.DecodePixelWidth = 500;
                await bitmapImage.SetSourceAsync(stream);
                WorkSpaceBgImage.Source = bitmapImage;
            }
        }
    }
}
