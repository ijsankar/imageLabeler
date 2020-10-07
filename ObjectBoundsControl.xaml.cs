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
        public string Label;
        public double h;
        public double w;
        public ObjectBoundsControl(WorkSpaceControl parent, double width =0, double height = 0, String label="")
        {
            this.InitializeComponent();
            this.parent = parent;
            ResizeFreeVertex(width, height);
            Rename(label);
        }
        public void ResizeFreeVertex(double width, double height)
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
            w = width;
            box.Height = height;
            h = height;
            Canvas.SetLeft(freeVertex, width-r);
            Canvas.SetTop(freeVertex, height-r);
            Canvas.SetLeft(LabelTextBox, (width - LabelTextBox.Width) / 2);
            Canvas.SetTop(LabelTextBox, (height - LabelTextBox.Height) / 2);
        }
        //public void ResizeRootVertex(double x, double y)

        public void Rename(String s)
        {
            LabelTextBox.Text = s;
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

        private void LabelTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            parent.obc = this;
            Label = LabelTextBox.Text;
            parent.saveData();
        }
    }
}
