// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.SharpDevelop.Tests.NavigationServiceTests
{
	public class TestNavigationPoint : DefaultNavigationPoint
	{
		public TestNavigationPoint(string filename) : this(filename, 0) {}
		public TestNavigationPoint(string filename, int line): base(filename)
		{
			this.LineNumber = line;
		}
		
		public new int NavigationData {
			get {
				return (int)base.NavigationData;
			}
			set {
				base.NavigationData = value;
			}
		}
		public int LineNumber {
			get {
				return this.NavigationData;
			}
			set {
				this.NavigationData = value;
			}
		}
		
		#region IComparable
		public override bool Equals(object obj)
		{
			TestNavigationPoint b = obj as TestNavigationPoint;
			if (b == null) return false;
			return this.FileName == b.FileName
				&& Math.Abs(this.LineNumber - b.LineNumber) <= 5;
		}
		
		public override int GetHashCode()
		{
			return this.FileName.GetHashCode() ^ this.LineNumber.GetHashCode();
		}
		#endregion
		
		public override void JumpTo()
		{
			// simulate the case where jumping triggers a call to log an intermediate position
			NavigationService.Log(new TestNavigationPoint(this.FileName, -500));

			// simulate changing something outside the NavigationService's model
			CurrentTestPosition = this;
		}

		public static INavigationPoint CurrentTestPosition = null;
	}
}
