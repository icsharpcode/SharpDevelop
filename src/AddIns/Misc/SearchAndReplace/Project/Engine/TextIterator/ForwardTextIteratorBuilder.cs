// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.Search;
using System;
using System.Diagnostics;

namespace SearchAndReplace
{
	public class ForwardTextIteratorBuilder : ITextIteratorBuilder
	{
		public ITextIterator BuildTextIterator(ProvidedDocumentInformation info)
		{
			Debug.Assert(info != null);
			return new ForwardTextIterator(info);
		}
	}
}
