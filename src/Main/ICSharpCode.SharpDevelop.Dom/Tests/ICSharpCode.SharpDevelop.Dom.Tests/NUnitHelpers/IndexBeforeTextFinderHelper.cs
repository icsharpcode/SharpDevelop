// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.SharpDevelop.Dom.Tests.NUnitHelpers
{
	public class IndexBeforeTextFinderHelper : LanguageProperties
	{
		public IndexBeforeTextFinderHelper()
			: base(StringComparer.Ordinal)
		{
		}
		
		public TextFinder CreateIndexBeforeTextFinder(string searchText)
		{
			return new IndexBeforeTextFinder(searchText);
		}
	}
}
