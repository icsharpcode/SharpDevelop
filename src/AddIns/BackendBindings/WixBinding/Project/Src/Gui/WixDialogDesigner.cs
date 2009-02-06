// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

using ICSharpCode.FormsDesigner;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor;

namespace ICSharpCode.WixBinding
{
	public class WixDialogDesigner : FormsDesignerViewContent, IWixDialogDesigner
	{
		string dialogId = String.Empty;
		WixProject wixProject;
		
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
			foreach (IViewContent secondaryView in view.SecondaryViewContents) {
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
			if (base.Host != null) {
				// Reload so the correct dialog is displayed.
				this.MergeAndUnloadDesigner();
				DialogId = id;
				this.ReloadDesignerFromMemory();
			} else {
				// Need to open the designer.
				DialogId = id;
				OpenDesigner();
			}
		}
		
		protected override void LoadInternal(OpenedFile file, Stream stream)
		{
			if (file == this.PrimaryFile) {
				// The FormsDesignerViewContent normally operates independently of any
				// text editor. The following statements connect the forms designer
				// directly to the underlying XML text editor so that the caret positioning
				// and text selection operations done by the WiX designer actually
				// become visible in the text editor.
				if (!this.SourceCodeStorage.ContainsFile(file)) {
					TextEditorControl editor = ((ITextEditorControlProvider)this.PrimaryViewContent).TextEditorControl;
					this.SourceCodeStorage.AddFile(file, editor.Document, editor.Encoding ?? ParserService.DefaultFileEncoding, true);
				}
				
				try {
					if (!ignoreDialogIdSelectedInTextEditor) {
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
			}
			base.LoadInternal(file, stream);
		}
		
		#region Switch...WithoutSaveLoad
		
		// These four methods prevent the text editor from doing a full save/load
		// cycle when switching views. This allows the text editor to keep its
		// selection and caret position.
		
		public override bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			return newView == this || newView == this.PrimaryViewContent;
		}
		
		public override void SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			if (newView != this) {
				this.MergeAndUnloadDesigner();
			}
		}
		
		public override bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			return this.DesignerCodeFile != null &&
				(oldView == this || oldView == this.PrimaryViewContent);
		}
		
		public override void SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			if (oldView != this) {
				this.ReloadDesignerFromMemory();
			}
		}
		
		#endregion
		
		/// <summary>
		/// Gets the Wix document filename.
		/// </summary>
		public string DocumentFileName {
			get {
				return this.PrimaryFileName;
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
			return this.DesignerCodeFileContent;
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
			}
		}
		
		/// <summary>
		/// Finds the WixDialogDesigner in the current window's secondary views
		/// and switches to it.
		/// </summary>
		void OpenDesigner()
		{
			try {
				ignoreDialogIdSelectedInTextEditor = true;
				WorkbenchWindow.ActiveViewContent = this;
			} finally {
				ignoreDialogIdSelectedInTextEditor = false;
			}
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
		/// Gets the active text area control.
		/// </summary>
		TextAreaControl ActiveTextAreaControl {
			get {
				ITextEditorControlProvider provider = this.PrimaryViewContent as ITextEditorControlProvider;
				if (provider != null) {
					return provider.TextEditorControl.ActiveTextAreaControl;
				}
				return null;
			}
		}
		
		void AddToErrorList(XmlException ex)
		{
			TaskService.ClearExceptCommentTasks();
			TaskService.Add(new Task(this.PrimaryFileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error));
			WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
		}
		
		/// <summary>
		/// Cannot use ProjectService.CurrentProject since it is possible to switch
		/// to the designer without selecting the project.
		/// </summary>
		WixProject GetProject()
		{
			Solution openSolution = ProjectService.OpenSolution;
			if (openSolution != null) {
				foreach (IProject project in openSolution.Projects) {
					if (project.IsFileInProject(this.PrimaryFileName)) {
						return project as WixProject;
					}
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
						Location location = WixDocument.GetStartElementLocation(new StringReader(textArea.Document.TextContent), "Dialog", dialogId);
						textArea.JumpTo(location.Y);
					}
				}
			} catch (XmlException) {
				// Ignore
			}
		}
		
		static SharpDevelopSideBar setupDialogControlsToolBox;
		
		public static SharpDevelopSideBar SetupDialogControlsToolBox {
			get {
				WorkbenchSingleton.AssertMainThread();
				if (setupDialogControlsToolBox == null) {
					setupDialogControlsToolBox = new SharpDevelopSideBar();
					setupDialogControlsToolBox.Tabs.Add(SetupDialogControlsSideTab.CreateSideTab());
					setupDialogControlsToolBox.ActiveTab = setupDialogControlsToolBox.Tabs[0];
				}
				return setupDialogControlsToolBox;
			}
		}
		
		
		public override System.Windows.Forms.Control ToolsControl {
			get {
				return SetupDialogControlsToolBox;
			}
		}
	}
}
