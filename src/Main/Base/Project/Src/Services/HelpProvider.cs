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
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop
{
	public class HelpProvider
	{
		public static List<HelpProvider> GetProviders()
		{
			return AddInTree.BuildItems<HelpProvider>("/SharpDevelop/Services/HelpProvider", null, false);
		}
		
		public static void ShowHelp(IEntity c)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			foreach (HelpProvider p in GetProviders()) {
				if (p.TryShowHelp(c))
					return;
			}
			new HelpProvider().TryShowHelp(c);
		}
		
		public virtual bool TryShowHelp(IEntity c)
		{
			return TryShowHelp(c.FullName);
		}
		
		public static void ShowHelp(string fullTypeName)
		{
			if (fullTypeName == null)
				throw new ArgumentNullException("fullTypeName");
			foreach (HelpProvider p in GetProviders()) {
				if (p.TryShowHelp(fullTypeName))
					return;
			}
			new HelpProvider().TryShowHelp(fullTypeName);
		}
		
		public virtual bool TryShowHelp(string fullTypeName)
		{
			FileService.OpenFile("http://msdn2.microsoft.com/library/" + Uri.EscapeDataString(fullTypeName));
			return true;
		}
		
		public static void ShowHelpByKeyword(string keyword)
		{
			if (keyword == null)
				throw new ArgumentNullException("keyword");
			foreach (HelpProvider p in GetProviders()) {
				if (p.TryShowHelpByKeyword(keyword))
					return;
			}
		}
		
		public virtual bool TryShowHelpByKeyword(string keyword)
		{
			return false;
		}
	}
}
