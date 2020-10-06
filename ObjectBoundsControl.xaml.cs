using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace imageLabeler
{
    public sealed partial class ObjectBoundsControl : UserControl
    {
        private int r = 8;
        private WorkSpaceControl parent;
        public ObjectBoundsControl(WorkSpaceControl parent, double width =0, double height = 0, String label="")
        {
            this.InitializeComponent();
            this.parent = parent;
            Resize(width, height);
            Rename(label);
        }
        public void Resize(double width, double height)
        {
            if (width<r)
            {
                width = r;
            }
            if (height<r)
            {
                height = r;
            }
            box.Width = width;
            box.Height = height;
            Canvas.SetLeft(freeVertex, width-r);
            Canvas.SetTop(freeVertex, height-r);
            Canvas.SetLeft(label, (width - label.Width) / 2);
            Canvas.SetTop(label, (height - label.Height) / 2);
        }
        public void Rename(String s)
        {
            label.Text = s;
        }

        public Tuple<double,double,String> GetBoundData()
        {
            return Tuple.Create(box.Width, box.Height, label.Text);
        }
        private void vertex_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Shapes.Rectangle r = sender as Windows.UI.Xaml.Shapes.Rectangle;
            r.Opacity = 100;
        }

        private void vertex_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Shapes.Rectangle r = sender as Windows.UI.Xaml.Shapes.Rectangle;
            r.Opacity = 20;
        }

        private void rootVertex_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            parent.IsRootPointerPressed = true;
            parent.obc = this;
        }

        private void freeVertex_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            parent.IsFreePointerPressed = true;
            parent.obc = this;
        }
    }
}
