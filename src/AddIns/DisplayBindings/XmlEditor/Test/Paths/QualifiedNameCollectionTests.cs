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
	public class QualifiedNameCollectionTests
	{
		QualifiedName firstName;
		QualifiedName secondName;
		QualifiedNameCollection qualifiedNameCollection;
		
		[SetUp]
		public void Init()
		{
			firstName = new QualifiedName("first", "first-ns", "first-prefix");
			secondName = new QualifiedName("second", "second-ns", "second-prefix");	
			
			QualifiedName[] array = new QualifiedName[] { firstName, secondName };
			qualifiedNameCollection = new QualifiedNameCollection(array);
		}
		
		[Test]
		public void CreateQualifiedNameCollectionInstanceUsingQualifiedNameArray()
		{
			Assert.AreSame(secondName, qualifiedNameCollection[1]);
		}
		
		[Test]
		public void CreateQualifiedNameCollectionInstanceUsingQualifiedNameCollection()
		{
			QualifiedName[] array = new QualifiedName[] { firstName, secondName };
			QualifiedNameCollection oldCollection = new QualifiedNameCollection(array);
			QualifiedNameCollection newCollection = new QualifiedNameCollection(oldCollection);
			
			Assert.AreSame(firstName, newCollection[0]);
		}
		
		[Test]
		public void RemoveFirstItemFromCollectionRemovesFirstName()
		{
			qualifiedNameCollection.RemoveFirst();
			Assert.AreSame(secondName, qualifiedNameCollection[0]);
		}
		
		[Test]
		public void RemoveFirstItemFromEmptyCollectionDoesNotThrowArgumentOutOfRangeException()
		{
			qualifiedNameCollection = new QualifiedNameCollection();
			Assert.DoesNotThrow(delegate { qualifiedNameCollection.RemoveFirst(); });
		}
		
		[Test]
		public void RemoveLastItemFromCollectionRemovesLastName()
		{
			qualifiedNameCollection.RemoveLast();
			Assert.AreEqual(1, qualifiedNameCollection.Count);
		}
		
		[Test]
		public void RemoveLastItemFromCollectionLeavesFirstName()
		{
			RemoveLastItemFromCollectionRemovesLastName();
			Assert.AreSame(firstName, qualifiedNameCollection[0]);
		}
		
		[Test]
		public void RemoveLastItemFromEmptyCollectionDoesNotThrowArgumentOutOfRangeException()
		{
			qualifiedNameCollection = new QualifiedNameCollection();
			Assert.DoesNotThrow(delegate { qualifiedNameCollection.RemoveLast(); });
		}
		
		[Test]
		public void GetLastPrefixReturnsPrefixFromLastQualifiedNameInCollection()
		{
			Assert.AreEqual("second-prefix", qualifiedNameCollection.GetLastPrefix());
		}
		
		[Test]
		public void GetLastPrefixFromEmptyCollectionDoesNotThrowArgumentOutOfRangeException()
		{
			qualifiedNameCollection = new QualifiedNameCollection();
			Assert.AreEqual(String.Empty, qualifiedNameCollection.GetLastPrefix());
		}
		
		[Test]
		public void GetNamespaceForPrefixFindsCorrectNamespaceForFirstName()
		{
			Assert.AreEqual("first-ns", qualifiedNameCollection.GetNamespaceForPrefix("first-prefix"));
		}
		
		[Test]
		public void GetNamespaceForPrefixForUnknownPrefixReturnsEmptyString()
		{
			Assert.AreEqual(String.Empty, qualifiedNameCollection.GetNamespaceForPrefix("a"));
		}

		[Test]
		public void GetLastReturnsLastQualifiedNameInCollection()
		{
			Assert.AreSame(secondName, qualifiedNameCollection.GetLast());
		}
		
		[Test]
		public void GetLastReturnsNullWhenCollectionIsEmpty()
		{
			qualifiedNameCollection = new QualifiedNameCollection();
			Assert.IsNull(qualifiedNameCollection.GetLast());
		}
		
		[Test]
		public void QualifiedNameCollectionHasItems()
		{
			Assert.IsTrue(qualifiedNameCollection.HasItems);
		}
				
		[Test]
		public void EmptyQualifiedNameCollectionHasNoItems()
		{
			qualifiedNameCollection = new QualifiedNameCollection();
			Assert.IsFalse(qualifiedNameCollection.HasItems);
		}
		
		[Test]
		public void RemoveFirstTwoItemsClearsEntireCollection()
		{
			qualifiedNameCollection.RemoveFirst(2);
			Assert.AreEqual(0, qualifiedNameCollection.Count);
		}
		
		[Test]
		public void RemovingMoreItemsThanInCollectionDoesNotThrowArgumentOutOfRangeException()
		{
			Assert.DoesNotThrow(delegate { qualifiedNameCollection.RemoveFirst(5); });
			Assert.AreEqual(0, qualifiedNameCollection.Count);
		}
		
		[Test]
		public void NewQualifiedNameCollectionIsEmpty()
		{
			qualifiedNameCollection = new QualifiedNameCollection();
			Assert.IsTrue(qualifiedNameCollection.IsEmpty);
		}
		
		[Test]
		public void QualifiedNameCollectionWithItemsIsNotEmpty()
		{
			Assert.IsFalse(qualifiedNameCollection.IsEmpty);
		}
		
		[Test]
		public void QualifiedNameCollectionRootElementNamespaceIsCorrect()
		{
			Assert.AreEqual("first-ns", qualifiedNameCollection.GetRootNamespace());
		}
	}
}
