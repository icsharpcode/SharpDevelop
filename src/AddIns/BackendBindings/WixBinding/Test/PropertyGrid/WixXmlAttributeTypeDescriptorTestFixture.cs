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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PropertyGrid
{
	[TestFixture]
	public class WixXmlAttributeTypeDescriptorTestFixture
	{
		ReadOnlyCollection<WixXmlAttribute> wixXmlAttributes;
		WixXmlAttributeTypeDescriptor descriptor;
		
		[SetUp]
		public void SetUp()
		{
			List<WixXmlAttribute> attributes = new List<WixXmlAttribute>();
			attributes.Add(new WixXmlAttribute("First", "FirstValue", WixXmlAttributeType.Text));
			attributes.Add(new WixXmlAttribute("Second", "SecondValue", WixXmlAttributeType.Text));
			wixXmlAttributes = new ReadOnlyCollection<WixXmlAttribute>(attributes);
			descriptor = new WixXmlAttributeTypeDescriptor(wixXmlAttributes);
		}
				
		[Test]
		public void PropertyOwner()
		{
			Assert.AreSame(descriptor, descriptor.GetPropertyOwner(null));
		}
		
		[Test]
		public void HasTwoProperties()
		{
			Assert.AreEqual(2, descriptor.GetProperties().Count);
		}
		
		[Test]
		public void FirstProperty()
		{
			WixXmlAttributePropertyDescriptor pd = (WixXmlAttributePropertyDescriptor)descriptor.GetProperties()["First"];
			Assert.AreEqual("FirstValue", (string)pd.GetValue(null));
		}
		
		[Test]
		public void SecondProperty()
		{
			WixXmlAttributePropertyDescriptor pd = (WixXmlAttributePropertyDescriptor)descriptor.GetProperties()["Second"];
			Assert.AreEqual("SecondValue", (string)pd.GetValue(null));
		}
	}
}
