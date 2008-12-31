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
using SharpDevelop.Samples.XamlDesigner.Properties;
using System.ComponentModel;
using Microsoft.Win32;
using AvalonDock;
using System.IO;
using System.Collections.Specialized;
using System.Globalization;
using SharpDevelop.XamlDesigner;

namespace SharpDevelop.Samples.XamlDesigner
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			Instance = this;
			DataContext = Shell.Instance;

			InitializeComponent();

			AvalonDockWorkaround();
			LoadSettings();
			ProcessPaths(App.Args);

			Shell.Instance.Open("../../TestFiles/6.xaml");
		}

		public static MainWindow Instance;

		OpenFileDialog openFileDialog;
		SaveFileDialog saveFileDialog;

		void RecentFiles_Click(object sender, RoutedEventArgs e)
		{
			var path = (string)(e.OriginalSource as MenuItem).Header;
			Shell.Instance.Open(path);
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

        void ProcessDrag(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            foreach (var path in e.Data.Paths()) {
				if (HasXamlExtension(path)) {
                    e.Effects = DragDropEffects.Copy;
                    break;
                }
            }
        }

        void ProcessPaths(IEnumerable<string> paths)
        {
            foreach (var path in paths) {
				if (HasXamlExtension(path)) {
                    Shell.Instance.Open(path);
                }
            }
		}

		public static bool HasXamlExtension(string filePath)
		{
			return System.IO.Path.GetExtension(filePath).Equals(".xaml", StringComparison.InvariantCultureIgnoreCase);
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

			if (!string.IsNullOrEmpty(Settings.Default.AvalonDockLayout)) {
				uxDockingManager.RestoreLayout(Settings.Default.AvalonDockLayout.ToStream());
			}

			if (!string.IsNullOrEmpty(Settings.Default.PaletteData)) {
				uxPalette.LoadData(Settings.Default.PaletteData);
			}
			else {
				uxPalette.ResetData();
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

			Settings.Default.PaletteData = uxPalette.SaveData();

			Shell.Instance.SaveSettings();
		}

		#region AvalonDockWorkaround

        void AvalonDockWorkaround()
        {
            uxDocumentPane.Items.KeepSyncronizedWith(Shell.Instance.Documents, d => CreateContentFor(d));
        }

        DocumentContent CreateContentFor(ShellDocument doc)
        {
            var content = new DocumentContent() {
                DataContext = doc,
                Content = doc.View
            };
            content.SetBinding(DocumentContent.TitleProperty, "Title");
            return content;
        }

        #endregion

		#region DockableContentVisibility

		public static DockableContentVisibility GetDockableContentVisibility(DependencyObject obj)
		{
			return (DockableContentVisibility)obj.GetValue(DockableContentVisibilityProperty);
		}

		public static void SetDockableContentVisibility(DependencyObject obj, DockableContentVisibility value)
		{
			obj.SetValue(DockableContentVisibilityProperty, value);
		}

		public static readonly DependencyProperty DockableContentVisibilityProperty =
			DependencyProperty.RegisterAttached("DockableContentVisibility", typeof(DockableContentVisibility), typeof(MainWindow));

		public static bool GetAttachDockableContentVisibility(DependencyObject obj)
		{
			return (bool)obj.GetValue(AttachDockableContentVisibilityProperty);
		}

		public static void SetAttachDockableContentVisibility(DependencyObject obj, bool value)
		{
			obj.SetValue(AttachDockableContentVisibilityProperty, value);
		}

		public static readonly DependencyProperty AttachDockableContentVisibilityProperty =
			DependencyProperty.RegisterAttached("AttachDockableContentVisibility", typeof(bool), typeof(MainWindow),
			new PropertyMetadata(AttachDockableContentVisibilityChanged));

		static void AttachDockableContentVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var v = new DockableContentVisibility(d as DockableContent, MainWindow.Instance.uxDockingManager);
			SetDockableContentVisibility(d, v);
		}

		public class DockableContentVisibility : ViewModel
		{
			public DockableContentVisibility(DockableContent content, DockingManager manager)
			{
				this.content = content;
				this.manager = manager;

				var dpd = DependencyPropertyDescriptor.FromProperty(DockableContent.StatePropertyKey.DependencyProperty, typeof(DockableContent));
				dpd.AddValueChanged(content, (s, e) => { RaisePropertyChanged("IsVisible"); });
			}

			DockableContent content;
			DockingManager manager;

			public bool IsVisible
			{
				get
				{
					return content.State != DockableContentState.Hidden;
				}
				set
				{
					if (value) {
						manager.Show(content);
					}
					else {
						manager.Hide(content);
					}
					RaisePropertyChanged("IsVisible");
				}
			}
		}		

		#endregion
	}
}
