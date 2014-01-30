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
using System.ComponentModel;

namespace WixBinding.Tests.PropertyGrid
{
	[TestFixture]
	public class WixXmlAttributePropertyDescriptorTestFixture
	{
		WixXmlAttributePropertyDescriptor pd;
		WixXmlAttribute wixXmlAttribute;
		
		[SetUp]
		public void Setup()
		{
			wixXmlAttribute = new WixXmlAttribute("LongName", "InitialValue", WixXmlAttributeType.Text);
			pd = new WixXmlAttributePropertyDescriptor(wixXmlAttribute);
		}
	
		[Test]
		public void Name()
		{
			Assert.AreEqual("LongName", pd.Name);
		}
		
		[Test]
		public void WixXmlAttributeSet()
		{
			Assert.IsTrue(Object.ReferenceEquals(wixXmlAttribute, pd.WixXmlAttribute));
		}
		
		[Test]
		public void NoAttributes()
		{
			Assert.AreEqual(0, pd.Attributes.Count);
		}
		
		[Test]
		public void TypeOfString()
		{
			Assert.IsTrue(pd.PropertyType == typeof(String));
		}
		
		[Test]
		public void IsNotReadOnly()
		{
			Assert.IsFalse(pd.IsReadOnly);
		}
		
		[Test]
		public void ComponentType()
		{
			Assert.IsTrue(typeof(WixXmlAttribute) == pd.ComponentType);
		}

		[Test]
		public void ShouldSerializeValue()
		{
			Assert.IsTrue(pd.ShouldSerializeValue(wixXmlAttribute));
		}
		
		[Test]
		public void CanResetValue()
		{
			Assert.IsFalse(pd.CanResetValue(wixXmlAttribute));
		}
		
		[Test]
		public void GetValue()
		{
			Assert.AreEqual("InitialValue", (string)pd.GetValue(wixXmlAttribute));
		}
		
		[Test]
		public void SetValue()
		{
			pd.SetValue(wixXmlAttribute, "NewValue");
			Assert.AreEqual("NewValue", (string)pd.GetValue(wixXmlAttribute));			
		}
		
		[Test]
		public void SetNullValue()
		{
			pd.SetValue(wixXmlAttribute, null);
			Assert.AreEqual(String.Empty, (string)pd.GetValue(wixXmlAttribute));
		}
	}
}
