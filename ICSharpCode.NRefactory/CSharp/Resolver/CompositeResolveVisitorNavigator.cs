// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	public sealed class CompositeResolveVisitorNavigator : IResolveVisitorNavigator
	{
		IResolveVisitorNavigator[] navigators;
		
		public CompositeResolveVisitorNavigator(IResolveVisitorNavigator[] navigators)
		{
			this.navigators = navigators;
		}
		
		public ResolveVisitorNavigationMode Scan(AstNode node)
		{
			ResolveVisitorNavigationMode mode = ResolveVisitorNavigationMode.Skip;
			foreach (var navigator in navigators) {
				ResolveVisitorNavigationMode newMode = navigator.Scan(node);
				if (newMode == ResolveVisitorNavigationMode.ResolveAll)
					return newMode; // ResolveAll has highest priority
				if (newMode == ResolveVisitorNavigationMode.Resolve)
					mode = newMode; // resolve has high priority and replaces the previous mode
				else if (mode == ResolveVisitorNavigationMode.Skip)
					mode = newMode; // skip has lowest priority and always gets replaced
			}
			return mode;
		}
		
		public void Resolved(AstNode node, ResolveResult result)
		{
			foreach (var navigator in navigators) {
				navigator.Resolved(node, result);
			}
		}
	}
}
