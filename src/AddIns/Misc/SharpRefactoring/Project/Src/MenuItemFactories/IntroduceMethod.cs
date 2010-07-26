// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;
using SharpRefactoring.Gui;
using Ast = ICSharpCode.NRefactory.Ast;

namespace SharpRefactoring
{
	/// <summary>
	/// Provides <see cref="GenerateCode.GetContextAction"></see> as editor context menu entry.
	/// </summary>
	public class IntroduceMethod : IRefactoringMenuItemFactory
	{
		public MenuItem Create(RefactoringMenuContext context)
		{
			if (context.ExpressionResult.Context == ExpressionContext.Attribute)
				return null;
			
			var introduceCodeAction = GenerateCode.GetContextAction(context.ResolveResult, context.Editor);
			if (introduceCodeAction == null)
				return null;
			
			var item = new MenuItem() {
				Header = introduceCodeAction.Title,
				Icon = ClassBrowserIconService.GotoArrow.CreateImage()
			};
			item.Click += delegate { introduceCodeAction.Execute(); };
			return item;
		}
	}
}
