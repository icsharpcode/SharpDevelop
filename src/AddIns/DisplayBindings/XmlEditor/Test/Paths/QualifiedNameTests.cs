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
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class QualifiedNameTests
	{
		[Test]
		public void GetPrefixedNameWithNoPrefixReturnsName()
		{
			QualifiedName name = new QualifiedName("root", "namespace");
			Assert.AreEqual("root", name.GetPrefixedName());
		}
		
		[Test]
		public void GetPrefixedNameWithPrefixReturnsPrefixAndName()
		{
			QualifiedName name = new QualifiedName("root", "namespace", "a");
			Assert.AreEqual("a:root", name.GetPrefixedName());
		}
		
		[Test]
		public void EmptyQualifiedNameGetPrefixedNameReturnsEmptyString()
		{
			QualifiedName name = new QualifiedName();
			Assert.AreEqual(String.Empty, name.GetPrefixedName());
		}
		
		[Test]
		public void HasNamespaceReturnsTrueForNonEmptyNamespace()
		{
			QualifiedName name = new QualifiedName("root", "namespace");
			Assert.IsTrue(name.HasNamespace);
		}
		
		[Test]
		public void IsEmptyReturnsTrueForEmptyNameAndNamespaceAndPrefix()
		{
			QualifiedName name = new QualifiedName();
			Assert.IsTrue(name.IsEmpty);
		}
		
		[Test]
		public void IsEmptyReturnsFalseForNonEmptyName()
		{
			QualifiedName name = new QualifiedName("root", String.Empty);
			Assert.IsFalse(name.IsEmpty);
		}
		
		[Test]
		public void IsEmptyReturnsFalseForNonEmptyNamespace()
		{
			QualifiedName name = new QualifiedName(String.Empty, "ns");
			Assert.IsFalse(name.IsEmpty);
		}
		
		[Test]
		public void IsEmptyReturnsFalseForNonEmptyNamespacePrefix()
		{
			QualifiedName name = new QualifiedName(String.Empty, String.Empty, "a");
			Assert.IsFalse(name.IsEmpty);
		}
		
		[Test]
		public void EmptyQualifiedNameReturnsEmptyStringForName()
		{
			QualifiedName name = new QualifiedName();
			Assert.AreEqual(String.Empty, name.Name);
		}
		
		[Test]
		public void EmptyQualifiedNameReturnsEmptyStringForNamespace()
		{
			QualifiedName name = new QualifiedName();
			Assert.AreEqual(String.Empty, name.Namespace);
		}		
		
		[Test]
		public void EmptyQualifiedNameReturnsEmptyStringForNamespacePrefix()
		{
			QualifiedName name = new QualifiedName();
			Assert.AreEqual(String.Empty, name.Prefix);
		}

		[Test]
		public void NamePropertyReturnsCorrectName()
		{
			QualifiedName name = new QualifiedName("a", "ns");
			Assert.AreEqual("a", name.Name);
		}
		
		[Test]
		public void NamespacePropertyReturnsCorrectNamespace()
		{
			QualifiedName name = new QualifiedName("a", "ns");
			Assert.AreEqual("ns", name.Namespace);
		}

		[Test]
		public void PrefixPropertyReturnsCorrectNamespace()
		{
			QualifiedName name = new QualifiedName("a", "ns", "prefix");
			Assert.AreEqual("prefix", name.Prefix);
		}
		
		[Test]
		public void HasPrefixReturnsTrueForNonEmptyPrefix()
		{
			QualifiedName name = new QualifiedName("a", "ns", "prefix");
			Assert.IsTrue(name.HasPrefix);
		}
		
		[Test]
		public void HasPrefixReturnsFalseForEmptyPrefix()
		{
			QualifiedName name = new QualifiedName("a", "ns", String.Empty);
			Assert.IsFalse(name.HasPrefix);
		}
		
		[Test]
		public void QualifiedNameChanged()
		{
			QualifiedName name = new QualifiedName("root", "ns", "a");
			name.Name = "element";
			Assert.AreEqual("a:element [ns]", name.ToString());
		}
		
		[Test]
		public void QualifiedNameNamespaceChanged()
		{
			QualifiedName name = new QualifiedName("root", "ns", "a");
			name.Namespace = "new-ns";
			Assert.AreEqual("a:root [new-ns]", name.ToString());
		}
		
		[Test]
		public void QualifiedNamePrefixChanged()
		{
			QualifiedName name = new QualifiedName("root", "ns", "a");
			name.Prefix = "newprefix";
			Assert.AreEqual("newprefix:root [ns]", name.ToString());
		}
		
		[Test]
		public void CreateQualifiedNameFromString()
		{
			QualifiedName name = QualifiedName.FromString("root");
			QualifiedName expectedQualifiedName = new QualifiedName("root", String.Empty);
			
			Assert.AreEqual(expectedQualifiedName, name);
		}
		
		[Test]
		public void CreateQualifiedNameFromStringContainingPrefix()
		{
			QualifiedName name = QualifiedName.FromString("a:root");
			QualifiedName expectedQualifiedName = new QualifiedName("root", String.Empty, "a");
			Assert.AreEqual(expectedQualifiedName, name);
		}
		
		[Test]
		public void CreateQualifiedNameFromNullString()
		{
			QualifiedName name = QualifiedName.FromString(null);
			Assert.IsTrue(name.IsEmpty);
		}
	}
}
