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
			
			if (language == LanguageProperties.CSharp) {
				AddImplementAbstractClassCommands(c, resultItems);
			}
			
			if (c.BaseTypes.Count > 0 && c.ClassType != ClassType.Interface && !FindReferencesAndRenameHelper.IsReadOnly(c)) {
				AddImplementInterfaceCommands(c, resultItems);
			}
			
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
		
		void AddImplementInterfaceCommands(IClass c, List<ToolStripItem> list)
		{
			CodeGenerator codeGen = c.ProjectContent.Language.CodeGenerator;
			if (codeGen == null) return;
			List<ToolStripItem> subItems = new List<ToolStripItem>();
			
			if (c.ProjectContent.Language.SupportsImplicitInterfaceImplementation) {
				// 'Implement interface (implicit)' menu item with subitems
				AddImplementInterfaceCommandItems(subItems, c, false);
				if (subItems.Count > 0) {
					list.Add(new ICSharpCode.Core.WinForms.Menu("${res:SharpDevelop.Refactoring.ImplementInterfaceImplicit}", subItems.ToArray()));
					subItems = new List<ToolStripItem>();
				}
			}
			
			// 'Implement interface (explicit)' menu item with subitems
			AddImplementInterfaceCommandItems(subItems, c, true);
			if (subItems.Count > 0) {
				string explicitMenuItemLabel = StringParser.Parse(c.ProjectContent.Language.SupportsImplicitInterfaceImplementation
				                                                  ? "${res:SharpDevelop.Refactoring.ImplementInterfaceExplicit}"
				                                                  : "${res:SharpDevelop.Refactoring.ImplementInterface}");
				list.Add(new ICSharpCode.Core.WinForms.Menu(explicitMenuItemLabel, subItems.ToArray()));
			}
		}
		
		void AddImplementInterfaceCommandItems(List<ToolStripItem> subItems, IClass c, bool explicitImpl)
		{
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			MakeMenuItemsFromActions(subItems, RefactoringService.GetImplementInterfaceActions(c, explicitImpl), ambience);
		}
		
		void AddImplementAbstractClassCommands(IClass c, List<ToolStripItem> list)
		{
			List<ToolStripItem> subItems = new List<ToolStripItem>();
			AddImplementAbstractClassCommandItems(subItems, c);
			if (subItems.Count > 0) {
				list.Add(new ICSharpCode.Core.WinForms.Menu("${res:SharpDevelop.Refactoring.ImplementAbstractClass}", subItems.ToArray()));
			}
		}
		
		void AddImplementAbstractClassCommandItems(List<ToolStripItem> subItems, IClass c)
		{
			IAmbience ambience = c.ProjectContent.Language.GetAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			MakeMenuItemsFromActions(subItems, RefactoringService.GetImplementAbstractClassActions(c), ambience);
		}
		
		void MakeMenuItemsFromActions(List<ToolStripItem> subItems, IEnumerable<RefactoringService.ImplementAbstractClassAction> actions, IAmbience labelAmbience)
		{
			foreach (var action in actions) {
				var actionCopy = action;
				subItems.Add(new MenuCommand(
					labelAmbience.Convert(actionCopy.ClassToImplement), 
					delegate { actionCopy.Execute(); }));
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
