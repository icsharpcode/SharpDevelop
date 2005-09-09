// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
		
		IMethod GetterMethod {
			get;
		}

		IMethod SetterMethod {
			get;
		}
	}
}
