using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

using Microsoft.Win32;

namespace ICSharpCode.XamlDesigner
{
	public partial class MainWindow
	{
		public static SimpleCommand CloseAllCommand = new SimpleCommand("Close All");
		public static SimpleCommand SaveAllCommand = new SimpleCommand("Save All", ModifierKeys.Control | ModifierKeys.Shift, Key.S);
		public static SimpleCommand ExitCommand = new SimpleCommand("Exit");
		public static SimpleCommand RefreshCommand = new SimpleCommand("Refresh", Key.F5);
		public static SimpleCommand RunCommand = new SimpleCommand("Run", ModifierKeys.Shift, Key.F5);
		public static SimpleCommand RenderToBitmapCommand = new SimpleCommand("Render to Bitmap");

		static void RenameCommands()
		{
			ApplicationCommands.Open.Text = "Open...";
			ApplicationCommands.SaveAs.Text = "Save As...";
		}

		void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.New();
		}

		void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.Open();
		}

		void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.CloseCurrentDocument();
		}

		void CloseCommand_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.CloseCurrentDocument();
		}

		void CloseAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.CloseAll();
		}

		void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.SaveCurrentDocument();
		}

		void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.SaveCurrentDocumentAs();
		}

		void SaveAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.SaveAll();
		}
		
		void RunCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			var xmlWriter = XmlWriter.Create(new StringWriter(sb));
			Shell.Instance.CurrentDocument.DesignSurface.SaveDesigner(xmlWriter);

			var txt = sb.ToString();
			var xmlReader = XmlReader.Create(new StringReader(txt));

			var ctl = XamlReader.Load(xmlReader);

			Window wnd = ctl as Window;
			if (wnd == null) {
				wnd = new Window();
				wnd.Content = ctl;
			}
			wnd.Show();
		}
		
		void RenderToBitmapCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			int desiredWidth = 300;
			int desiredHeight = 300;
			
			StringBuilder sb = new StringBuilder();
			var xmlWriter = XmlWriter.Create(new StringWriter(sb));
			Shell.Instance.CurrentDocument.DesignSurface.SaveDesigner(xmlWriter);

			var txt = sb.ToString();
			var xmlReader = XmlReader.Create(new StringReader(txt));

			var ctl = XamlReader.Load(xmlReader) as Control;
			if (ctl is Window) {
				var wnd = ctl as Window;
				wnd.Width = desiredWidth;
				wnd.Height = desiredHeight;
				wnd.Top = -10000;
				wnd.Left = -10000;
				wnd.Show();
			} else {
				ctl.Measure(new Size(desiredWidth, desiredHeight));
				ctl.Arrange(new Rect(new Size(desiredWidth, desiredHeight)));
			}
			
			RenderTargetBitmap bmp = new RenderTargetBitmap(300, 300, 96, 96, PixelFormats.Default);
			bmp.Render(ctl);

			var encoder = new PngBitmapEncoder();

			encoder.Frames.Add(BitmapFrame.Create(bmp));

			var dlg = new SaveFileDialog();
			dlg.Filter = "*.png|*.png";
			if (dlg.ShowDialog() == true) {
				using (Stream stm = File.OpenWrite(dlg.FileName)) {
					encoder.Save(stm);
					stm.Flush();
				}
			}
			
			if (ctl is Window) {
				var wnd = ctl as Window;
				wnd.Close();
			}
		}

		void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Shell.Instance.Exit();
		}

		void CurrentDocument_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Shell.Instance.CurrentDocument != null;
		}

		void RouteDesignSurfaceCommands()
		{
			RouteDesignSurfaceCommand(ApplicationCommands.Undo);
			RouteDesignSurfaceCommand(ApplicationCommands.Redo);
			RouteDesignSurfaceCommand(ApplicationCommands.Copy);
			RouteDesignSurfaceCommand(ApplicationCommands.Cut);
			RouteDesignSurfaceCommand(ApplicationCommands.Paste);
			RouteDesignSurfaceCommand(ApplicationCommands.SelectAll);
			RouteDesignSurfaceCommand(ApplicationCommands.Delete);
		}

		void RouteDesignSurfaceCommand(RoutedCommand command)
		{
			var cb = new CommandBinding(command);
			cb.CanExecute += delegate(object sender, CanExecuteRoutedEventArgs e) {
				if (Shell.Instance.CurrentDocument != null) {
					Shell.Instance.CurrentDocument.DesignSurface.RaiseEvent(e);
				}else {
					e.CanExecute = false;
				}
			};
			cb.Executed += delegate(object sender, ExecutedRoutedEventArgs e) {
				Shell.Instance.CurrentDocument.DesignSurface.RaiseEvent(e);
			};
			CommandBindings.Add(cb);
		}
	}
}
