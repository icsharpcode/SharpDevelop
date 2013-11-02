// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Description of DeclaringTypeSubMenuBuilder.
	/// </summary>
	public class DeclaringTypeSubMenuBuilder : IMenuItemBuilder
	{
		public DeclaringTypeSubMenuBuilder()
		{
		}

		public System.Collections.Generic.IEnumerable<object> BuildItems(Codon codon, object parameter)
		{
			MemberResolveResult resolveResult = GetResolveResult() as MemberResolveResult;
			if (resolveResult == null) {
				return null;
			}
			
			var items = new List<MenuItem>();
			items.Add(new MenuItem() {
			          	Header = "TEST"
			          });
			
//			var items = MenuService.CreateMenuItems((UIElement) SD.Workbench, SD.Workbench, "");
			
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
