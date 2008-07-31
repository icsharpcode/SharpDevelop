using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.Xaml;
using System.Xml;
using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Designer.Services;
using System.Diagnostics;

namespace ICSharpCode.XamlDesigner
{
	public class Document : INotifyPropertyChanged
	{
		public Document(string tempName, string text)
		{
			this.tempName = tempName;
			Text = text;
			IsDirty = false;
		}

		public Document(string filePath)
		{
			this.filePath = filePath;
			ReloadFile();
		}
				
		string tempName;
		DesignSurface designSurface = new DesignSurface();

		string text;

		public string Text {
			get {
				return text;
			}
			set {
				if (text != value) {
					text = value;
					IsDirty = true;
					RaisePropertyChanged("Text");
				}
			}
		}

		DocumentMode mode;

		void SetMode(DocumentMode newMode)
		{
			mode = newMode;
			if (IsDesign) {
				UpdateDesign();
			}
			else {
				UpdateXaml();
			}
			RaisePropertyChanged("IsXaml");
			RaisePropertyChanged("IsDesign");
			RaisePropertyChanged("SelectionService");
		}

		public bool IsXaml {
			get { return mode == DocumentMode.Xaml; }
			set { if (value) SetMode(DocumentMode.Xaml); }
		}

		public bool IsDesign {
			get { return mode == DocumentMode.Design; }
			set { if (value) SetMode(DocumentMode.Design); }
		}

		string filePath;

		public string FilePath {
			get {
				return filePath;
			}
			private set {
				filePath = value;
				RaisePropertyChanged("FilePath");
				RaisePropertyChanged("FileName");
				RaisePropertyChanged("Title");
				RaisePropertyChanged("Name");
			}
		}

		bool isDirty;

		public bool IsDirty {
			get {
				return isDirty;
			}
			private set {
				isDirty = value;
				RaisePropertyChanged("IsDirty");
				RaisePropertyChanged("Name");
				RaisePropertyChanged("Title");
			}
		}

		public string FileName {
			get {
				if (FilePath == null) return null;
				return Path.GetFileName(FilePath);
			}
		}

		public string Name {
			get {
				return FileName ?? tempName;
			}
		}

		public string Title {
			get {
				return IsDirty ? Name + "*" : Name;
			}
		}

		public DesignSurface DesignSurface {
			get { return designSurface; }
		}

		public DesignContext DesignContext {
			get { return designSurface.DesignContext; }
		}

		public UndoService UndoService {
			get { return DesignContext.Services.GetService<UndoService>(); }
		}

		public ISelectionService SelectionService {
			get {
				if (IsDesign && DesignContext != null) {
					return DesignContext.Services.Selection;
				}
				return null;
			}
		}

		void ReloadFile()
		{
			Text = File.ReadAllText(FilePath);
			UpdateDesign();
			IsDirty = false;
		}

		public void Save()
		{
			if (IsDesign) {
				UpdateXaml();
			}
			File.WriteAllText(FilePath, Text);
			IsDirty = false;
		}

		public void SaveAs(string filePath)
		{
			FilePath = filePath;
			Save();
		}

		void UpdateXaml()
		{
			var sb = new StringBuilder();
			using (var xmlWriter = XmlWriter.Create(sb)) {
				try {
					DesignSurface.SaveDesigner(xmlWriter);					
					Text = XamlFormatter.Format(sb.ToString());
				}
				catch (Exception x) {
					Shell.ReportException(x);
				}
			}
		}

		void UpdateDesign()
		{
			try {
				XamlLoadSettings settings = new XamlLoadSettings();
				using (var xmlReader = XmlReader.Create(new StringReader(Text))) {
					DesignSurface.LoadDesigner(xmlReader, settings);
				}
				UndoService.UndoStackChanged += delegate { IsDirty = true; };
			}
			catch (Exception x) {
				Shell.ReportException(x);
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion

		enum DocumentMode
		{
			Xaml, Design
		}
	}
}
