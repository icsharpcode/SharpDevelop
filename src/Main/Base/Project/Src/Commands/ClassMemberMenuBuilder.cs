// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.SharpDevelop.Commands
{
	/// <summary>
	/// Build context menu for class members in the text editor.
	/// </summary>
	public class ClassMemberMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			MenuCommand cmd;
			ClassMemberBookmark bookmark = (ClassMemberBookmark)owner;
			IMember member = bookmark.Member;
			List<ToolStripItem> list = new List<ToolStripItem>();
			
			cmd = new MenuCommand("&Rename", Rename);
			cmd.Tag = member;
			list.Add(cmd);
			
			if (member.IsOverride) {
				cmd = new MenuCommand("Go to &base class", GoToBase);
				cmd.Tag = member;
				list.Add(cmd);
			}
			if (member.IsVirtual || member.IsAbstract || (member.IsOverride && !member.DeclaringType.IsSealed)) {
				cmd = new MenuCommand("Find &overrides", FindOverrides);
				cmd.Tag = member;
				list.Add(cmd);
			}
			
			cmd = new MenuCommand("&Find references", FindReferences);
			cmd.Tag = member;
			list.Add(cmd);
			
			return list.ToArray();
		}
		
		#region GoToBase
		void GoToBase(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			IMember baseMember = FindBaseMember(member);
			if (baseMember != null) {
				ICompilationUnit cu = baseMember.DeclaringType.CompilationUnit;
				if (cu != null) {
					string fileName = cu.FileName;
					if (fileName != null) {
						if (baseMember.Region != null && baseMember.Region.BeginLine > 0) {
							FileService.JumpToFilePosition(fileName, baseMember.Region.BeginLine - 1, 0);
						} else {
							FileService.JumpToFilePosition(fileName, 0, 0);
						}
						return;
					}
				}
			}
		}
		
		IMember FindBaseMember(IMember member)
		{
			IClass parentClass = member.DeclaringType;
			IClass baseClass = parentClass.BaseClass;
			if (baseClass == null) return null;
			if (member is IMethod) {
				IMethod parentMethod = (IMethod)member;
				foreach (IClass childClass in baseClass.ClassInheritanceTree) {
					foreach (IMethod m in childClass.Methods) {
						if (string.Equals(parentMethod.Name, m.Name, StringComparison.InvariantCultureIgnoreCase)) {
							if (m.IsStatic == parentMethod.IsStatic) {
								if (DiffUtility.Compare(parentMethod.Parameters, m.Parameters) == 0) {
									return m;
								}
							}
						}
					}
				}
			} else if (member is IProperty) {
				IProperty parentMethod = (IProperty)member;
				foreach (IClass childClass in baseClass.ClassInheritanceTree) {
					foreach (IProperty m in childClass.Properties) {
						if (string.Equals(parentMethod.Name, m.Name, StringComparison.InvariantCultureIgnoreCase)) {
							if (m.IsStatic == parentMethod.IsStatic) {
								if (DiffUtility.Compare(parentMethod.Parameters, m.Parameters) == 0) {
									return m;
								}
							}
						}
					}
				}
			}
			return null;
		}
		#endregion
		
		void Rename(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			MessageService.ShowMessage("Not implemented.");
		}
		
		void FindOverrides(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			MessageService.ShowMessage("Not implemented.");
		}
		
		void FindReferences(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			MessageService.ShowMessage("Not implemented.");
		}
	}
}
