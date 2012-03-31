// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
