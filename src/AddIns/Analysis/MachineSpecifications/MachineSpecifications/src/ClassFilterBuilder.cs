// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	/// <summary>
	/// Creates class list filter for tests which should be run.
	/// </summary>
	public class ClassFilterBuilder
	{
		public IList<string> BuildFilterFor(IEnumerable<ITest> tests)
		{
			return GetFilter(tests).ToList();
		}
		
		IEnumerable<string> GetFilter(IEnumerable<ITest> tests)
		{
			foreach (ITest test in tests) {
				var testWithAssociatedType = test as ITestWithAssociatedType;
				var testNamespace = test as TestNamespace;
				if (testWithAssociatedType != null) {
					yield return testWithAssociatedType.GetTypeName();
				} else if (testNamespace != null) {
					foreach (string filter in GetFilter(testNamespace.NestedTests)) {
						yield return filter;
					}
				}
			}
		}
	}
}
