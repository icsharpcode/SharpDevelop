/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.11.2011
 * Time: 19:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog
{
	/// <summary>
	/// Description of AdvancedServiceViewModel.
	/// </summary>
	/// 
	public enum Modifyers
	{
		//[Description("${res:Dialog.ProjectOptions.RunPostBuildEvent.Always}")]
		Public,
		//[Description("${res:Dialog.ProjectOptions.RunPostBuildEvent.OnOutputUpdated}")]
		Internal
	}
	
	public enum CollectionTypes
	{
		[Description("System.Array")]
		Array,
		[Description("System.Collections.ArrayList")]
		ArrayList,
		[Description("System.Collections.Generic.LinkedList")]
		LinkedList,
		[Description("System.Collections.Generic.List")]
		List,
		[Description("System.Collections.ObjectModel.Collection")]
		Collection,
		[Description("System.Collections.ObjectModel.ObservableCollection")]
		ObservableCollection,
		[Description("System.ComponentModel.BindingList")]
		BindingList
	}
	
	public enum DictionaryCollectionTypes
	{
		Dictionary,
		SortedList,
		SortedDictionary,
		HashTable,
		KeyedCollection,
		SortedList_2,
		HybridDictionary,
		ListDictionary,
		OrderedDictionary
	}
	
	
	internal class AdvancedServiceViewModel:ViewModelBase
	{
		private string compatibilityText ="Add a web Reference instead of a Service Reference. ";
		private string c_2 ="thios will generate code base on .NET Framework 2.0  Web services technology";
		
		public AdvancedServiceViewModel()
		{
			Title ="Service Reference Settings";
			UseReferencedAssemblies = true;
			AssembliesToReference = new ObservableCollection <string>();
			AssembliesToReference.Add("Microsoft.CSharp");
			AssembliesToReference.Add("mscorlib");
			AssembliesToReference.Add("System.Core");
			AssembliesToReference.Add("System.Data");
			AssembliesToReference.Add("System.Data.DataSetExtensions");
			AssembliesToReference.Add("System.Runtime.Serialization");
			AssembliesToReference.Add("System.ServiceModel");
			AssembliesToReference.Add("System.Xml");
			AssembliesToReference.Add("System.Xml.Linq");
		}
		
		public string Title {get;set;}
		
		private Modifyers selectedModifyer;
		
		public Modifyers SelectedModifyer {
			get { return selectedModifyer; }
			set { selectedModifyer = value;
				base.RaisePropertyChanged(() =>SelectedModifyer);			}
		}
		
		bool generateAsyncOperations;
		
		public bool GenerateAsyncOperations {
			get { return generateAsyncOperations; }
			set { generateAsyncOperations = value;
				base.RaisePropertyChanged(() =>GenerateAsyncOperations);}
		}
		
		
		bool generateMessageContract;
		
		public bool GenerateMessageContract {
			get { return generateMessageContract; }
			set { generateMessageContract = value;
			base.RaisePropertyChanged(() =>GenerateMessageContract);}
		}
		
		
		private CollectionTypes collectionType;
		
		public CollectionTypes CollectionType {
			get { return collectionType; }
			set { collectionType = value;
			base.RaisePropertyChanged(() =>CollectionType);}
		}
		
		private DictionaryCollectionTypes dictionaryCollectionType;
		
		public DictionaryCollectionTypes DictionaryCollectionType {
			get { return dictionaryCollectionType; }
			set { dictionaryCollectionType = value;
			base.RaisePropertyChanged(() =>DictionaryCollectionType);}
		}
		
		
		private bool useReferencedAssemblies;
		
		public bool UseReferencedAssemblies {
			get { return useReferencedAssemblies; }
			set { useReferencedAssemblies = value;
				if (useReferencedAssemblies) {
					ReuseTypes = true;
				}
				else {
					ReuseTypes = false;
				}
			base.RaisePropertyChanged(() =>UseReferencedAssemblies);}
		}
		
		
		private bool reuseTypes;
		
		public bool ReuseTypes {
			get { return reuseTypes; }
			set { reuseTypes = value;
				if (reuseTypes) {
					
				} 
				else
				{
					
				}
			base.RaisePropertyChanged(() =>ReuseTypes);}
		}
		
		
		private bool reuseReferencedTypes;
		
		public bool ReuseReferencedTypes {
			get { return reuseReferencedTypes; }
			set { reuseReferencedTypes = value;
				if (reuseReferencedTypes)
				{
					ListViewEnable = true;
					
				} else 
				{
					ListViewEnable = false;
				}
						
				base.RaisePropertyChanged(() => ReuseReferencedTypes);}
		}
		
		
		private bool listViewEnable;
		
		public bool ListViewEnable {
			get { return listViewEnable; }
			set { listViewEnable = value;
			base.RaisePropertyChanged(() => ListViewEnable);}
		}
		
		
		public ObservableCollection <string> AssembliesToReference {get;private set;}
		
		
		public string CompatibilityText 
		{
			get {
				return compatibilityText + c_2;
			}
		}
			
	}
}
