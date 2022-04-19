using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace tinyMangaViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand ZoomInCommand { get; } = new RoutedCommand();
        public static RoutedCommand ZoomOutCommand { get; } = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void ZoomIn(object sender, ExecutedRoutedEventArgs e)
        {
            /*
            var child = viewbox.Child as FrameworkElement;
            var transform = child.LayoutTransform;
            var factor = viewbox.ActualWidth / child.ActualWidth;

            viewbox.Stretch = Stretch.None;
            child.RenderTransform = new ScaleTransform(factor, factor, child.ActualWidth / 2, child.ActualHeight / 2);
            */
        }

        private void ZoomOut(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
