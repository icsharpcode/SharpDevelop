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

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.Project
{
	[TestFixture]
	public class NameValuePairCollectionTests
	{
		[Test]
		public void SingleNameValuePair()
		{
			NameValuePairCollection nameValuePairs = new NameValuePairCollection("a=b");
			NameValuePair pair = nameValuePairs[0];
			Assert.AreEqual(1, nameValuePairs.Count);
			Assert.AreEqual("b", nameValuePairs.GetValue("a"));
			Assert.AreEqual("a", pair.Name);
			Assert.AreEqual("b", pair.Value);
		}
		
		[Test]
		public void EmptyString()
		{
			NameValuePairCollection nameValuePairs = new NameValuePairCollection(String.Empty);
			Assert.AreEqual(0, nameValuePairs.Count);
		}
		
		[Test]
		public void SingleNameValuePairWithSpaces()
		{
			NameValuePairCollection nameValuePairs = new NameValuePairCollection(" a = b ");
			NameValuePair pair = nameValuePairs[0];
			Assert.AreEqual(1, nameValuePairs.Count);
			Assert.AreEqual("b", nameValuePairs.GetValue("a"));
			Assert.AreEqual("a", pair.Name);
			Assert.AreEqual("b", pair.Value);
		}
		
		[Test]
		public void TwoNameValuePairs()
		{
			NameValuePairCollection nameValuePairs = new NameValuePairCollection("a=1;b=2");
			NameValuePair pair1 = nameValuePairs[0];
			NameValuePair pair2 = nameValuePairs[1];
			Assert.AreEqual(2, nameValuePairs.Count);
			Assert.AreEqual("1", nameValuePairs.GetValue("a"));
			Assert.AreEqual("a", pair1.Name);
			Assert.AreEqual("1", pair1.Value);
			Assert.AreEqual("a=1", pair1.ToString());
			Assert.AreEqual("2", nameValuePairs.GetValue("b"));
			Assert.AreEqual("b", pair2.Name);
			Assert.AreEqual("2", pair2.Value);
			Assert.AreEqual("b=2", pair2.ToString());
			Assert.AreEqual("a=1;b=2", nameValuePairs.GetList());
		}
		
		[Test]
		public void NoEqualSign()
		{
			NameValuePairCollection nameValuePairs = new NameValuePairCollection("a");
			NameValuePair pair = nameValuePairs[0];
			Assert.AreEqual(1, nameValuePairs.Count);
			Assert.AreEqual(String.Empty, nameValuePairs.GetValue("a"));
			Assert.AreEqual("a=", pair.ToString());
		}
	}
}
