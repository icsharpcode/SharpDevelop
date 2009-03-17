// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.TextEditor;
using SearchAndReplace;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	/// <summary>
	/// Build context menu for class members in the text editor.
	/// </summary>
	public class ClassBookmarkMenuBuilder : ISubmenuBuilder
	{
		/// <summary>
		/// Gets a specific part of the compound class.
		/// </summary>
		static IClass GetPart(IClass possibleCompound, string fileName)
		{
			CompoundClass compound = possibleCompound as CompoundClass;
			if (compound == null)
				return possibleCompound;
			
			IList<IClass> parts = compound.Parts;
			if (!string.IsNullOrEmpty(fileName)) {
				// get the part with the requested file name
				foreach (IClass part in parts) {
					if (FileUtility.IsEqualFileName(fileName, part.CompilationUnit.FileName))
						return part;
				}
			}
			
			// Fallback: get the part with the shortest file name.
			// This should prefer non-designer files over designer files.
			IClass preferredClass = parts[0];
			for (int i = 1; i < parts.Count; i++) {
				if (IsShorterFileName(parts[i].CompilationUnit.FileName, preferredClass.CompilationUnit.FileName))
					preferredClass = parts[i];
			}
			return preferredClass;
		}
		
		static bool IsShorterFileName(string a, string b)
		{
			// Fix forum-17295: compilation unit's file name might be null: prefer the non-null file
			if (a == null)
				return false;
			if (b == null)
				return true;
			return a.Length < b.Length;
		}
		
		static IClass GetCurrentPart(IClass possibleCompound)
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent != null)
				return GetPart(possibleCompound, viewContent.PrimaryFileName);
			else
				return GetPart(possibleCompound, null);
		}
		
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			MenuCommand cmd;
			IClass c;
			ClassNode classNode = owner as ClassNode;
			if (classNode != null) {
				c = classNode.Class;
			} else {
				ClassBookmark bookmark = (ClassBookmark)owner;
				c = bookmark.Class;
			}
			
			ParserService.ParseCurrentViewContent();
			c = c.ProjectContent.GetClass(c.FullyQualifiedName, c.TypeParameters.Count, c.ProjectContent.Language, GetClassOptions.LookForInnerClass);
			c = GetCurrentPart(c);
			if (c == null) {
				return new ToolStripMenuItem[0];
			}
			
			
			
			LanguageProperties language = c.ProjectContent.Language;
			
			List<ToolStripItem> list = new List<ToolStripItem>();
			
			// refactoring actions
			if (!FindReferencesAndRenameHelper.IsReadOnly(c)) {
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
							cmd = new MenuCommand(
								StringParser.Parse("${res:SharpDevelop.Refactoring.RenameFileTo}",
								                   new string[,] {{ "FileName", Path.GetFileName(correctFileName) }}),
								delegate {
									IProject p = (IProject)c.ProjectContent.Project;
									RenameFile(p, c.CompilationUnit.FileName, correctFileName);
									if (p != null) {
										p.Save();
									}
								});
							list.Add(cmd);
						} else if (language.RefactoringProvider.SupportsCreateNewFileLikeExisting && language.RefactoringProvider.SupportsGetFullCodeRangeForType) {
							// Move class to file ##
							cmd = new MenuCommand(
								StringParser.Parse("${res:SharpDevelop.Refactoring.MoveClassToFile}",
								                   new string[,] {{ "FileName", Path.GetFileName(correctFileName) }}),
								delegate {
									FindReferencesAndRenameHelper.MoveClassToFile(c, correctFileName);
								});
							list.Add(cmd);
						}
					}
				}
				
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.RenameCommand}", Rename);
				cmd.Tag = c;
				list.Add(cmd);
			
				if (language.RefactoringProvider.SupportsExtractInterface) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.ExtractInterfaceCommand}", ExtractInterface);
					cmd.Tag = c;
					list.Add(cmd);
				}
			}
			
			// navigation actions
			if (c.BaseTypes.Count > 0) {
				list.Add(new MenuSeparator());
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.GoToBaseCommand}", GoToBase);
				cmd.Tag = c;
				list.Add(cmd);
				if (c.ClassType != ClassType.Interface && !FindReferencesAndRenameHelper.IsReadOnly(c)) {
					AddImplementInterfaceCommands(c, list);
				}
			}
			
			// Search actions
			list.Add(new MenuSeparator());
			
			if (!c.IsSealed && !c.IsStatic) {
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.FindDerivedClassesCommand}", FindDerivedClasses);
				cmd.Tag = c;
				list.Add(cmd);
			}
			
			cmd = new MenuCommand("${res:SharpDevelop.Refactoring.FindReferencesCommand}", FindReferences);
			cmd.Tag = c;
			list.Add(cmd);
			
			return list.ToArray();
		}
		
		static void RenameFile(IProject p, string oldFileName, string newFileName)
		{
			FileService.RenameFile(oldFileName, newFileName, false);
			if (p != null) {
				string oldPrefix = Path.GetFileNameWithoutExtension(oldFileName) + ".";
				string newPrefix = Path.GetFileNameWithoutExtension(newFileName) + ".";
				foreach (ProjectItem item in p.Items) {
					FileProjectItem fileItem = item as FileProjectItem;
					if (fileItem == null)
						continue;
					string dependentUpon = fileItem.DependentUpon;
					if (string.IsNullOrEmpty(dependentUpon))
						continue;
					string directory = Path.GetDirectoryName(fileItem.FileName);
					dependentUpon = Path.Combine(directory, dependentUpon);
					if (FileUtility.IsEqualFileName(dependentUpon, oldFileName)) {
						fileItem.DependentUpon = FileUtility.GetRelativePath(directory, newFileName);
						string fileName = Path.GetFileName(fileItem.FileName);
						if (fileName.StartsWith(oldPrefix)) {
							RenameFile(p, fileItem.FileName, Path.Combine(directory, newPrefix + fileName.Substring(oldPrefix.Length)));
						}
					}
				}
			}
		}
	
		void AddImplementInterfaceCommandItems(List<ToolStripItem> subItems, IClass c, bool explicitImpl)
		{
			CodeGenerator codeGen = c.ProjectContent.Language.CodeGenerator;
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			foreach (IReturnType rt in c.BaseTypes) {
				IClass interf = rt.GetUnderlyingClass();
				if (interf != null && interf.ClassType == ClassType.Interface) {
					IReturnType rtCopy = rt; // copy for access by anonymous method
					EventHandler eh = delegate {
						TextEditorDocument d = new TextEditorDocument(FindReferencesAndRenameHelper.GetDocument(c));
						if (d != null)
							codeGen.ImplementInterface(rtCopy, d, explicitImpl, c);
						ParserService.ParseCurrentViewContent();
					};
					subItems.Add(new MenuCommand(ambience.Convert(interf), eh));
				}
			}
		}
		
		void AddImplementInterfaceCommands(IClass c, List<ToolStripItem> list)
		{
			CodeGenerator codeGen = c.ProjectContent.Language.CodeGenerator;
			if (codeGen == null) return;
			List<ToolStripItem> subItems = new List<ToolStripItem>();
			if (c.ProjectContent.Language.SupportsImplicitInterfaceImplementation) {
				AddImplementInterfaceCommandItems(subItems, c, false);
				if (subItems.Count > 0) {
					list.Add(new ICSharpCode.Core.WinForms.Menu("${res:SharpDevelop.Refactoring.ImplementInterfaceImplicit}", subItems.ToArray()));
					subItems = new List<ToolStripItem>();
				}
			}
			AddImplementInterfaceCommandItems(subItems, c, true);
			
			if (subItems.Count > 0) {
				if (c.ProjectContent.Language.SupportsImplicitInterfaceImplementation) {
					list.Add(new ICSharpCode.Core.WinForms.Menu("${res:SharpDevelop.Refactoring.ImplementInterfaceExplicit}", subItems.ToArray()));
				} else {
					list.Add(new ICSharpCode.Core.WinForms.Menu("${res:SharpDevelop.Refactoring.ImplementInterface}", subItems.ToArray()));
				}
			}
		}
		
		void ExtractInterface(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			FindReferencesAndRenameHelper.ExtractInterface((IClass)item.Tag);
		}
		
		void GoToBase(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IClass c = (IClass)item.Tag;
			IClass baseClass = c.BaseClass;
			if (baseClass != null) {
				string fileName = baseClass.CompilationUnit.FileName;
				if (fileName != null) {
					FileService.JumpToFilePosition(fileName, baseClass.Region.BeginLine - 1, baseClass.Region.BeginColumn - 1);
				}
			}
		}
		
		void Rename(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			FindReferencesAndRenameHelper.RenameClass((IClass)item.Tag);
		}
		
		void FindDerivedClasses(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IClass c = (IClass)item.Tag;
			IEnumerable<IClass> derivedClasses = RefactoringService.FindDerivedClasses(c, ParserService.AllProjectContents, false);
			
			List<SearchResultMatch> results = new List<SearchResultMatch>();
			foreach (IClass derivedClass in derivedClasses) {
				if (derivedClass.CompilationUnit == null) continue;
				if (derivedClass.CompilationUnit.FileName == null) continue;
				
				SearchResultMatch res = new SimpleSearchResultMatch(ClassNode.GetText(derivedClass), new TextLocation(derivedClass.Region.BeginColumn - 1, derivedClass.Region.BeginLine - 1));
				res.ProvidedDocumentInformation = FindReferencesAndRenameHelper.GetDocumentInformation(derivedClass.CompilationUnit.FileName);
				results.Add(res);
			}
			SearchResultPanel.Instance.ShowSearchResults(new SearchResult(
				StringParser.Parse("${res:SharpDevelop.Refactoring.ClassesDerivingFrom}", new string[,] {{ "Name", c.Name }}),
				results
			));
		}
		
		void FindReferences(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IClass c = (IClass)item.Tag;
			using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.FindReferences}"))
			{
				FindReferencesAndRenameHelper.ShowAsSearchResults(
					StringParser.Parse("${res:SharpDevelop.Refactoring.ReferencesTo}",
					                   new string[,] {{ "Name", c.Name }}),
					RefactoringService.FindReferences(c, monitor)
				);
			}
		}
	}
}
