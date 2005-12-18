// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IProperty : IMethodOrProperty
	{
		DomRegion GetterRegion {
			get;
		}

		DomRegion SetterRegion {
			get;
		}

		bool CanGet {
			get;
		}

		bool CanSet {
			get;
		}
		
		bool IsIndexer {
			get;
		}
	}
}
