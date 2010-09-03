// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
