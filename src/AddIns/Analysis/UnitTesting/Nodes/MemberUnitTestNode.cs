// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	public class MemberUnitTestNode : UnitTestBaseNode
	{
		TestMember testMember;
		
		public TestMember TestMember {
			get { return testMember; }
		}
		
		public MemberUnitTestNode(TestMember testMember)
		{
			this.testMember = testMember;
			this.testMember.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) {
				if (e.PropertyName == "TestResult") {
					SharpTreeNode p = this;
					while (p != null) {
						p.RaisePropertyChanged("Icon");
						p = p.Parent;
					}
				}
			};
		}
		
		internal override TestResultType TestResultType {
			get { return this.testMember.TestResult; }
		}
		
		public override object Text {
			get { return testMember.Method.Name; }
		}
	}
}
