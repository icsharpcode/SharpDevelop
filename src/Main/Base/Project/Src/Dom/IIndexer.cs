// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IIndexer: IMember
	{
		IRegion BodyRegion {
			get;
		}
		
		IRegion GetterRegion {
			get;
		}

		IRegion SetterRegion {
			get;
		}

		List<IParameter> Parameters {
			get;
		}
	}
}
