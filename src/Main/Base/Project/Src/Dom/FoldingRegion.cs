// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class FoldingRegion
	{
		string  name;
		IRegion region;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public IRegion Region {
			get {
				return region;
			}
		}
		
		public FoldingRegion(string name, IRegion region)
		{
			this.name = name;
			this.region = region;
		}
	}
}
