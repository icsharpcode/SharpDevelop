// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IUsing
	{
		IRegion Region {
			get;
		}

		List<string> Usings {
			get;
		}

		SortedList Aliases {
			get;
		}

		IClass SearchType(string partitialTypeName);
		string SearchNamespace(string partitialNamespaceName);
	}
}
