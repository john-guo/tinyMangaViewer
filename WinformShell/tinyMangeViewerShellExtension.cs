using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformShell
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".zip", ".7z", ".rar")]
    public class tinyMangeViewerShellExtension : SharpContextMenu
    {
        private readonly string exe;

        public tinyMangeViewerShellExtension()
        {
            exe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tinyMangaViewer.exe");
        }

        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            var menuItem = new ToolStripMenuItem
            {
                Text = "Manga View",
            };
            
            menuItem.Click += MenuItem_Click;

            menu.Items.Add(menuItem);

            return menu;
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            var filename = SelectedItemPaths.First();
            Process.Start(new ProcessStartInfo(exe, $"\"{filename}\"")
            {
                UseShellExecute = true,
            });
        }
    }
}
