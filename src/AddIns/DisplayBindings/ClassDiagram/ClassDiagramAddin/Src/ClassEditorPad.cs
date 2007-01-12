/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 12/13/2006
 * Time: 2:12 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.NRefactory.Ast;

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
			editor.MemberModified += EditorMemberModified;
			editor.ParameterActivated += EditorParameterActivated;
			editor.ParameterModified += EditorParameterModified;
			editor.ClassMemberAdded += EditorClassMemberAdded;
		}
		
		private void EditorMemberActivated (object sender, IMemberEventArgs e)
		{
			ICompilationUnit compUnit = e.Member.DeclaringType.CompilationUnit;
			FileService.JumpToFilePosition(compUnit.FileName,
			                               e.Member.Region.BeginLine - 1,
			                               e.Member.Region.BeginColumn - 1);
		}
		
		private void EditorParameterActivated (object sender, IParameterEventArgs e)
		{
			ICompilationUnit compUnit = e.Method.DeclaringType.CompilationUnit;
			FileService.JumpToFilePosition(compUnit.FileName,
			                               e.Parameter.Region.BeginLine - 1,
			                               e.Parameter.Region.BeginColumn - 1);
		}
		
		private void EditorClassMemberAdded (object sender, IMemberEventArgs e)
		{
			AbstractNode memberDom = null;
			
			DomRegion cRegion = e.Member.DeclaringType.BodyRegion;
			if (cRegion.IsEmpty) cRegion = e.Member.DeclaringType.Region;
			
			memberDom = CodeGenerator.ConvertMember(e.Member, new ClassFinder(e.Member.DeclaringType, cRegion.BeginLine + 1, 1));
			
			IProject proj = ProjectService.CurrentProject;
			IViewContent vc = FileService.OpenFile(e.Member.DeclaringType.CompilationUnit.FileName).ViewContent;
			if (vc == null) return;
			IDocument doc = GetDocument(vc);

			if (e.Member.DeclaringType is CompoundClass)
			{
				foreach (IClass c in ((CompoundClass)e.Member.DeclaringType).GetParts())
				{
					System.Diagnostics.Debug.WriteLine(c.Name+": " +c.Modifiers.ToString());
				}
			}
			proj.LanguageProperties.CodeGenerator.InsertCodeAtEnd(cRegion, doc, memberDom);
		}
		
		private static IDocument GetDocument(IViewContent viewContent)
		{
			ITextEditorControlProvider provider1 = viewContent as ITextEditorControlProvider;
			if (provider1 == null)
			{
				return null;
			}
			return new TextEditorDocument (provider1.TextEditorControl.Document);
		}
		
		private void EditorMemberModified (object sender, IMemberModificationEventArgs e)
		{
			switch (e.Modification)
			{
				case Modification.Name:
					DialogResult dr = MessageBox.Show("Rename all occurances?", "Rename Member", MessageBoxButtons.YesNoCancel);
					if (dr == DialogResult.Cancel) e.Cancel = true;
					else if (dr == DialogResult.Yes)
					{
						FindReferencesAndRenameHelper.RenameMember(e.Member, e.NewValue);
					}
					else
					{
						// TODO - place local renameing code here.
					}
					break;
				case Modification.Type:
					// TODO - place type replacment code here.
					break;
				case Modification.Modifier:
					// TODO - place visibility replacment code here.
					break;
				case Modification.Summary:
					// TODO - place summary replacment code here.
					break;
			}
		}
		
		private void EditorParameterModified (object sender, IParameterModificationEventArgs e)
		{
			switch (e.Modification)
			{
				case Modification.Name:
					DialogResult dr = MessageBox.Show("Rename all occurances?", "Rename Parameter", MessageBoxButtons.YesNoCancel);
					if (dr == DialogResult.Cancel) e.Cancel = true;
					else if (dr == DialogResult.Yes)
					{
						ResolveResult local = new LocalResolveResult(e.Method, new DefaultField.ParameterField(e.Parameter.ReturnType, e.Parameter.Name, e.Parameter.Region, e.Method.DeclaringType));
						List<Reference> list = RefactoringService.FindReferences(local, null);
						if (list == null) return;
						FindReferencesAndRenameHelper.RenameReferences(list, e.NewValue);
					}
					else
					{
						// TODO - place local renameing code here.
					}
					break;
				case Modification.Type:
					// TODO - place type replacment code here.
					break;
				case Modification.Modifier:
					// TODO - place visibility replacment code here.
					break;
				case Modification.Summary:
					// TODO - place summary replacment code here.
					break;
			}
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
