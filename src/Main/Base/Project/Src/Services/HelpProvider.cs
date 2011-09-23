// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

		public static bool ShowHelp(IClass c)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			foreach (HelpProvider p in GetProviders()) {
				if (p.TryShowHelp(c))
					return true;
			}
			return new HelpProvider().TryShowHelp(c);
		}

		public virtual bool TryShowHelp(IClass c)
		{
			return TryShowHelp(c.FullyQualifiedName);
		}

		public static bool ShowHelp(IMember m)
		{
			if (m == null)
				throw new ArgumentNullException("m");
			foreach (HelpProvider p in GetProviders()) {
				if (p.TryShowHelp(m))
					return true;
			}
			return new HelpProvider().TryShowHelp(m);
		}

		public virtual bool TryShowHelp(IMember m)
		{
			IMethod method = m as IMethod;
			if (method != null && method.IsConstructor)
				return TryShowHelp(m.DeclaringType.FullyQualifiedName + "." + m.DeclaringType.Name);
			else
				return TryShowHelp(m.FullyQualifiedName);
		}

		public static bool ShowHelp(string fullTypeName)
		{
			if (fullTypeName == null)
				throw new ArgumentNullException("fullTypeName");
			foreach (HelpProvider p in GetProviders()) {
				if (p.TryShowHelp(fullTypeName))
					return true;
			}
			return new HelpProvider().TryShowHelp(fullTypeName);
		}

		public virtual bool TryShowHelp(string fullTypeName)
		{
			FileService.OpenFile("http://msdn2.microsoft.com/library/" + Uri.EscapeDataString(fullTypeName));
			return true;
		}

		public static bool ShowHelpByKeyword(string keyword)
		{
			if (keyword == null)
				throw new ArgumentNullException("keyword");
			foreach (HelpProvider p in GetProviders()) {
				if (p.TryShowHelpByKeyword(keyword))
					return true;
			}

			return false;
		}

		public virtual bool TryShowHelpByKeyword(string keyword)
		{
			return false;
		}

		/// <summary>
		/// A general ShowHelp step based on caret offset in text editor; 
		/// this is useful when parsing failed but it's still good to show a context help based on text around the caret
		/// </summary>
		/// <param name="offset">offset of caret in the whole document</param>
		/// <returns>true if a help is provided by provider, false otherwise</returns>
		public static bool ShowHelp(int offset)
		{
			foreach (HelpProvider p in GetProviders()) {
				if (p.TryShowHelp(offset))
					return true;
			}

			return new HelpProvider().TryShowHelp(offset);
		}

		/// <summary>
		/// ShowHelp based on caret offset in text editor
		/// </summary>
		/// <param name="offset"></param>
		/// <returns>true if a help is provided, false otherwise</returns>
		public virtual bool TryShowHelp(int offset)
		{
			return false;
		}
	}
}