// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Collections.Generic;
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
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring
{
	public class ClassCodeGeneratorMenuBuilder : ISubmenuBuilder, IMenuItemBuilder
	{
		public System.Collections.ICollection BuildItems(Codon codon, object owner)
		{
			return BuildSubmenu(codon, owner).TranslateToWpf();
		}

		public System.Windows.Forms.ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			List<ToolStripItem> resultItems = new List<ToolStripItem>();
			
			IClass c;
			ClassNode classNode = owner as ClassNode;
			if (classNode != null) {
				c = classNode.Class;
			} else {
				ClassBookmark bookmark = (ClassBookmark)owner;
				c = bookmark.Class;
			}
			
			LanguageProperties language = c.ProjectContent.Language;
			if (language != LanguageProperties.CSharp)
				return resultItems.ToArray();
			
			AddImplementAbstractClassCommands(c, resultItems);
			
			return resultItems.ToArray();
		}
		
		void AddImplementAbstractClassCommandItems(List<ToolStripItem> subItems, IClass c)
		{
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			foreach (IReturnType rt in c.BaseTypes) {
				IClass abstractClass = rt.GetUnderlyingClass();
				if (abstractClass != null && abstractClass.ClassType == ClassType.Class && abstractClass.IsAbstract) {
					IReturnType rtCopy = rt; // copy for access by anonymous method
					EventHandler eh = delegate {
						var d = FindReferencesAndRenameHelper.GetDocument(c);
						if (d != null)
							ImplementAbstractClass(d, c, abstractClass);
						ParserService.ParseCurrentViewContent();
					};
					subItems.Add(new MenuCommand(ambience.Convert(abstractClass), eh));
				}
			}
		}
		
		void AddImplementAbstractClassCommands(IClass c, List<ToolStripItem> list)
		{
			List<ToolStripItem> subItems = new List<ToolStripItem>();
			AddImplementAbstractClassCommandItems(subItems, c);
			
			if (subItems.Count > 0) {
				list.Add(new ICSharpCode.Core.WinForms.Menu("${res:SharpDevelop.Refactoring.ImplementAbstractClass}", subItems.ToArray()));
			}
		}
		
		#region Code generation
		void ImplementAbstractClass(IDocument document, IClass target, IClass abstractClass)
		{
			CodeGenerator generator = target.ProjectContent.Language.CodeGenerator;
			RefactoringDocumentAdapter doc = new RefactoringDocumentAdapter(document);
			var pos = doc.OffsetToPosition(doc.PositionToOffset(target.BodyRegion.EndLine, target.BodyRegion.EndColumn) - 1);
			ClassFinder context = new ClassFinder(target, pos.Line, pos.Column);
			
			foreach (IMember member in abstractClass.AllMembers.Where(m => m.IsAbstract && !HasMember(m, target))) {
				generator.InsertCodeAtEnd(target.BodyRegion, doc, generator.GetOverridingMethod(member, context));
			}
		}
		#endregion
		
		#region Helpers
		SignatureComparer comparer = new SignatureComparer();
		
		bool HasMember(IMember member, IClass containingClass)
		{
			foreach (IMember m in containingClass.AllMembers) {
				if (comparer.Equals(member, m))
					return true;
			}
			
			return false;
		}
		#endregion
	}
}
