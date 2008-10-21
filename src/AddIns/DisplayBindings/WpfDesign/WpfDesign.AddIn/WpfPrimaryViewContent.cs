using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WpfDesign.Designer.XamlBackend;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.WpfDesign.Designer.Services;
using System.Windows.Threading;
using System.IO;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class WpfPrimaryViewContent : TextEditorDisplayBindingWrapper, IWpfViewContent, IHasPropertyContainer, IUndoHandler, IToolsHost, IParseInformationListener
	{
		public WpfPrimaryViewContent(OpenedFile file)
			: base(file)
		{
		}

		List<Task> tasks = new List<Task>();

		public override void Load(OpenedFile file, Stream stream)
		{
			base.Load(file, stream);
			var doc = XamlMapper.GetXamlDocument(file.FileName);
			Context = new XamlDesignContext(doc);
			Context.AddService(typeof(IPropertyDescriptionService), new PropertyDescriptionService());
			Context.AddService(typeof(IEventHandlerService), new CSharpEventHandlerService(this));
			Context.AddService(typeof(ITopLevelWindowService), new WpfAndWinFormsTopLevelWindowService());
			Context.AddService(typeof(ChooseClassServiceBase), new IdeChooseClassService());
			Context.UndoService.UndoStackChanged += new EventHandler(Undo_UndoStackChanged);
			Context.Parse(Text);
		}

		void Undo_UndoStackChanged(object sender, EventArgs e)
		{
			this.PrimaryFile.MakeDirty();
		}

		//public new void ParseInformationUpdated(ParseInformation parseInfo)
		//{
		//    base.ParseInformationUpdated(parseInfo);

		//    Dispatcher.CurrentDispatcher.Invoke(new Action(delegate {
		//        Context.Load(Text);
		//    }));

		//    UpdateTasks();
		//}

		void UpdateTasks()
		{
			foreach (var task in tasks) {
				TaskService.Remove(task);
			}

			tasks.Clear();

			foreach (var error in Context.Document.Errors) {
				var task = new Task(PrimaryFile.FileName, error.Message, error.LinePosition - 1, error.LineNumber - 1, TaskType.Error);
				tasks.Add(task);
				TaskService.Add(task);
			}
		}

		#region WpfViewContent Implementation

		public XamlDesignContext Context { get; private set; }

		public PropertyContainer PropertyContainer
		{
			get { return WpfTools.PropertyContainer; }
		}

		public System.Windows.Forms.Control ToolsControl
		{
			get { return WpfTools.ToolboxHost; }
		}

		public bool EnableRedo
		{
			get { return Context.UndoService.CanRedo; }
		}

		public bool EnableUndo
		{
			get { return Context.UndoService.CanUndo; }
		}

		public void Redo()
		{
			Context.UndoService.Redo();
		}

		public void Undo()
		{
			Context.UndoService.Undo();
		}

		#endregion
	}
}
