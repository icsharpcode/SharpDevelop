// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop
{
	public class HelpProvider
	{
		public static List<HelpProvider> GetProviders()
		{
			return AddInTree.BuildItems<HelpProvider>("/SharpDevelop/Services/HelpProvider", null, false);
		}
		
		public static void ShowHelp(IClass c)
		{
			foreach (HelpProvider p in GetProviders()) {
				if (p.TryShowHelp(c))
					return;
			}
			new HelpProvider().TryShowHelp(c);
		}
		
		public virtual bool TryShowHelp(IClass c)
		{
			return TryShowHelp(c.FullyQualifiedName);
		}
		
		public static void ShowHelp(IMember m)
		{
			foreach (HelpProvider p in GetProviders()) {
				if (p.TryShowHelp(m))
					return;
			}
			new HelpProvider().TryShowHelp(m);
		}
		
		public virtual bool TryShowHelp(IMember m)
		{
			return TryShowHelp(m.FullyQualifiedName);
		}
		
		public static void ShowHelp(string fullTypeName)
		{
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
