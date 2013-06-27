// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a namespace of unit tests.
	/// </summary>
	public class TestNamespace : TestBase
	{
		readonly ITestProject project;
		readonly string namespaceName;
		readonly string displayName;
		
		public TestNamespace(ITestProject project, string namespaceName, string displayName = null)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (namespaceName == null)
				throw new ArgumentNullException("namespaceName");
			this.project = project;
			this.namespaceName = namespaceName;
			if (displayName != null) {
				this.displayName = displayName;
			} else {
				this.displayName = namespaceName.Substring(namespaceName.LastIndexOf('.') + 1);
			}
			BindResultToCompositeResultOfNestedTests();
		}
		
		public override ITestProject ParentProject {
			get { return project; }
		}
		
		public string NamespaceName {
			get { return namespaceName; }
		}
		
		public override string DisplayName {
			get { return displayName; }
		}
		
		/// <summary>
		/// Change return type of NestedTests to TestCollection so that tests can be added externally.
		/// </summary>
		public new TestCollection NestedTests {
			get { return base.NestedTestCollection; }
		}
		
	}
}
