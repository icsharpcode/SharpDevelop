// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
