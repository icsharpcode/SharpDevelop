using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ICSharpCode.XamlDesigner.Properties;
using System.ComponentModel;
using Microsoft.Win32;
using AvalonDock;
using System.IO;
using System.Collections.Specialized;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.Xaml;

namespace ICSharpCode.XamlDesigner
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			Instance = this;
			DataContext = Shell.Instance;
			RenameCommands();

			InitializeComponent();

			Shell.Instance.PropertyGrid = uxPropertyGridView.PropertyGrid;
			AvalonDockWorkaround();
			RegisterCommandHandlers();	

			LoadSettings();
			ProcessPaths(App.Args);
		}

		public static MainWindow Instance;

		OpenFileDialog openFileDialog;
        SaveFileDialog saveFileDialog;

		protected override void OnDragEnter(DragEventArgs e)
		{
			ProcessDrag(e);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			ProcessDrag(e);
		}

		protected override void OnDrop(DragEventArgs e)
		{
			ProcessPaths(e.Data.Paths());
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (Shell.Instance.PrepareExit()) {
				SaveSettings();
			}
			else {
				e.Cancel = true;
			}
			base.OnClosing(e);
		}

		void RecentFiles_Click(object sender, RoutedEventArgs e)
		{
			var path = (string)(e.OriginalSource as MenuItem).Header;
			Shell.Instance.Open(path);
		}

        void ProcessDrag(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            foreach (var path in e.Data.Paths()) {
                if (path.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) ||
                    path.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)) {
                    e.Effects = DragDropEffects.Copy;
                    break;
                }
                else if (XamlConstants.HasXamlExtension(path)) {
                    e.Effects = DragDropEffects.Copy;
                    break;
                }
            }
        }

        void ProcessPaths(IEnumerable<string> paths)
        {
            foreach (var path in paths) {
				//if (path.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) ||
				//    path.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)) {
				//    Toolbox.Instance.AddAssembly(path);
				//}
                //else 
				if (XamlConstants.HasXamlExtension(path)) {
                    Shell.Instance.Open(path);
                }
            }
        }

        public string AskOpenFileName()
        {
            if (openFileDialog == null) {
                openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Xaml Documents (*.xaml)|*.xaml";
            }
            if ((bool)openFileDialog.ShowDialog()) {
				return openFileDialog.FileName;
            }
			return null;
        }

		public string AskSaveFileName(string initName)
		{
			if (saveFileDialog == null) {
				saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "Xaml Documents (*.xaml)|*.xaml";
			}
			saveFileDialog.FileName = initName;
			if ((bool)saveFileDialog.ShowDialog()) {
				return saveFileDialog.FileName;
			}
			return null;
		}

		void LoadSettings()
		{
			WindowState = Settings.Default.MainWindowState;

			Rect r = Settings.Default.MainWindowRect;
			if (r != new Rect()) {
				Left = r.Left;
				Top = r.Top;
				Width = r.Width;
				Height = r.Height;
			}

			if (Settings.Default.AvalonDockLayout != null) {
				uxDockingManager.RestoreLayout(Settings.Default.AvalonDockLayout.ToStream());
			}

			var toolboxContentPath = "WpfToolbox.xaml";
			if (File.Exists(toolboxContentPath)) {
				uxToolbox.Load(File.ReadAllText(toolboxContentPath));
			}
		}

		void SaveSettings()
		{ 
			Settings.Default.MainWindowState = WindowState;
		    if (WindowState == WindowState.Normal) {
		        Settings.Default.MainWindowRect = new Rect(Left, Top, Width, Height);
		    }

			var writer = new StringWriter();
			uxDockingManager.SaveLayout(writer);
			Settings.Default.AvalonDockLayout = writer.ToString();

			Shell.Instance.SaveSettings();
		}

		#region AvalonDockWorkaround

        void AvalonDockWorkaround()
        {
            uxDocumentPane.Items.KeepSyncronizedWith(Shell.Instance.Documents, d => CreateContentFor(d));
        }

        DocumentContent CreateContentFor(Document doc)
        {
            var content = new DocumentContent() {
                DataContext = doc,
                Content = new DocumentView(doc)
            };
            content.SetBinding(DocumentContent.TitleProperty, "Title");
            return content;
        }

        #endregion
	}
}
