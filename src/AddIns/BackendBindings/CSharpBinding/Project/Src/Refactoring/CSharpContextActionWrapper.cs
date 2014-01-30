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
