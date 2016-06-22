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
using ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.ServiceReferences
{
	[TestFixture]
	public class ServiceReferenceGeneratorOptionsTests
	{
		ServiceReferenceGeneratorOptions options;
		
		void CreateOptions()
		{
			options = new ServiceReferenceGeneratorOptions();
		}
		
		[Test]
		public void GetArrayCollectionTypeDescription_List_ReturnsFullyQualifiedListName()
		{
			CreateOptions();
			options.ArrayCollectionType = CollectionTypes.List;
			
			string text = options.GetArrayCollectionTypeDescription();
			
			Assert.AreEqual("System.Collections.Generic.List`1", text);
		}
		
		[Test]
		public void GetArrayCollectionTypeDescription_ArrayList_ReturnsFullyQualifiedArrayListName()
		{
			CreateOptions();
			options.ArrayCollectionType = CollectionTypes.ArrayList;
			
			string text = options.GetArrayCollectionTypeDescription();
			
			Assert.AreEqual("System.Collections.ArrayList", text);
		}
		
		[Test]
		public void GetDictionaryCollectionTypeDescription_HashTable_ReturnsFullyQualifiedHashTableName()
		{
			CreateOptions();
			options.DictionaryCollectionType = DictionaryCollectionTypes.HashTable;
			
			string text = options.GetDictionaryCollectionTypeDescription();
			
			Assert.AreEqual("System.Collections.Hashtable", text);
		}
		
		[Test]
		public void GetDictionaryCollectionTypeDescription_Dictionary_ReturnsFullyQualifiedDictionaryTypeName()
		{
			CreateOptions();
			options.DictionaryCollectionType = DictionaryCollectionTypes.Dictionary;
			
			string text = options.GetDictionaryCollectionTypeDescription();
			
			Assert.AreEqual("System.Collections.Generic.Dictionary`2", text);
		}
		
		[Test]
		public void GetDictionaryCollectionTypeDescription_GenericSortedList_ReturnsFullyQualifiedGenericSortedListName()
		{
			CreateOptions();
			options.DictionaryCollectionType = DictionaryCollectionTypes.SortedList;
			
			string text = options.GetDictionaryCollectionTypeDescription();
			
			Assert.AreEqual("System.Collections.Generic.SortedList`2", text);
		}
	}
}
