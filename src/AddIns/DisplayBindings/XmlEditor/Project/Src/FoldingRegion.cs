// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Description of FoldingRegion.
	/// </summary>
	public class FoldingRegion
	{
		DomRegion region;
		string name;
		
		public DomRegion Region {
			get { return region; }
			set { region = value; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public FoldingRegion(string name, DomRegion region)
		{
			this.region = region;
			this.name = name;
		}
	}
}
