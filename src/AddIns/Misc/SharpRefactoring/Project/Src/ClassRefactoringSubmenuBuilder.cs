// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
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
