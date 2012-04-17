// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceGeneratorOptions
	{
		List<string> assemblies = new List<string>();
		
		public ServiceReferenceGeneratorOptions()
			: this(new string[0])
		{
		}
		
		public ServiceReferenceGeneratorOptions(IEnumerable<string> assemblies)
		{
			this.assemblies.AddRange(assemblies);
			this.AppConfigFileName = String.Empty;
			this.MergeAppConfig = false;
			this.OutputFileName = String.Empty;
			this.ServiceName = String.Empty;
			this.Language = "CS";
			this.NoAppConfig = true;
			this.UseTypesInProjectReferences = true;
			this.ArrayCollectionType = CollectionTypes.Array;
			this.DictionaryCollectionType = DictionaryCollectionTypes.Dictionary;
		}
		
		public string ServiceName { get; set; }
		public string Namespace { get; set; }
		public string OutputFileName { get; set; }
		public string Url { get; set; }
		public string Language { get; set; }
		public string AppConfigFileName { get; set; }
		public bool NoAppConfig { get; set; }
		public bool MergeAppConfig { get; set; }
		public bool GenerateInternalClasses { get; set; }
		public bool GenerateAsyncOperations { get; set; }
		public bool GenerateMessageContract { get; set; }
		public bool UseTypesInProjectReferences { get; set; }
		public bool UseTypesInSpecifiedAssemblies { get; set; }
		public CollectionTypes ArrayCollectionType { get; set; }
		public DictionaryCollectionTypes DictionaryCollectionType { get; set; }

		public void MapProjectLanguage(string language)
		{
			if (language == "VBNet") {
				Language = "VB";
			} else {
				Language = "CS";
			}
		}
		
		public IList<string> Assemblies {
			get { return assemblies; }
		}
		
		public ServiceReferenceGeneratorOptions Clone()
		{
			return new ServiceReferenceGeneratorOptions(this.assemblies) {
				ServiceName = this.ServiceName,
				OutputFileName = this.OutputFileName,
				Url = this.Url,
				Language = this.Language,
				AppConfigFileName = this.AppConfigFileName,
				NoAppConfig = this.NoAppConfig,
				MergeAppConfig = this.MergeAppConfig,
				UseTypesInSpecifiedAssemblies = this.UseTypesInSpecifiedAssemblies,
				UseTypesInProjectReferences = this.UseTypesInProjectReferences,
				ArrayCollectionType = this.ArrayCollectionType,
				DictionaryCollectionType = this.DictionaryCollectionType,
				GenerateInternalClasses = this.GenerateInternalClasses,
				GenerateAsyncOperations = this.GenerateAsyncOperations,
				GenerateMessageContract = this.GenerateMessageContract
			};
		}
		
		public string GetNamespaceMapping()
		{
			if (Namespace != null) {
				return "*," + Namespace;
			}
			return null;
		}
		
		public string GetArrayCollectionTypeDescription()
		{
			string description = GetEnumTypeDescription(ArrayCollectionType.GetType(), ArrayCollectionType.ToString());
			return description + GetGenericTypeSuffix(ArrayCollectionType);
		}
		
		string GetEnumTypeDescription(Type type, string name)
		{
			foreach (FieldInfo field in type.GetFields()) {
				if (field.IsStatic) {
					if (field.Name == name) {
						return GetDescription(field);
					}
				}
			}
			return null;
		}
		
		string GetDescription(FieldInfo field)
		{
			foreach (DescriptionAttribute attribute in field.GetCustomAttributes(typeof(DescriptionAttribute), false))
				return attribute.Description;
			return field.Name;
		}
		
		string GetGenericTypeSuffix(CollectionTypes type)
		{
			switch (type) {
				case CollectionTypes.List:
				case CollectionTypes.LinkedList:
				case CollectionTypes.ObservableCollection:
				case CollectionTypes.Collection:
				case CollectionTypes.BindingList:
					return "`1";
			}
			return String.Empty;
		}
		
		public string GetDictionaryCollectionTypeDescription()
		{
			string description = GetEnumTypeDescription(DictionaryCollectionType.GetType(), DictionaryCollectionType.ToString());
			return description + GetGenericTypeSuffix(DictionaryCollectionType);
		}
		
		string GetGenericTypeSuffix(DictionaryCollectionTypes type)
		{
			switch (type) {
				case DictionaryCollectionTypes.SortedList_2:
				case DictionaryCollectionTypes.HashTable:
				case DictionaryCollectionTypes.HybridDictionary:
				case DictionaryCollectionTypes.ListDictionary:
				case DictionaryCollectionTypes.OrderedDictionary:
					return String.Empty;
			}
			return "`2";
		}
		
		public void AddProjectReferencesIfUsingTypesFromProjectReferences(IEnumerable<ReferenceProjectItem> assemblies)
		{
			if (UseTypesInProjectReferences) {
				foreach (ReferenceProjectItem item in assemblies) {
					Assemblies.Add(item.FileName);
				}
			}
		}
		
		public void GenerateNamespace(string rootNamespace)
		{
			if (String.IsNullOrEmpty(rootNamespace)) {
				Namespace = ServiceName;
			} else {
				Namespace = rootNamespace + "." + ServiceName;
			}
		}
	}
}
