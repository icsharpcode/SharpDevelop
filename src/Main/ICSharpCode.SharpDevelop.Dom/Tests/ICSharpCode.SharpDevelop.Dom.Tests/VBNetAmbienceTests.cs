// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom.VBNet;
using System;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	[TestFixture]
	public class VBNetAmbienceTests
	{
		VBNetAmbience fullMemberNameAmbience;
		IClass valueCollection;
		
		[TestFixtureSetUpAttribute]
		public void FixtureSetUp()
		{
			valueCollection = SharedProjectContentRegistryForTests.Instance.Mscorlib.GetClass("System.Collections.Generic.Dictionary.ValueCollection", 2);
			Assert.AreEqual(2, valueCollection.TypeParameters.Count);
			Assert.AreEqual(2, valueCollection.DeclaringType.TypeParameters.Count);
			
			fullMemberNameAmbience = new VBNetAmbience();
			fullMemberNameAmbience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.UseFullyQualifiedMemberNames;
		}
		
		[TestAttribute]
		public void TestFullClassNameOfClassInsideGenericClass()
		{
			Assert.AreEqual("Public NotInheritable Class System.Collections.Generic.Dictionary(Of TKey, TValue).ValueCollection", fullMemberNameAmbience.Convert(valueCollection));
		}
		
		[TestAttribute]
		public void TestFullNameOfValueCollectionCountProperty()
		{
			IProperty count = valueCollection.Properties.Single(p => p.Name == "Count");
			Assert.AreEqual("Public NotOverridable ReadOnly Property System.Collections.Generic.Dictionary(Of TKey, TValue).ValueCollection.Count As Integer", fullMemberNameAmbience.Convert(count));
		}
		
		[TestAttribute]
		public void TestFullNameOfValueCollectionCopyToMethod()
		{
			IMethod copyTo = valueCollection.Methods.Single(m => m.Name == "CopyTo");
			Assert.AreEqual("Public NotOverridable Sub System.Collections.Generic.Dictionary(Of TKey, TValue).ValueCollection.CopyTo(array As TValue(), index As Integer)", fullMemberNameAmbience.Convert(copyTo));
		}
	}
}
