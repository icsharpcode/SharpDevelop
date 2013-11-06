// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Builds context menu items with commands related to the declaring type of a member.
	/// </summary>
	public class DeclaringTypeSubMenuBuilder : IMenuItemBuilder
	{
		public IEnumerable<object> BuildItems(Codon codon, object parameter)
		{
			IMember member = null;
			
			if (parameter is IMemberModel) {
				// Menu is directly created from a member model (e.g. bookmarks etc.)
				member = ((IMemberModel) parameter).Resolve();
			} else if (parameter is ResolveResult) {
				MemberResolveResult resolveResult = parameter as MemberResolveResult;
				if (resolveResult != null) {
					member = resolveResult.Member;
				}
			} else if (parameter is ITextEditor) {
				// Shown in context menu of a text editor
				MemberResolveResult resolveResult = GetResolveResult((ITextEditor) parameter) as MemberResolveResult;
				if (resolveResult != null) {
					member = resolveResult.Member;
				}
			}
			
			if (member == null) {
				return null;
			}

			IType declaringType = member.DeclaringTypeDefinition;
			if (declaringType == null) {
				return null;
			}
			
			var items = new List<object>();
			var declaringTypeItem = new MenuItem() {
				Header = SD.ResourceService.GetString("SharpDevelop.Refactoring.DeclaringType") + ": " + declaringType.Name,
				Icon = new Image() { Source = ClassBrowserIconService.GetIcon(declaringType).ImageSource }
			};
			
			var subItems = MenuService.CreateMenuItems(
				null, new TypeResolveResult(declaringType), "/SharpDevelop/EntityContextMenu");
			if (subItems != null) {
				foreach (var item in subItems) {
					declaringTypeItem.Items.Add(item);
				}
			}
			items.Add(declaringTypeItem);
			
			return items;
		}
		
		static ResolveResult GetResolveResult(ITextEditor currentEditor)
		{
			if (currentEditor != null) {
				return SD.ParserService.Resolve(currentEditor, currentEditor.Caret.Location);
			}
			
			return ErrorResolveResult.UnknownError;
		}
	}
}
