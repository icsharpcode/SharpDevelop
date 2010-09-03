// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom.CSharp;
using System;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	[TestFixture]
	public class CSharpAmbienceTests
	{
		CSharpAmbience fullMemberNameAmbience;
		IClass valueCollection;
		
		[TestFixtureSetUpAttribute]
		public void FixtureSetUp()
		{
			valueCollection = SharedProjectContentRegistryForTests.Instance.Mscorlib.GetClass("System.Collections.Generic.Dictionary.ValueCollection", 2);
			Assert.AreEqual(2, valueCollection.TypeParameters.Count);
			Assert.AreEqual(2, valueCollection.DeclaringType.TypeParameters.Count);
			
			fullMemberNameAmbience = new CSharpAmbience();
			fullMemberNameAmbience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.UseFullyQualifiedMemberNames;
		}
		
		[TestAttribute]
		public void TestFullClassNameOfClassInsideGenericClass()
		{
			Assert.AreEqual("public sealed class System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection", fullMemberNameAmbience.Convert(valueCollection));
		}
		
		[TestAttribute]
		public void TestFullNameOfValueCollectionCountProperty()
		{
			IProperty count = valueCollection.Properties.Single(p => p.Name == "Count");
			Assert.AreEqual("public sealed int System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection.Count", fullMemberNameAmbience.Convert(count));
		}
		
		[TestAttribute]
		public void TestFullNameOfValueCollectionCopyToMethod()
		{
			IMethod copyTo = valueCollection.Methods.Single(m => m.Name == "CopyTo");
			Assert.AreEqual("public sealed void System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection.CopyTo(TValue[] array, int index)", fullMemberNameAmbience.Convert(copyTo));
		}
	}
}
