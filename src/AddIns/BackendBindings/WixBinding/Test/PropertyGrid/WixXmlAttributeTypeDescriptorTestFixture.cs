// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
