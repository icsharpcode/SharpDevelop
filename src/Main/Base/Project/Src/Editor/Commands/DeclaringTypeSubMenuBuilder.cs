// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Builds context menu items with commands related to the declaring type of a member.
	/// </summary>
	public class DeclaringTypeSubMenuBuilder : IMenuItemBuilder
	{
		public IEnumerable<object> BuildItems(Codon codon, object parameter)
		{
			MemberResolveResult resolveResult = GetResolveResult() as MemberResolveResult;
			if (resolveResult == null) {
				return null;
			}
			
			IMember member = resolveResult.Member;
			IType declaringType = member.DeclaringTypeDefinition;
			if (declaringType == null) {
				return null;
			}
			
			var items = new List<object>();
			var declaringTypeItem = new MenuItem() {
				Header = "Declaring type: " + declaringType.Name,
				Icon = ClassBrowserIconService.GetIcon(declaringType).ImageSource
			};
			
			var subItems = MenuService.CreateMenuItems(
				null, new TypeResolveResult(declaringType), "/SharpDevelop/ViewContent/TextEditor/ContextMenu/TypeContextMenu");
			if (subItems != null) {
				foreach (var item in subItems) {
					declaringTypeItem.Items.Add(item);
				}
			}
			
			items.Add(declaringTypeItem);
			items.Add(new Separator());
			
			return items;
		}
		
		static ResolveResult GetResolveResult()
		{
			ITextEditor currentEditor = SD.GetActiveViewContentService<ITextEditor>();
			if (currentEditor != null) {
				return SD.ParserService.Resolve(currentEditor, currentEditor.Caret.Location);
			} else {
				return ErrorResolveResult.UnknownError;
			}
		}
	}
}
