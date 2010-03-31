using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpSnippetCompiler.Core
{
	public class Workbench : IWorkbench
	{
		readonly static string viewContentPath = "/SharpDevelop/Workbench/Pads";

		Form mainForm;
		List<PadDescriptor> padDescriptors = new List<PadDescriptor>();
		List<IViewContent> views = new List<IViewContent>();
		IWorkbenchLayout workbenchLayout;
		IViewContent activeViewContent;
		object activeContent;
		
		public event EventHandler ActiveWorkbenchWindowChanged;
		public event EventHandler ActiveViewContentChanged;		
		public event EventHandler ActiveContentChanged;		
		public event ViewContentEventHandler ViewOpened;		
		public event ViewContentEventHandler ViewClosed;
		public event KeyEventHandler ProcessCommandKey;

		public Workbench(Form mainForm)
		{
			this.mainForm = mainForm;
		}
		
		public Form MainForm {
			get { return mainForm; }
		}
		
		public string Title {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public ICollection<IViewContent> ViewContentCollection {
			get { return views; }
		}
		
		public ICollection<IViewContent> PrimaryViewContents {
			get { return views.AsReadOnly(); }
		}
		
		public IList<IWorkbenchWindow> WorkbenchWindowCollection {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<PadDescriptor> PadContentCollection {
			get { return padDescriptors; }
		}
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IViewContent ActiveViewContent {
			get { return activeViewContent; }
			set { activeViewContent = value; }
		}
		
		public object ActiveContent {
			get { return activeContent; }
			set { activeContent = value; }
		}
		
		public IWorkbenchLayout WorkbenchLayout {
			get { return workbenchLayout; }
			set { workbenchLayout = value; }
		}
		
		public bool IsActiveWindow {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void ShowView(IViewContent content)
		{
			views.Add(content);
			OnViewOpened(new ViewContentEventArgs(content));
		}
		
		public void ShowPad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public void ShowView(IViewContent content, bool switchToOpenedView)
		{
			throw new NotImplementedException();
		}		
		
		public void UnloadPad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public PadDescriptor GetPad(Type type)
		{
			foreach (PadDescriptor pad in padDescriptors) {
				if (pad.Class == type.FullName) {
					return pad;
				}
			}
			return null;			
		}
		
		public void CloseContent(IViewContent content)
		{
			if (views.Contains(content)) {
				views.Remove(content);
			}
			
			content.Dispose();
		}
		
		public void CloseAllViews()
		{
			throw new NotImplementedException();
		}
		
		public void RedrawAllComponents()
		{
			throw new NotImplementedException();
		}
		
		public Properties CreateMemento()
		{
			throw new NotImplementedException();
		}
		
		public void SetMemento(Properties memento)
		{
			Console.WriteLine("Workbench.SetMemento not implemented");
		}
		
		public void UpdateRenderer()
		{
			Console.WriteLine("Workbench.UpdateRenderer not implemented");
		}
		
		public void Initialize()
		{
			try {
				ArrayList contents = AddInTree.GetTreeNode(viewContentPath).BuildChildItems(this);
				foreach (PadDescriptor content in contents) {
					if (content != null) {
						padDescriptors.Add(content);
					}
				}
			} catch (TreePathNotFoundException) {}			
		}
		
		protected virtual void OnActiveWorkbenchWindowChanged(EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}

		protected virtual void OnActiveViewContentChanged(EventArgs e)
		{
			if (ActiveViewContentChanged != null) {
				ActiveViewContentChanged(this, e);
			}
		}

		protected virtual void OnActiveContentChanged(EventArgs e)
		{
			if (ActiveContentChanged != null) {
				ActiveContentChanged(this, e);
			}
		}

		protected virtual void OnViewOpened(ViewContentEventArgs e)
		{
			if (ViewOpened != null) {
				ViewOpened(this, e);
			}
		}

		protected virtual void OnViewClosed(ViewContentEventArgs e)
		{
			if (ViewClosed != null) {
				ViewClosed(this, e);
			}
		}

		protected virtual void OnProcessCommandKey(KeyEventArgs e)
		{
			if (ProcessCommandKey != null) {
				ProcessCommandKey(this, e);
			}
		}		
	}
}
