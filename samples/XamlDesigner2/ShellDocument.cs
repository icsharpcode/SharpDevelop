using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Diagnostics;
using SharpDevelop.XamlDesigner;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.Samples.XamlDesigner
{
	public class ShellDocument : ViewModel
	{
		public ShellDocument(string tempName, string text)
			: this()
		{
			this.tempName = tempName;
			Context.Load(text);
		}

		public ShellDocument(string filePath)
			: this()
		{
			this.filePath = filePath;
			Context.Load(File.ReadAllText(filePath));
		}

		ShellDocument()
		{
			view = new ShellDocumentView(this);
			context = Shell.Instance.Project.CreateContext(TextHolder);
			context.ToolPanel.ShowModeSelector = true;
			context.PropertyChanged += new PropertyChangedEventHandler(context_PropertyChanged);
		}

		void context_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsDirty") {
				RaisePropertyChanged("IsDirty");
			}
		}

		string tempName;
		DesignContext context;
		ShellDocumentView view;

		public ShellDocumentView View
		{
			get { return view; }
		}

		public ITextHolder TextHolder
		{
			get { return view.uxTextEditor; }
		}

		public bool IsDirty
		{
			get { return context.IsDirty; }
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
			get { return FileName ?? tempName; }
		}

		public string Title
		{
			get { return IsDirty ? Name + "*" : Name; }
		}

		public DesignContext Context
		{
			get { return context; }
		}		

		public void Save()
		{
			File.WriteAllText(FilePath, TextHolder.Text);
			Context.SavePoint();
		}

		public void SaveAs(string filePath)
		{
			FilePath = filePath;
			Save();
		}
	}
}
