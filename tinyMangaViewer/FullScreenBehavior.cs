using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace tinyMangaViewer
{
    public class FullScreenBehavior : Behavior<Window>
    {
        private WindowState windowState;
        private double Left;
        private double Top;
        private double Width;
        private double Height;

        protected override void OnAttached()
        {
            base.OnAttached();

            Left = AssociatedObject.Left;
            Top = AssociatedObject.Top;
            Width = AssociatedObject.Width;
            Height = AssociatedObject.Height;
            windowState = AssociatedObject.WindowState;
        }

        private void Save()
        {
            Left = AssociatedObject.Left;
            Top = AssociatedObject.Top;
            Width = AssociatedObject.Width;
            Height = AssociatedObject.Height;
            windowState = AssociatedObject.WindowState;
        }

        private void Restore()
        {
            AssociatedObject.Left = Left;
            AssociatedObject.Top = Top;
            AssociatedObject.Width = Width;
            AssociatedObject.Height = Height;
            AssociatedObject.WindowState = windowState;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property != Window.WindowStyleProperty)
                return;
            var style = (WindowStyle)e.NewValue;
            var oldStyle = (WindowStyle)e.OldValue;
            if (style == WindowStyle.None)
            {
                Save();
                AssociatedObject.WindowState = WindowState.Maximized;
                AssociatedObject.ResizeMode = ResizeMode.NoResize;
                AssociatedObject.Left = 0;
                AssociatedObject.Top = 0;
                AssociatedObject.Width = SystemParameters.VirtualScreenWidth;
                AssociatedObject.Height = SystemParameters.VirtualScreenHeight;
                AssociatedObject.Topmost = true;
            }
            else if (oldStyle == WindowStyle.None)
            {
                Restore();
                AssociatedObject.ResizeMode = ResizeMode.CanResize;
                AssociatedObject.Topmost = false;
            }
        }
    }
}
