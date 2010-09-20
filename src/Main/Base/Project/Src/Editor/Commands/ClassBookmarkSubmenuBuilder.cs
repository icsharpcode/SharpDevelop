// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Linq;
using ICSharpCode.SharpDevelop.Editor.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Build context submenu for class members in the text editor.
	/// Paths:
	/// /SharpDevelop/ViewContent/DefaultTextEditor/ClassBookmarkContextMenu, id=MenuBuilder
	/// /SharpDevelop/Pads/ClassBrowser/ClassContextMenu, id=MenuBuilder
	/// </summary>
	public class ClassBookmarkSubmenuBuilder : ISubmenuBuilder, IMenuItemBuilder
	{
		public System.Collections.ICollection BuildItems(Codon codon, object owner)
		{
			return BuildSubmenu(codon, owner).TranslateToWpf();
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
			
			List<ToolStripItem> list = new List<ToolStripItem>();
			
			// "Go to base" for classes is not that useful as it is faster to click the base class in the editor.
			// Also, we have "Find base classes" which shows all base classes.
//			if (c.BaseTypes.Count > 0) {
//				list.Add(new MenuSeparator());
//				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.GoToBaseCommand}", GoToBase);
//				cmd.Tag = c;
//				list.Add(cmd);
//			}
			
			cmd = FindReferencesAndRenameHelper.MakeFindReferencesMenuCommand(FindReferences);
			cmd.Tag = c;
			list.Add(cmd);
			list.AddIfNotNull(MakeFindBaseClassesItem(c));
			list.AddIfNotNull(MakeFindDerivedClassesItem(c));
			
			return list.ToArray();
		}
		
		void FindReferences(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IClass c = (IClass)item.Tag;
			FindReferencesAndRenameHelper.RunFindReferences(c);
		}
		
		MenuCommand MakeFindDerivedClassesItem(IClass baseClass)
		{
			if (baseClass == null || baseClass.IsStatic || baseClass.IsSealed)
				return null;
			var item = new MenuCommand(StringParser.Parse("${res:SharpDevelop.Refactoring.FindDerivedClassesCommand}"));
			item.ShortcutKeys = System.Windows.Forms.Keys.F6;
			//item.Image = ClassBrowserIconService.Class.Bitmap;
			item.Click += delegate {
				ContextActionsHelper.MakePopupWithDerivedClasses(baseClass).OpenAtCaretAndFocus();
			};
			return item;
		}
		
		MenuCommand MakeFindBaseClassesItem(IClass @class)
		{
			if (@class == null || @class.BaseTypes == null || @class.BaseTypes.Count == 0)
				return null;
			var item = new MenuCommand(StringParser.Parse("${res:SharpDevelop.Refactoring.FindBaseClassesCommand}"));
			//item.Image = ClassBrowserIconService.Class.Bitmap;
			item.Click += delegate {
				ContextActionsHelper.MakePopupWithBaseClasses(@class).OpenAtCaretAndFocus();
			};
			return item;
		}
		
//		void GoToBase(object sender, EventArgs e)
//		{
//			MenuCommand item = (MenuCommand)sender;
//			IClass c = (IClass)item.Tag;
//			IClass baseClass = c.BaseClass;
//			if (baseClass != null) {
//				string fileName = baseClass.CompilationUnit.FileName;
//				if (fileName != null) {
//					FileService.JumpToFilePosition(fileName, baseClass.Region.BeginLine, baseClass.Region.BeginColumn);
//				}
//			}
//		}
		
		public static IClass GetClass(object menuOwner)
		{
			IClass c;
			ClassNode classNode = menuOwner as ClassNode;
			if (classNode != null) {
				c = classNode.Class;
			} else {
				ClassBookmark bookmark = (ClassBookmark)menuOwner;
				c = bookmark.Class;
			}
			ParserService.ParseCurrentViewContent();
			c = c.ProjectContent.GetClass(c.FullyQualifiedName, c.TypeParameters.Count, c.ProjectContent.Language, GetClassOptions.LookForInnerClass);
			return GetCurrentPart(c);
		}
		
		public static IClass GetCurrentPart(IClass possibleCompound)
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent != null)
				return GetPart(possibleCompound, viewContent.PrimaryFileName);
			else
				return GetPart(possibleCompound, null);
		}
		
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
	}
}
