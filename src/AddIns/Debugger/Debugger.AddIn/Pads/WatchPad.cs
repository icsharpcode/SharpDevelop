// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2039 $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Debugger;
using Debugger.AddIn.TreeModel;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using Exception = System.Exception;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class WatchPad : DebuggerPad
	{
		TreeViewAdv watchList;
		Process debuggedProcess;
		List<TextNode> watches;
		static WatchPad instance;
		
		/// <remarks>Always check if Instance is null, might be null if pad is not opened!</remarks>
		public static WatchPad Instance {
			get { return instance; }
		}
		
		public WatchPad()
		{
			instance = this;
		}
		
		public List<TextNode> Watches {
			get { return watches; }
		}
		
		readonly TreeColumn nameColumn = new TreeColumn();
		readonly TreeColumn valColumn  = new TreeColumn();
		readonly TreeColumn typeColumn = new TreeColumn();
		
		/// <remarks>
		/// This is not used anywhere, but it is neccessary to be overridden in children of AbstractPadContent.
		/// </remarks>
		public override object Control {
			get {
				return watchList;
			}
		}
		
		public Process Process {
			get { return debuggedProcess; }
		}
		
		protected override void InitializeComponents()
		{
			watchList = new TreeViewAdv();
			watchList.Columns.Add(nameColumn);
			watchList.Columns.Add(valColumn);
			watchList.Columns.Add(typeColumn);
			watchList.UseColumns = true;
			watchList.SelectionMode = TreeSelectionMode.Single;
			watchList.LoadOnDemand = true;
			
			NodeIcon iconControl = new ItemIcon();
			iconControl.ParentColumn = nameColumn;
			watchList.NodeControls.Add(iconControl);
			
			NodeTextBox nameControl = new WatchItemName();
			nameControl.ParentColumn = nameColumn;
			watchList.NodeControls.Add(nameControl);
			
			NodeTextBox textControl = new ItemText();
			textControl.ParentColumn = valColumn;
			watchList.NodeControls.Add(textControl);
			
			NodeTextBox typeControl = new ItemType();
			typeControl.ParentColumn = typeColumn;
			watchList.NodeControls.Add(typeControl);
			
			watchList.AutoRowHeight = true;
			watchList.MouseDoubleClick += new MouseEventHandler(watchList_DoubleClick);
			watchList.ContextMenuStrip = MenuService.CreateContextMenu(this, "/SharpDevelop/Pads/WatchPad/ContextMenu");
			
			watchList.AllowDrop = true;
			watchList.DragEnter += new DragEventHandler(watchList_DragEnter);
			watchList.DragDrop += new DragEventHandler(watchList_DragDrop);
			
			watches = new List<TextNode>();
			
			ResourceService.LanguageChanged += delegate { OnLanguageChanged(); };
			OnLanguageChanged();
		}
		
		void watchList_DragDrop(object sender, DragEventArgs e)
		{
			watchList.BeginUpdate();
			TextNode text = new TextNode(e.Data.GetData(DataFormats.StringFormat).ToString(), SupportedLanguage.CSharp);
			TreeViewVarNode node = new TreeViewVarNode(this.debuggedProcess, this.watchList, text);
			watches.Add(text);
			watchList.Root.Children.Add(node);
			watchList.EndUpdate();
			
			node.IsSelected = true;
			
			this.RefreshPad();
		}

		void watchList_DragEnter(object sender, DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.StringFormat)) {
				e.Effect = DragDropEffects.Copy;
			}
			else {
				e.Effect = DragDropEffects.None;
			}
		}
		
		void watchList_DoubleClick(object sender, MouseEventArgs e)
		{
			if (watchList.SelectedNode == null)
			{
				watchList.BeginUpdate();
				TextNode text = new TextNode("", SupportedLanguage.CSharp);
				TreeViewVarNode node = new TreeViewVarNode(this.debuggedProcess, this.watchList, text);
				watches.Add(text);
				watchList.Root.Children.Add(node);
				watchList.EndUpdate();
				
				node.IsSelected = true;
				
				this.RefreshPad();
				
				foreach (NodeControlInfo nfo in watchList.GetNodeControls(node)) {
					if (nfo.Control is WatchItemName)
						((EditableControl)nfo.Control).MouseUp(new TreeNodeAdvMouseEventArgs(e));
				}
			}
		}
		
		void ResetPad(object sender, EventArgs e)
		{
			watchList.BeginUpdate();
			watchList.Root.Children.Clear();
			
			foreach (TextNode text in watches)
				watchList.Root.Children.Add(new TreeViewVarNode(this.debuggedProcess, this.watchList, text));
			
			watchList.EndUpdate();
		}
		
		void OnLanguageChanged()
		{
			nameColumn.Header = ResourceService.GetString("Global.Name");
			nameColumn.Width = 250;
			valColumn.Header  = ResourceService.GetString("Dialog.HighlightingEditor.Properties.Value");
			valColumn.Width = 300;
			typeColumn.Header = ResourceService.GetString("ResourceEditor.ResourceEdit.TypeColumn");
			typeColumn.Width = 250;
		}
		
		protected override void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused -= debuggedProcess_Paused;
				debuggedProcess.Exited -= ResetPad;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused += debuggedProcess_Paused;
				debuggedProcess.Exited += ResetPad;
			}
			RefreshPad();
		}
		
		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			RefreshPad();
		}
		
		public override void RefreshPad()
		{
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedStackFrame == null)
				return;
			
			using(new PrintTimes("Watch Pad refresh")) {
				try {
					watchList.BeginUpdate();
					Utils.DoEvents(debuggedProcess);
					List<TreeViewVarNode> nodes = new List<TreeViewVarNode>();
					
					foreach (var nod in watches) {
						try {
							LoggingService.Info("Evaluating: " + (string.IsNullOrEmpty(nod.Name) ? "is null or empty!" : nod.Name));
							var nodExpression = debugger.GetExpression(nod.Name);
							//Value val = ExpressionEvaluator.Evaluate(nod.Name, nod.Language, debuggedProcess.SelectedStackFrame);
							ExpressionNode valNode = new ExpressionNode(null, nod.Name, nodExpression);
							nodes.Add(new TreeViewVarNode(debuggedProcess, watchList, valNode));
						} catch (GetValueException) {
							string error = String.Format(StringParser.Parse("${res:MainWindow.Windows.Debug.Watch.InvalidExpression}"), nod.Name);
							ErrorInfoNode infoNode = new ErrorInfoNode(nod.Name, error);
							nodes.Add(new TreeViewVarNode(debuggedProcess, watchList, infoNode));
						}
					}
					
					watchList.Root.Children.Clear();
					
					foreach (TreeViewVarNode nod in nodes)
						watchList.Root.Children.Add(nod);
				} catch(AbortedBecauseDebuggeeResumedException) {
				} catch(Exception ex) {
					if (debuggedProcess == null || debuggedProcess.HasExited) {
						// Process unexpectedly exited
					} else {
						MessageService.ShowException(ex);
					}
				}
			}
			
			watchList.EndUpdate();
		}
	}
}
