using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SharpDevelop.Samples.XamlDesigner.Properties;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Diagnostics;
using SharpDevelop.XamlDesigner;
using System.Data;
using SharpDevelop.XamlDesigner.Commanding;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.Samples.XamlDesigner
{
	public class Shell : ViewModel
	{
		public Shell()
		{
			AddCommand("New", New);
			AddCommand("Open", Open);
			AddCommand("Close", CloseCurrentDocument, HasCurrentDocument);
			AddCommand("Save", SaveCurrentDocument, HasCurrentDocument);
			AddCommand("SaveAs", SaveCurrentDocumentAs, HasCurrentDocument);
			AddCommand("SaveAll", () => { SaveAll(); }, HasCurrentDocument);
			AddCommand("CloseAll", () => { CloseAll(); }, HasCurrentDocument);
			AddCommand("Exit", Exit);

			Documents = new ObservableCollection<ShellDocument>();
			RecentFiles = new ObservableCollection<string>();

			LoadSettings();
		}

		public static Shell Instance = new Shell();
		public const string ApplicationTitle = "Xaml Designer";

		public ObservableCollection<ShellDocument> Documents { get; private set; }
		public ObservableCollection<string> RecentFiles { get; private set; }

		DesignProject project = new DesignProject();

		public DesignProject Project
		{
			get { return project; }
		}

		ShellDocument currentDocument;

		public ShellDocument CurrentDocument
		{
			get
			{
				return currentDocument;
			}
			set
			{
				currentDocument = value;
				RaisePropertyChanged("CurrentDocument");
				RaisePropertyChanged("Title");
			}
		}

		public string Title
		{
			get
			{
				if (CurrentDocument != null) {
					return CurrentDocument.Title + " - " + ApplicationTitle;
				}
				return ApplicationTitle;
			}
		}

		void LoadSettings()
		{
			if (Settings.Default.RecentFiles != null) {
				RecentFiles.AddRange(Settings.Default.RecentFiles.Cast<string>());
			}
		}

		public void SaveSettings()
		{
			if (Settings.Default.RecentFiles == null) {
				Settings.Default.RecentFiles = new StringCollection();
			}
			else {
				Settings.Default.RecentFiles.Clear();
			}
			foreach (var f in RecentFiles) {
				Settings.Default.RecentFiles.Add(f);
			}
		}

		bool HasCurrentDocument()
		{
			return CurrentDocument != null;
		}

		//public static void ReportException(Exception x)
		//{
		//    MessageBox.Show(x.ToString());
		//}

		//public void JumpToError(XamlDocumentError error)
		//{
		//    if (CurrentDocument != null) {
		//        (Views[CurrentDocument] as DocumentView).JumpToError(error);
		//    }
		//}

		#region Files

		bool IsSomethingDirty
		{
			get
			{
				foreach (var doc in Shell.Instance.Documents) {
					if (doc.IsDirty) return true;
				}
				return false;
			}
		}

		static int nonameIndex = 1;

		public void New()
		{
			ShellDocument doc = new ShellDocument("New" + nonameIndex++, File.ReadAllText("NewFileTemplate.xaml"));
			Documents.Add(doc);
			CurrentDocument = doc;
		}

		public void Open()
		{
			var path = MainWindow.Instance.AskOpenFileName();
			if (path != null) {
				Open(path);
			}
		}

		public void Open(string path)
		{
			path = Path.GetFullPath(path);

			if (RecentFiles.Contains(path)) {
				RecentFiles.Remove(path);
			}
			RecentFiles.Insert(0, path);

			foreach (var doc in Documents) {
				if (doc.FilePath == path) {
					CurrentDocument = doc;
					return;
				}
			}

			var newDoc = new ShellDocument(path);
			Documents.Add(newDoc);
			CurrentDocument = newDoc;
		}

		public bool Save(ShellDocument doc)
		{
			if (doc.IsDirty) {
				if (doc.FilePath == null) {
					return SaveAs(doc);
				}
				doc.Save();
			}
			return true;
		}

		public bool SaveAs(ShellDocument doc)
		{
			var initName = doc.FileName ?? doc.Name + ".xaml";
			var path = MainWindow.Instance.AskSaveFileName(initName);
			if (path != null) {
				doc.SaveAs(path);
				return true;
			}
			return false;
		}

		public bool SaveAll()
		{
			foreach (var doc in Documents) {
				if (!Save(doc)) return false;
			}
			return true;
		}

		public bool Close(ShellDocument doc)
		{
			if (doc.IsDirty) {
				var result = MessageBox.Show("Save \"" + doc.Name + "\" ?", Shell.ApplicationTitle,
					MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

				if (result == MessageBoxResult.Yes) {
					if (!Save(doc)) return false;
				}
				else if (result == MessageBoxResult.Cancel) {
					return false;
				}
			}
			Documents.Remove(doc);
			return true;
		}

		public bool CloseAll()
		{
			foreach (var doc in Documents.ToArray()) {
				if (!Close(doc)) return false;
			}
			return true;
		}

		public bool PrepareExit()
		{
			if (IsSomethingDirty) {
				var result = MessageBox.Show("Save All?", Shell.ApplicationTitle,
					MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

				if (result == MessageBoxResult.Yes) {
					if (!SaveAll()) return false;
				}
				else if (result == MessageBoxResult.Cancel) {
					return false;
				}
			}
			return true;
		}

		public void Exit()
		{
			MainWindow.Instance.Close();
		}

		public void SaveCurrentDocument()
		{
			Save(CurrentDocument);
		}

		public void SaveCurrentDocumentAs()
		{
			SaveAs(CurrentDocument);
		}

		public void CloseCurrentDocument()
		{
			Close(CurrentDocument);
		}

		#endregion
	}
}
