/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 13.08.2005
 * Time: 15:14
 */

using System;
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class HelpProvider
	{
		public static ArrayList GetProviders()
		{
			return AddInTree.BuildItems("/SharpDevelop/Services/HelpProvider", null, false);
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
