// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public object BuildItem(BuildItemArgs args)
		{
			return BuildItem(args.Codon, new TestFrameworkFactory(args.AddIn));
		}
		
		public TestFrameworkDescriptor BuildItem(Codon codon, ITestFrameworkFactory factory)
		{
			return new TestFrameworkDescriptor(codon.Properties, factory);
		}
	}
}
