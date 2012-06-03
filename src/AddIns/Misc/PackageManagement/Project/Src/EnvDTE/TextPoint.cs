// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class TextPoint : MarshalByRefObject
	{
		public TextPoint()
		{
		}
		
		public int LineCharOffset { get; private set; }
		
		public EditPoint CreateEditPoint()
		{
			throw new NotImplementedException();
		}
		
		internal static TextPoint CreateStartPoint(DomRegion region)
		{
			return new TextPoint { LineCharOffset = region.BeginColumn };
		}
		
		internal static TextPoint CreateEndPoint(DomRegion region)
		{
			return new TextPoint { LineCharOffset = region.EndColumn };
		}
	}
}
