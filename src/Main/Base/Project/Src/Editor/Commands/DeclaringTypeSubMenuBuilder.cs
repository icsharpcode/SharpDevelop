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
