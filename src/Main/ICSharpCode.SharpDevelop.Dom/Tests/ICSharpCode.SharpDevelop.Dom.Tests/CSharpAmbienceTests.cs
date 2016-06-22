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
