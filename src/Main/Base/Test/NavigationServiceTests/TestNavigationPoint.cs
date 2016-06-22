// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
