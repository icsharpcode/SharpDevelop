// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class GenerateCodeAction : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textEditorControl = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			
			
			
			ParseInformation parseInformation;
			
			if (window.ViewContent.IsUntitled) {
				parseInformation = ParserService.ParseFile(textEditorControl.FileName, textEditorControl.Document.TextContent);
			} else {
				parseInformation = ParserService.GetParseInformation(textEditorControl.FileName);
			}
			
			if (parseInformation == null) {
				return;
			}
			
			ICompilationUnit cu = parseInformation.MostRecentCompilationUnit as ICompilationUnit;
			if (cu == null) {
				return;
			}
			IClass currentClass = GetCurrentClass(textEditorControl, cu, textEditorControl.FileName);
			
			if (currentClass != null) {
				ArrayList categories = new ArrayList();
				using (FormVersion1 form = new FormVersion1(textEditorControl, new CodeGenerator[] {
					new ConstructorCodeGenerator(currentClass),
					new GetterCodeGenerator(currentClass),
					new SetterCodeGenerator(currentClass),
					new GetterAndSetterCodeGenerator(currentClass),
					new OnXXXMethodsCodeGenerator(currentClass),
					new OverrideMethodsCodeGenerator(currentClass),
					new InterfaceImplementorCodeGenerator(currentClass),
					new AbstractClassImplementorCodeGenerator(currentClass),
					new ToStringCodeGenerator(currentClass),
					new EqualsCodeGenerator(currentClass),
				})) {
					form.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				}
			}
		}
		
		/// <summary>
		/// Returns the class in which the carret currently is, returns null
		/// if the carret is outside the class boundaries.
		/// </summary>
		IClass GetCurrentClass(TextEditorControl textEditorControl, ICompilationUnit cu, string fileName)
		{
			IDocument document = textEditorControl.Document;
			if (cu != null) {
				int caretLineNumber = document.GetLineNumberForOffset(textEditorControl.ActiveTextAreaControl.Caret.Offset) + 1;
				int caretColumn     = textEditorControl.ActiveTextAreaControl.Caret.Offset - document.GetLineSegment(caretLineNumber - 1).Offset + 1;
				return FindClass(cu.Classes, caretLineNumber, caretColumn);
			}
			return null;
		}
		IClass FindClass(ICollection classes, int lineNr, int column)
		{
			foreach (IClass c in classes) {
				if (c.Region.IsInside(lineNr, column)) {
					IClass inner = FindClass(c.InnerClasses, lineNr, column);
					return inner == null ? c : inner;
				}
			}
			return null;
		}
	}
	
	public class SurroundCodeAction : AbstractEditAction
	{
		
		public override void Execute(TextArea editActionHandler)
		{
//			SelectionWindow selectionWindow = new SelectionWindow("Surround");
//			selectionWindow.Show();
		}
	}
	
	/// <summary>
	///     Add summary description for form
	/// </summary>
	public class FormVersion1 : Form
	{
		private System.Windows.Forms.ColumnHeader   createdObject0;
		private System.Windows.Forms.ListView       categoryListView;
		private System.Windows.Forms.Label          statusLabel;
		private System.Windows.Forms.CheckedListBox selectionListBox;
		
		TextEditorControl textEditorControl;
		
		CodeGenerator SelectedCodeGenerator {
			get {
				if (categoryListView.SelectedItems.Count <= 0) {
					return null;
				}
				return (CodeGenerator)categoryListView.SelectedItems[0].Tag;
			}
		}
		
		public FormVersion1(TextEditorControl textEditorControl, CodeGenerator[] codeGenerators)
		{
			this.textEditorControl = textEditorControl;
			
			//  Must be called for initialization
			this.InitializeComponents();
			selectionListBox.Sorted = true;
			Point caretPos  = textEditorControl.ActiveTextAreaControl.Caret.Position;
			TextArea textArea = textEditorControl.ActiveTextAreaControl.TextArea;
			TextView textView = textArea.TextView;
			Point visualPos;
			int physicalline = textView.Document.GetVisibleLine(caretPos.Y);
			visualPos = new Point(textView.GetDrawingXPos(caretPos.Y, caretPos.X) +
			                      textView.DrawingPosition.X,
			          (int)((1 + physicalline) * textView.FontHeight) - 
			          textArea.VirtualTop.Y - 1 + textView.DrawingPosition.Y);
			Location = textEditorControl.ActiveTextAreaControl.TextArea.PointToScreen(visualPos);
			StartPosition   = FormStartPosition.Manual;
			
			
			categoryListView.SmallImageList = categoryListView.LargeImageList = ClassBrowserIconService.ImageList;
			
			foreach (CodeGenerator codeGenerator in codeGenerators) {
				if (codeGenerator.IsActive) {
					ListViewItem newItem = new ListViewItem(codeGenerator.CategoryName);
					newItem.ImageIndex = codeGenerator.ImageIndex;
					newItem.Tag        = codeGenerator;
					categoryListView.Items.Add(newItem);
				}
			}
			
			categoryListView.SelectedIndexChanged += new EventHandler(CategoryListViewItemChanged);
		}
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			if (categoryListView.Items.Count > 0) {
				categoryListView.Select();
				categoryListView.Focus();
				categoryListView.Items[0].Focused = categoryListView.Items[0].Selected = true;
			} else {
				Close();
			}
		}
		
		protected override bool ProcessDialogKey(Keys keyData)
		{
			switch (keyData) {
				case Keys.Escape:
					Close();
					return true;
				case Keys.Back:
					categoryListView.Focus();
					return true;
				case Keys.Return:
					if (SelectedCodeGenerator != null) {
						if (categoryListView.Focused && SelectedCodeGenerator.Content.Count > 0) {
							selectionListBox.Focus();
						} else {
							Close();
							SelectedCodeGenerator.GenerateCode(textEditorControl.ActiveTextAreaControl.TextArea, selectionListBox.CheckedItems.Count > 0 ? (IList)selectionListBox.CheckedItems : (IList)selectionListBox.SelectedItems);
						}
						return true;
					}  else {
						return false;
					}
			}
			return base.ProcessDialogKey(keyData);
		}
		
		void CategoryListViewItemChanged(object sender, EventArgs e)
		{
			CodeGenerator codeGenerator = SelectedCodeGenerator;
			if (codeGenerator == null) {
				return;
			}
			
			statusLabel.Text = codeGenerator.Hint;
			selectionListBox.BeginUpdate();
			selectionListBox.Items.Clear();
			if (codeGenerator.Content.Count > 0) {
				Hashtable objs = new Hashtable();
				selectionListBox.Sorted = codeGenerator.Content.Count > 1;
				foreach (object o in codeGenerator.Content) {
					if (!objs.Contains(o.ToString())) {
						selectionListBox.Items.Add(o);
						objs.Add(o.ToString(), "");
					} 
				}
				selectionListBox.SelectedIndex = 0;
			} 
			selectionListBox.EndUpdate();
			selectionListBox.Refresh();
		}
		
		/// <summary>
		///   This method was autogenerated - do not change the contents manually
		/// </summary>
		private void InitializeComponents()
		{
			// 
			//  Set up generated class form
			// 
			this.SuspendLayout();
			this.Name = "form";
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Size = new System.Drawing.Size(264, 312);
			this.ShowInTaskbar = false;
			
			// 
			//  Set up member selectionListBox
			// 
			selectionListBox = new System.Windows.Forms.CheckedListBox();
			selectionListBox.Name = "selectionListBox";
			selectionListBox.Location = new System.Drawing.Point(0, 128);
			selectionListBox.Size = new System.Drawing.Size(264, 184);
			selectionListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			selectionListBox.TabIndex = 2;
			this.Controls.Add(selectionListBox);
			
			// 
			//  Set up member statusLabel
			// 
			statusLabel = new System.Windows.Forms.Label();
			statusLabel.Name = "statusLabel";
			statusLabel.Text = "Choose fields to generate getters and setters";
			statusLabel.TabIndex = 1;
			statusLabel.Size = new System.Drawing.Size(264, 16);
			statusLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			statusLabel.Location = new System.Drawing.Point(0, 112);
			statusLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.Controls.Add(statusLabel);
			
			// 
			//  Set up member categoryListView
			// 
			categoryListView = new System.Windows.Forms.ListView();
			categoryListView.Name = "categoryListView";
			categoryListView.Dock = System.Windows.Forms.DockStyle.Top;
			categoryListView.TabIndex = 0;
			categoryListView.View = System.Windows.Forms.View.Details;
			categoryListView.Size = new System.Drawing.Size(264, 112);
			categoryListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			categoryListView.MultiSelect = false;
			
			// 
			//  Set up member createdObject0
			// 
			createdObject0 = new System.Windows.Forms.ColumnHeader();
			createdObject0.Width = 258;
			categoryListView.Columns.Add(createdObject0);
			this.Controls.Add(categoryListView);
			this.ResumeLayout(false);
		}
	}
}
