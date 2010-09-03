// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
