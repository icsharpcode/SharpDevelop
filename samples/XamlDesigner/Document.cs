using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.XamlBackend;
using ICSharpCode.WpfDesign.Designer.OutlineView;
using System.Xml;
using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Designer.Services;
using System.Diagnostics;

namespace ICSharpCode.XamlDesigner
{
	public class Document : INotifyPropertyChanged
	{
		public Document(string tempName, string text)
			: this()
		{
			this.tempName = tempName;
			Text = text;
			Context.Parse(Text);
			IsDirty = false;
		}

		public Document(string filePath)
			: this()
		{
			this.filePath = filePath;
			ReloadFile();
		}

		Document()
		{
			var doc = Shell.Instance.Project.CreateDocument();
			context = new XamlDesignContext(doc);
			context.UndoService.UndoStackChanged += new EventHandler(UndoService_UndoStackChanged);
		}

		string tempName;
		XamlDesignContext context;

		string text;

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				if (text != value) {
					text = value;
					IsDirty = true;
					RaisePropertyChanged("Text");
				}
			}
		}

		DocumentMode mode;

		public DocumentMode Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
				//if (InDesignMode) {
				//    UpdateDesign();
				//}
				//else {
				//    UpdateXaml();
				//}
				RaisePropertyChanged("Mode");
				RaisePropertyChanged("InXamlMode");
				RaisePropertyChanged("InDesignMode");
			}
		}

		public bool InXamlMode
		{
			get { return Mode == DocumentMode.Xaml; }
		}

		public bool InDesignMode
		{
			get { return Mode == DocumentMode.Design; }
		}

		string filePath;

		public string FilePath
		{
			get
			{
				return filePath;
			}
			private set
			{
				filePath = value;
				RaisePropertyChanged("FilePath");
				RaisePropertyChanged("FileName");
				RaisePropertyChanged("Title");
				RaisePropertyChanged("Name");
			}
		}

		bool isDirty;

		public bool IsDirty
		{
			get
			{
				return isDirty;
			}
			private set
			{
				isDirty = value;
				RaisePropertyChanged("IsDirty");
				RaisePropertyChanged("Name");
				RaisePropertyChanged("Title");
			}
		}

		public string FileName
		{
			get
			{
				if (FilePath == null) return null;
				return Path.GetFileName(FilePath);
			}
		}

		public string Name
		{
			get
			{
				return FileName ?? tempName;
			}
		}

		public string Title
		{
			get
			{
				return IsDirty ? Name + "*" : Name;
			}
		}

		public DesignContext Context
		{
			get { return context; }
		}

		//TODO
		//public XamlErrorService XamlErrorService {
		//    get { 
		//        if (DesignContext != null) {
		//            return DesignContext.GetService<XamlErrorService>();
		//        }
		//        return null;
		//    }
		//}

		void ReloadFile()
		{
			Text = File.ReadAllText(FilePath);
			//UpdateDesign();
			Context.Parse(Text);
			IsDirty = false;
		}

		public void Save()
		{
			//if (InDesignMode) {
			//    UpdateXaml();
			//}
			File.WriteAllText(FilePath, Text);
			IsDirty = false;
		}

		public void SaveAs(string filePath)
		{
			FilePath = filePath;
			Save();
		}

		public void Refresh()
		{
			//UpdateXaml();
			//UpdateDesign();
		}

		//void UpdateXaml()
		//{
		//    if (Context.CanSave) {
		//        Text = Context.Save();
		//    }
		//}

		//void UpdateDesign()
		//{
		//    Context.Parse(Text);
		//}

		void UndoService_UndoStackChanged(object sender, EventArgs e)
		{
			IsDirty = Context.UndoService.CanUndo;
			if (Context.ParseSuggested) {
				Context.Parse(Text);
			}
			//if (Context.Is
			//IsDirty = true;
			//if (InXamlMode) {
			//    UpdateXaml();
			//}
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
	}

	public enum DocumentMode
	{
		Xaml, Design
	}
}
