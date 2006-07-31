// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.FormsDesigner.Gui;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.TextEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	public class WixDialogDesigner : FormsDesignerViewContent, IWixDialogDesigner
	{
		string dialogId = String.Empty;
		string originalTitleName = String.Empty;
		WixProject wixProject;
		SetupDialogControlsSideTab setupDialogControlsSideTab;
		
		/// <summary>
		/// Ignore the dialog id in the text editor if the OpenDialog method is called
		/// and the designer is not being opened by the user clicking the Design tab.
		/// </summary>
		bool ignoreDialogIdSelectedInTextEditor;
		
		public WixDialogDesigner(IViewContent view) 
			: this(view, new WixDialogDesignerLoaderProvider(), new WixDialogDesignerGenerator())
		{
		}
		
		public WixDialogDesigner(IViewContent view, WixDialogDesignerLoaderProvider designerLoaderProvider, WixDialogDesignerGenerator designerGenerator)
			: base(view, designerLoaderProvider, designerGenerator)
		{
			designerLoaderProvider.Designer = this;
		}
		
		/// <summary>
		/// Gets the WixDialogDesigner from the primary view.
		/// </summary>
		/// <returns>The wix dialog designer view that is attached as a 
		/// secondary view; <see langword="null"/> if the primary view
		/// has no such designer attached.</returns>
		public static WixDialogDesigner GetDesigner(IViewContent view)
		{
			foreach (ISecondaryViewContent secondaryView in view.SecondaryViewContents) {
				WixDialogDesigner designer = secondaryView as WixDialogDesigner;
				if (designer != null) {
					return designer;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Attempts to open the Wix dialog from the document currently 
		/// associated with this designer.
		/// </summary>
		/// <param name="id">The id of the dialog that will be opened.</param>
		public void OpenDialog(string id)
		{			
			JumpToDialogElement(id);
			if (base.IsFormsDesignerVisible) {
				// Reload so the correct dialog is displayed.
				base.Deselecting();
				DialogId = id;
				base.Selected();
			} else {
				// Need to open the designer.
				originalTitleName = base.viewContent.TitleName;
				DialogId = id;
				OpenDesigner();
			}
		}

		/// <summary>
		/// Set dialog id to null after calling base.Deselecting since base.Deselecting 
		/// calls MergeFormChanges which will reference this dialog id.
		/// </summary>
		public override void Deselecting()
		{
			base.Deselecting();
			DialogId = null;
			RemoveWixToolboxSideTab();
		}
		
		/// <summary>
		/// Designer has been selected.
		/// </summary>
		public override void Selected()
		{			
			try {
				if (!ignoreDialogIdSelectedInTextEditor) {
					originalTitleName = base.viewContent.TitleName;
					string dialogId = GetDialogIdSelectedInTextEditor();
					if (dialogId == null) {
						dialogId = GetFirstDialogIdInTextEditor();
						JumpToDialogElement(dialogId);
					}
					DialogId = dialogId;
				}
				wixProject = GetProject();
			} catch (XmlException ex) {	
				// Let the Wix designer loader try to load the XML and generate 
				// an error message.
				DialogId = "InvalidXML";
				AddToErrorList(ex);
			}
			base.Selected();
			RemoveFormsDesignerToolboxSideTabs();
			AddWixToolboxSideTab();
		}
		
		/// <summary>
		/// Removes the WinForms toolbox side tabs.
		/// </summary>
		public override void SwitchedTo()
		{
			base.SwitchedTo();
			RemoveFormsDesignerToolboxSideTabs();
		}
		
		/// <summary>
		/// Gets the Wix document filename.
		/// </summary>
		public string DocumentFileName {
			get {
				return base.viewContent.FileName;
			}
		}
		
		/// <summary>
		/// Gets the wix project containing the document open in the designer.
		/// </summary>
		public WixProject Project {
			get {
				return wixProject;
			}
		}
		
		/// <summary>
		/// Gets the Wix document xml.
		/// </summary>
		public string GetDocumentXml()
		{
			return GetActiveTextEditorText();
		}
		
		/// <summary>
		/// Gets or sets the dialog id currently being designed.
		/// </summary>
		public string DialogId {
			get {
				return dialogId;
			}
			set {
				dialogId = value;
				UpdateTitleName(dialogId);
			}
		}
			
		/// <summary>
		/// Finds the WixDialogDesigner in the current window's secondary views
		/// and switches to it.
		/// </summary>
		void OpenDesigner()
		{
			int viewNumber = GetViewNumber(base.viewContent.SecondaryViewContents);
			try {
				ignoreDialogIdSelectedInTextEditor = true;
				WorkbenchWindow.SwitchView(viewNumber);
			} finally {
				ignoreDialogIdSelectedInTextEditor = false;
			}
		}
		
		/// <summary>
		/// Gets the view number for the WixDialogDesigner.
		/// </summary>
		static int GetViewNumber(List<ISecondaryViewContent> secondaryViews)
		{
			int viewIndex = 0;
			foreach (ISecondaryViewContent view in secondaryViews) {
				++viewIndex;
				if (view is WixDialogDesigner) {
					break;
				}
			}
			return viewIndex;
		}
				
		/// <summary>
		/// From the current cursor position in the text editor determine the
		/// selected dialog id.
		/// </summary>
		string GetDialogIdSelectedInTextEditor()
		{
			TextAreaControl textArea = ActiveTextAreaControl;
			if (textArea != null) {
				return WixDocument.GetDialogId(new StringReader(textArea.Document.TextContent), textArea.Caret.Line);
			}
			return null;
		}
		
		/// <summary>
		/// Gets the first dialog id in the file.
		/// </summary>
		string GetFirstDialogIdInTextEditor()
		{
			TextAreaControl textArea = ActiveTextAreaControl;
			if (textArea != null) {
				StringReader reader = new StringReader(textArea.Document.TextContent);
				ReadOnlyCollection<string> ids = WixDocument.GetDialogIds(reader);
				if (ids.Count > 0) {
					return ids[0];
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the text from the active text editor.
		/// </summary>
		string GetActiveTextEditorText()
		{
			TextAreaControl textArea = ActiveTextAreaControl;
			if (textArea != null) {
				return textArea.Document.TextContent;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Gets the active text area control.
		/// </summary>
		TextAreaControl ActiveTextAreaControl {
			get {
				ITextEditorControlProvider provider = viewContent as ITextEditorControlProvider;
				if (provider != null) {
					return provider.TextEditorControl.ActiveTextAreaControl;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the view's title name when displaying the specified dialog.
		/// </summary>
		static string GetTitleName(string title, string dialogId)
		{
			return String.Concat(title, " [", dialogId, "]");
		}
		
		/// <summary>
		/// Updates the view's title with the dialog id.
		/// </summary>
		void UpdateTitleName(string dialogId)
		{
			if (dialogId != null) {
				base.viewContent.TitleName = GetTitleName(originalTitleName, dialogId);
			} else {
				base.viewContent.TitleName = originalTitleName;
			}
		}
		
		void AddToErrorList(XmlException ex)
		{
			TaskService.ClearExceptCommentTasks();
			TaskService.Add(new Task(base.viewContent.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error));
			WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
		}
		
		/// <summary>
		/// Cannot use ProjectService.CurrentProject since it is possible to switch 
		/// to the designer without selecting the project.
		/// </summary>
		WixProject GetProject()
		{
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				if (project.IsFileInProject(base.viewContent.FileName)) {
					return project as WixProject;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Jumps to dialog element with corresponding dialog id. This is only used
		/// when the user opens a dialog from the Setup dialogs window or the cursor
		/// was not near a dialog when the designer was switched to. Jumping to the
		/// dialog selected ensures that when the user switches back, if they did
		/// not make any changes, then the dialog xml is displayed.
		/// </summary>
		void JumpToDialogElement(string dialogId)
		{
			try {
				if (dialogId != null) {
					TextAreaControl textArea = ActiveTextAreaControl;
					if (textArea != null) {
						Location location = WixDocument.GetDialogStartElementLocation(new StringReader(textArea.Document.TextContent), dialogId);
						textArea.JumpTo(location.Y);
					}
				}
			} catch (XmlException) { 
				// Ignore
			}
		}
		
		/// <summary>
		/// Removes forms designer toolbox side bars.
		/// </summary>
		/// <remarks>
		/// Cannot remove forms designer toolbox side tabs otherwise
		/// the designer will not unload when you call base.Deselected.
		/// Not sure why the existence of the side tab stops the unload. 
		/// This has been the case since SharpDevelop 1.1.
		/// </remarks>
		void RemoveFormsDesignerToolboxSideTabs()
		{
//			foreach(AxSideTab tab in ToolboxProvider.SideTabs) {
//				if (!SharpDevelopSideBar.SideBar.Tabs.Contains(tab)) {
//					return;
//				}
//				SharpDevelopSideBar.SideBar.Tabs.Remove(tab);
//			}
//			SharpDevelopSideBar.SideBar.Refresh();
		}
		
		/// <summary>
		/// Gets the setup controls side tab.
		/// </summary>
		SetupDialogControlsSideTab SetupDialogControlsSideTab {
			get {
				if (setupDialogControlsSideTab == null) {
					setupDialogControlsSideTab = SetupDialogControlsSideTab.CreateSideTab();
				}
				return setupDialogControlsSideTab;
			}
		}
		
		/// <summary>
		/// Adds the Wix toolbox side tab
		/// </summary>
		void AddWixToolboxSideTab()
		{
			if (!SharpDevelopSideBar.SideBar.Tabs.Contains(SetupDialogControlsSideTab)) {
				SharpDevelopSideBar.SideBar.Tabs.Add(SetupDialogControlsSideTab);
				SharpDevelopSideBar.SideBar.ActiveTab = SetupDialogControlsSideTab;
				SharpDevelopSideBar.SideBar.Refresh();
			}
		}
		
		/// <summary>
		/// Removes the Wix toolbox side tab
		/// </summary>
		void RemoveWixToolboxSideTab()
		{
			if (SharpDevelopSideBar.SideBar.Tabs.Contains(SetupDialogControlsSideTab)) {
				SharpDevelopSideBar.SideBar.Tabs.Remove(SetupDialogControlsSideTab);
				SharpDevelopSideBar.SideBar.Refresh();
			}
		}
	}
}
