using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xaml;
using System.IO;
using System.Xaml.Schema;
using System.Windows;
using SharpDevelop.XamlDesigner.Controls;
using System.Windows.Controls;
using SharpDevelop.XamlDesigner.Extensibility;
using SharpDevelop.XamlDesigner.Extensibility.Attributes;
using SharpDevelop.XamlDesigner.Palette;
using SharpDevelop.XamlDesigner.Commanding;
using SharpDevelop.XamlDesigner.Dom.UndoSystem;
using System.Windows.Threading;
using System.Windows.Markup;

namespace SharpDevelop.XamlDesigner.Dom
{
	public class DesignContext : ViewModel
	{
		internal DesignContext(DesignProject project, ITextHolder textHolder)
		{
			Project = project;
			TextHolder = textHolder;

			DesignView = new DesignView(this);
			ToolPanel = new ToolPanel(this);
			Selection = new SelectionCollection();
			UndoManager = new UndoManager();
			DesignCommands = new DesignCommands(this);
			AdornerManager = new AdornerManager(this);

			CreateTimer();
		}

		public DesignView DesignView { get; private set; }
		public ToolPanel ToolPanel { get; private set; }
		public AdornerManager AdornerManager { get; private set; }
		public SelectionCollection Selection { get; private set; }
		public UndoManager UndoManager { get; private set; }
		public DesignCommands DesignCommands { get; private set; }
		public DesignProject Project { get; private set; }

		public bool NeedParse;
		//public List<string> References = new List<string>();
		public ITextHolder TextHolder { get; private set; }

		DispatcherTimer timer;

		DesignItem root;

		public DesignItem Root
		{
			get
			{
				return root;
			}
			set
			{
				root = value;
				RaisePropertyChanged("Root");
			}
		}

		DocumentMode mode = DocumentMode.Design;

		public DocumentMode Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
				RaisePropertyChanged("Mode");
			}
		}

		bool isDirty;

		public bool IsDirty
		{
			get
			{
				return isDirty;
			}
			set
			{
				isDirty = value;
				RaisePropertyChanged("IsDirty");
			}
		}

		public void Load(string text)
		{
			TextHolder.Text = text;
			Parse();
		}

		public void Parse()
		{
			timer.Stop();
			Root = XamlOperations.Parse(this);
			//DesignView.Root = Root.View;
		}

		public void SavePoint()
		{
			IsDirty = false;
		}

		void CreateTimer()
		{
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(DesignEnvironment.Instance.ParseDelay);
			timer.Tick += new EventHandler(timer_Tick);
		}

		void timer_Tick(object sender, EventArgs e)
		{
			if (NeedParse) {
				Parse();
			}
		}

		public void ResetTimer()
		{
			timer.Stop();
			timer.Start();
		}

		public DesignItem CreateItem(Type type)
		{
			return CreateItem(Activator.CreateInstance(type));
		}

		public DesignItem CreateItem(object instance)
		{
			return new DesignItem(this, instance);
		}
	}
}
