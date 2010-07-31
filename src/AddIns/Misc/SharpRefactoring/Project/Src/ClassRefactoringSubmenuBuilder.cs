// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Commands;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring
{
	/// <summary>
	/// Generates items the class submenu.
	/// Paths:
	/// /SharpDevelop/ViewContent/DefaultTextEditor/ClassBookmarkContextMenu, id=classCodeGenerators
	/// /SharpDevelop/Pads/ClassBrowser/ClassContextMenu, id=classCodeGenerators
	/// 
	/// This builder generates items to the same submenu as <see cref="ICSharpCode.SharpDevelop.Editor.Commands.ClassBookmarkSubmenuBuilder."></see>.
	/// </summary>
	public class ClassRefactoringSubmenuBuilder : ISubmenuBuilder, IMenuItemBuilder
	{
		public System.Collections.ICollection BuildItems(Codon codon, object owner)
		{
			return BuildSubmenu(codon, owner).TranslateToWpf();
		}

		public System.Windows.Forms.ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			List<ToolStripItem> resultItems = new List<ToolStripItem>();
			
			IClass c = ClassBookmarkSubmenuBuilder.GetClass(owner);
			if (c == null) {
				return new ToolStripMenuItem[0];
			}
			LanguageProperties language = c.ProjectContent.Language;
			
			if (!FindReferencesAndRenameHelper.IsReadOnly(c)) {
				AddCorrectClassFileNameCommands(c, resultItems);
				
				AddRenameCommand(c, resultItems);
				
				if (language.RefactoringProvider.SupportsExtractInterface) {
					AddExtractInterfaceCommand(c, resultItems);
				}
			}
			
			return resultItems.ToArray();
		}
		
		void AddRenameCommand(IClass c, List<ToolStripItem> resultItems)
		{
			var cmd = new MenuCommand("${res:SharpDevelop.Refactoring.RenameCommand}", Rename);
			cmd.ShortcutKeys = MenuCommand.ParseShortcut("Control|R");
			cmd.Tag = c;
			resultItems.Add(cmd);
		}
		
		void AddExtractInterfaceCommand(IClass c, List<ToolStripItem> resultItems)
		{
			var cmd = new MenuCommand("${res:SharpDevelop.Refactoring.ExtractInterfaceCommand}", ExtractInterface);
			cmd.Tag = c;
			resultItems.Add(cmd);
		}
		
		void AddCorrectClassFileNameCommands(IClass c, List<ToolStripItem> resultItems)
		{
			if (c.DeclaringType == null &&
			    !c.BodyRegion.IsEmpty &&
			    !c.Name.Equals(Path.GetFileNameWithoutExtension(c.CompilationUnit.FileName),
			                   StringComparison.OrdinalIgnoreCase))
			{
				// File name does not match class name
				string correctFileName = Path.Combine(Path.GetDirectoryName(c.CompilationUnit.FileName),
				                                      c.Name + Path.GetExtension(c.CompilationUnit.FileName));
				if (FileUtility.IsValidPath(correctFileName)
				    && Path.IsPathRooted(correctFileName)
				    && !File.Exists(correctFileName))
				{
					if (c.CompilationUnit.Classes.Count == 1) {
						// Rename file to ##
						var cmd = new MenuCommand(
							StringParser.Parse("${res:SharpDevelop.Refactoring.RenameFileTo}",
							                   new string[,] {{ "FileName", Path.GetFileName(correctFileName) }}),
							delegate {
								IProject p = (IProject)c.ProjectContent.Project;
								RefactoringHelpers.RenameFile(p, c.CompilationUnit.FileName, correctFileName);
								if (p != null) {
									p.Save();
								}
							});
						resultItems.Add(cmd);
					} else {
						var refactoringProvider = c.ProjectContent.Language.RefactoringProvider;
						if (refactoringProvider.SupportsCreateNewFileLikeExisting && refactoringProvider.SupportsGetFullCodeRangeForType) {
							// Move class to file ##
							var cmd = new MenuCommand(
								StringParser.Parse("${res:SharpDevelop.Refactoring.MoveClassToFile}",
								                   new string[,] {{ "FileName", Path.GetFileName(correctFileName) }}),
								delegate {
									FindReferencesAndRenameHelper.MoveClassToFile(c, correctFileName);
								});
							resultItems.Add(cmd);
						}
					}
				}
			}
		}
		
		#region Implementation
		void Rename(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			FindReferencesAndRenameHelper.RenameClass((IClass)item.Tag);
		}
		
		void ExtractInterface(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			FindReferencesAndRenameHelper.ExtractInterface((IClass)item.Tag);
		}
		#endregion
	}

}
