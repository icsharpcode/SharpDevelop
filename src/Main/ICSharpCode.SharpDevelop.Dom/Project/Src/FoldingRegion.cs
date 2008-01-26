// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public sealed class FoldingRegion : Immutable
	{
		string  name;
		DomRegion region;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public DomRegion Region {
			get {
				return region;
			}
		}
		
		public FoldingRegion(string name, DomRegion region)
		{
			this.name = name;
			this.region = region;
		}
	}
}
