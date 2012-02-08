// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptRegion
	{
		JavaScriptRegionStart start;
		JavaScriptRegionEnd end;
		
		public JavaScriptRegion(JavaScriptRegionStart start, JavaScriptRegionEnd end)
		{
			this.start = start;
			this.end = end;
		}
		
		public void AddRegion(IList<FoldingRegion> foldingRegions)
		{
			FoldingRegion namedFoldingRegion = CreateFoldingRegion();
			foldingRegions.Add(namedFoldingRegion);
		}
		
		FoldingRegion CreateFoldingRegion()
		{
			DomRegion location = GetRegionLocation();
			return new FoldingRegion(start.Name, location);
		}
		
		DomRegion GetRegionLocation()
		{
			int beginLine = start.Line;
			int endLine = end.Line;
			int beginColumn = start.StartColumn;
			int endColumn = end.EndColumn;
			return new DomRegion(beginLine, beginColumn, endLine, endColumn);
		}
	}
}
