// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	public class MemberUnitTestNode : UnitTestNode
	{
		TestMember testMember;
		
		public TestMember TestMember {
			get { return testMember; }
		}
		
		public MemberUnitTestNode(TestMember testMember)
		{
			this.testMember = testMember;
			this.testMember.TestResultChanged += delegate {
				RaisePropertyChanged("Icon");
				RaisePropertyChanged("ExpandedIcon");
			};
		}
		
		internal override TestResultType TestResultType {
			get { return this.testMember.TestResult; }
		}
		
		public override object Text {
			get { return testMember.Member.Name; }
		}
		
		public override void ActivateItem(System.Windows.RoutedEventArgs e)
		{
			var region = testMember.Member.Region;
			SD.FileService.JumpToFilePosition(new FileName(region.FileName), region.BeginLine, region.BeginColumn);
		}
	}
}
