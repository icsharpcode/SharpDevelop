// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.UnitTesting
{
	public class TestFrameworkDoozer : IDoozer
	{
		public TestFrameworkDoozer()
		{
		}
		
		public bool HandleConditions {
			get { return false; }
		}
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return BuildItem(codon, new TestFrameworkFactory(codon.AddIn));
		}
		
		public TestFrameworkDescriptor BuildItem(Codon codon, ITestFrameworkFactory factory)
		{
			return new TestFrameworkDescriptor(codon.Properties, factory);
		}
	}
}
