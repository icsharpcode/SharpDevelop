// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	public class OverrideCompletionItemProvider : AbstractCompletionItemProvider
	{
		public static IEnumerable<IMember> GetOverridableMembers(IClass c)
		{
			if (c == null) {
				throw new ArgumentException("c");
			}
			
			return MemberLookupHelper.GetAccessibleMembers(c.BaseType, c, c.ProjectContent.Language, true)
				.Where(m => m.IsOverridable && !m.IsConst);
		}
		
		/// <summary>
		/// Gets a list of overridable methods from the specified class.
		/// A better location for this method is in the DefaultClass
		/// class and the IClass interface.
		/// </summary>
		public static IMethod[] GetOverridableMethods(IClass c)
		{
			return GetOverridableMembers(c).OfType<IMethod>().ToArray();
		}
		
		/// <summary>
		/// Gets a list of overridable properties from the specified class.
		/// </summary>
		public static IProperty[] GetOverridableProperties(IClass c)
		{
			return GetOverridableMembers(c).OfType<IProperty>().ToArray();
		}
		
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(editor.FileName);
			if (parseInfo == null) return null;
			IClass c = parseInfo.CompilationUnit.GetInnermostClass(editor.Caret.Line, editor.Caret.Column);
			if (c == null) return null;
			LanguageProperties language = c.ProjectContent.Language;
			OverrideCompletionItemList result = new OverrideCompletionItemList();
			foreach (IMember m in GetOverridableMembers(c)) {
				if (language.ShowMemberInOverrideCompletion(m)) {
					result.Items.Add(new OverrideCompletionItem(m));
				}
			}
			result.SortItems();
			return result;
		}
		
		sealed class OverrideCompletionItemList : DefaultCompletionItemList
		{
			public override CompletionItemListKeyResult ProcessInput(char key)
			{
				if (key == '(')
					return CompletionItemListKeyResult.NormalKey;
				else
					return base.ProcessInput(key);
			}
		}
	}
}
