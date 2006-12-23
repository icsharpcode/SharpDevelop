// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Itai Bar-Haim" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;

using ClassDiagram;

namespace ClassDiagramAddin
{
	/// <summary>
	/// Description of the pad content
	/// </summary>
	public class ClassEditorPad : AbstractPadContent
	{
		ClassEditor editor = new ClassEditor();
		
		/// <summary>
		/// Creates a new ClassEditorPad object
		/// </summary>
		public ClassEditorPad()
		{
			editor.MemberActivated += EditorMemberActivated;
		}
		
		private void EditorMemberActivated (object sender, IMemberEventArgs e)
		{
			ICompilationUnit compUnit = e.Member.DeclaringType.CompilationUnit;
			FileService.JumpToFilePosition(compUnit.FileName,
			                               e.Member.Region.BeginLine - 1,
			                               e.Member.Region.BeginColumn - 1);

		}
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override Control Control
		{
			get { return editor; }
		}
		
		/// <summary>
		/// Refreshes the pad
		/// </summary>
		public override void RedrawContent()
		{
			// TODO: Refresh the whole pad control here, renew all resource strings whatever
			//       Note that you do not need to recreate the control.
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			editor.Dispose();
		}
		
		private static TextEditorControl GetTextEditorControl()
		{
			TextEditorControl tec = null;
			IWorkbenchWindow window1 = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if ((window1 != null) && (window1.ViewContent is ITextEditorControlProvider))
			{
				tec = ((ITextEditorControlProvider) window1.ViewContent).TextEditorControl;
			}
			return tec;
		}
	}
}
