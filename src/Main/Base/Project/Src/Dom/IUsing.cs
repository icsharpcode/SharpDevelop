// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IUsing
	{
		DomRegion Region {
			get;
		}
		
		List<string> Usings {
			get;
		}
		
		bool HasAliases { get; }
		
		void AddAlias(string alias, IReturnType type);
		
		/// <summary>
		/// Gets the list of aliases. Can be null when there are no aliases!
		/// </summary>
		SortedList<string, IReturnType> Aliases {
			get;
		}
		
		IReturnType SearchType(string partitialTypeName);
		string SearchNamespace(string partitialNamespaceName);
	}
}
