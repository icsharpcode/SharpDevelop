// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpBinding.Parser;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding.Refactoring
{
	sealed class CSharpContextActionWrapper : IContextAction
	{
		readonly IContextActionProvider provider;
		readonly string description;
		readonly Func<SDRefactoringContext, CodeAction> getUpdatedCodeAction;
		
		public CSharpContextActionWrapper(IContextActionProvider provider, CodeAction codeAction,
		                                  Func<SDRefactoringContext, CodeAction> getUpdatedCodeAction)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");
			if (codeAction == null)
				throw new ArgumentNullException("codeAction");
			if (getUpdatedCodeAction == null)
				throw new ArgumentNullException("getUpdatedCodeAction");
			this.provider = provider;
			this.description = codeAction.Description;
			this.getUpdatedCodeAction = getUpdatedCodeAction;
			// Don't maintain a reference to 'action', it indirectly references the compilation etc.
		}
		
		public IContextActionProvider Provider {
			get { return provider; }
		}
		
		public string DisplayName {
			get { return description; }
		}
		
		public string GetDisplayName(EditorRefactoringContext context)
		{
			return DisplayName;
		}
		
		public void Execute(EditorRefactoringContext context)
		{
			var resolver = context.GetAstResolverAsync().Result;
			var refactoringContext = new SDRefactoringContext(context.Editor, resolver, context.CaretLocation);
			var action = getUpdatedCodeAction(refactoringContext);
			if (action != null) {
				using (var script = refactoringContext.StartScript()) {
					action.Run(script);
				}
			}
		}
	}
}
