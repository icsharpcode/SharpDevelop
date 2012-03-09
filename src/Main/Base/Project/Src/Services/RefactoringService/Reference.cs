// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor.Search;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// A reference to a class or class member.
	/// </summary>
	public class Reference : SearchResultMatch
	{
		DomRegion region;
		ResolveResult resolveResult;
		
		public Reference(DomRegion region, ResolveResult resolveResult, int offset, int length, HighlightedInlineBuilder builder)
			: base(FileName.Create(region.FileName), region.Begin, region.End, offset, length, builder)
		{
			if (region.IsEmpty)
				throw new ArgumentException("Region must not be empty");
			if (resolveResult == null)
				throw new ArgumentNullException("resolveResult");
			this.resolveResult = resolveResult;
		}
		
		public ResolveResult ResolveResult {
			get { return resolveResult; }
		}
	}
}
