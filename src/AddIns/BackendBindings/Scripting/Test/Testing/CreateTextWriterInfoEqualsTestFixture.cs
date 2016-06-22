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
using System.Text;
using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Testing
{
	[TestFixture]
	public class CreateTextWriterInfoEqualsTestFixture
	{
		[Test]
		public void CreateTextWriterInfosAreEqualWhenFileNameAndEncodingAndAppendAreEqual()
		{
			CreateTextWriterInfo lhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			CreateTextWriterInfo rhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			Assert.AreEqual(lhs, rhs);
		}
		
		[Test]
		public void CreateTextWriterInfosAreNotEqualWhenFileNamesAreDifferent()
		{
			CreateTextWriterInfo lhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			CreateTextWriterInfo rhs = new CreateTextWriterInfo("different-filename.txt", Encoding.UTF8, true);
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void CreateTextWriterInfosAreNotEqualWhenEncodingsAreDifferent()
		{
			CreateTextWriterInfo lhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			CreateTextWriterInfo rhs = new CreateTextWriterInfo("test.txt", Encoding.ASCII, true);
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void CreateTextWriterInfosAreNotEqualWhenAppendIsDifferent()
		{
			CreateTextWriterInfo lhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			CreateTextWriterInfo rhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, false);
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void CreateTextWriterInfoEqualsReturnsFalseWhenNullPassedAsParameter()
		{
			CreateTextWriterInfo lhs = new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			Assert.IsFalse(lhs.Equals(null));
		}
	}
}
