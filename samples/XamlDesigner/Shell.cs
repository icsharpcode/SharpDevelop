using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ICSharpCode.WpfDesign.Designer.PropertyGrid;
using ICSharpCode.XamlDesigner.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Diagnostics;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.XamlDesigner
{
	public class Shell : INotifyPropertyChanged
	{
		public Shell()
		{
			Documents = new ObservableCollection<Document>();
			RecentFiles = new ObservableCollection<string>();
			Views = new Dictionary<object, FrameworkElement>();

			LoadSettings();
		}

		public static Shell Instance = new Shell();
		public const string ApplicationTitle = "Xaml Designer";

		//public Toolbox Toolbox { get; set; }
        //public SceneTree SceneTree { get; set; }
        public PropertyGrid PropertyGrid { get; internal set; }
        //public ErrorList ErrorList { get; set; }
		
		public ObservableCollection<Document> Documents { get; private set; }
		public ObservableCollection<string> RecentFiles { get; private set; }		
		public Dictionary<object, FrameworkElement> Views { get; private set; }

		Document currentDocument;

		public Document CurrentDocument {
			get {
				return currentDocument;
			}
			set {
				currentDocument = value;
				RaisePropertyChanged("CurrentDocument");
				RaisePropertyChanged("Title");
			}
		}

		public string Title {
			get {
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

		public static void ReportException(Exception x)
		{
			MessageBox.Show(x.ToString());
		}

		public void JumpToError(XamlError error)
		{
			if (CurrentDocument != null) {
				(Views[CurrentDocument] as DocumentView).JumpToError(error);
			}
		}

		public bool CanRefresh()
		{
			return CurrentDocument != null;
		}

		public void Refresh()
		{
			CurrentDocument.Refresh();
		}

		#region Files

		bool IsSomethingDirty {
			get {
				foreach (var doc in Shell.Instance.Documents) {
					if (doc.IsDirty) return true;
				}
				return false;
			}
		}

		static int nonameIndex = 1;

		public void New()
        {
            Document doc = new Document("New" + nonameIndex++, File.ReadAllText("NewFileTemplate.xaml"));
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

            var newDoc = new Document(path);
            Documents.Add(newDoc);
            CurrentDocument = newDoc;
		}		

		public bool Save(Document doc)
		{
			if (doc.IsDirty) {
				if (doc.FilePath == null) {
					return SaveAs(doc);
				}
				doc.Save();
			}
			return true;
		}

		public bool SaveAs(Document doc)
		{
			var initName = doc.FileName ?? doc.Name + ".xaml";
			var path = MainWindow.Instance.AskSaveFileName(initName);
			if (path != null)  {
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

		public bool Close(Document doc)
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
			Views.Remove(doc);
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

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion
	}
}
