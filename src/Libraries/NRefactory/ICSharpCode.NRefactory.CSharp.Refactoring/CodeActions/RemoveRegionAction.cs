// 
// RemoveRegion.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Remove region", Description = "Removes a pre processor #region/#endregion directive.")]
	public class RemoveRegionAction : CodeActionProvider
	{
		static PreProcessorDirective GetEndDirective(PreProcessorDirective directive)
		{
			var nextNode = directive.GetNextNode();
			int d = 0;
			while (nextNode != null) {
				var pp = nextNode as PreProcessorDirective;
				if (pp != null) {
					if (pp.Type == PreProcessorDirectiveType.Region) {
						d++;
					} else if (pp.Type == PreProcessorDirectiveType.Endregion) {
						if (d == 0) {
							return pp;
						}
						d--;
					}
				}
				nextNode = nextNode.GetNextNode();
			}
			return null;
		}

		static PreProcessorDirective GetStartDirective(PreProcessorDirective directive)
		{
			var nextNode = directive.GetPrevNode();
			int d = 0;
			while (nextNode != null) {
				var pp = nextNode as PreProcessorDirective;
				if (pp != null) {
					if (pp.Type == PreProcessorDirectiveType.Endregion) {
						d++;
					} else if (pp.Type == PreProcessorDirectiveType.Region) {
						if (d == 0) {
							return pp;
						}
						d--;
					}
				}
				nextNode = nextNode.GetPrevNode();
			}
			return null;
		}

		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var directive = GetDirective(context);
			if (directive == null)
				yield break;

			PreProcessorDirective endDirective = directive.Type == PreProcessorDirectiveType.Region ? GetEndDirective(directive) : GetStartDirective(directive);

			if (endDirective == null)
				yield break;

			yield return new CodeAction (context.TranslateString("Remove region"), script => {
				script.Remove (directive);
				script.Remove (endDirective);
			}, directive);
		}
		
		static PreProcessorDirective GetDirective (RefactoringContext context)
		{
			var directive = context.GetNode<PreProcessorDirective> ();
			if (directive == null || directive.Type != PreProcessorDirectiveType.Region && directive.Type != PreProcessorDirectiveType.Endregion)
				return null;
			return directive;
		}
	}
}

