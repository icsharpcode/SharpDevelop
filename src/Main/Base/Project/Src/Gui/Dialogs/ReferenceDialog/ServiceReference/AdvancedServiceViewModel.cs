// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;

using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public enum Modifiers
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
	
	internal class AdvancedServiceViewModel : ViewModelBase
	{
		string compatibilityText = "Add a Web Reference instead of a Service Reference. ";
		string c_2 = "This will generate code based on .NET Framework 2.0 Web Services technology.";
		string accesslevel = "Access level for generated classes:";
		
		public AdvancedServiceViewModel()
		{
			Title ="Service Reference Settings";
			UseReferencedAssemblies = true;
			BitmapSource image = PresentationResourceService.GetBitmapSource("Icons.16x16.Reference");
			AssembliesToReference = new ObservableCollection<CheckableImageAndDescription>();
			AssembliesToReference.Add(new CheckableImageAndDescription(image, "Microsoft.CSharp"));
			AssembliesToReference.Add(new CheckableImageAndDescription(image, "mscorlib"));
			AssembliesToReference.Add(new CheckableImageAndDescription(image, "System.Core"));
			AssembliesToReference.Add(new CheckableImageAndDescription(image, "System.Data"));
			AssembliesToReference.Add(new CheckableImageAndDescription(image, "System.Data.DataSetExtensions"));
			AssembliesToReference.Add(new CheckableImageAndDescription(image, "System.Runtime.Serialization"));
			AssembliesToReference.Add(new CheckableImageAndDescription(image, "System.ServiceModel"));
			AssembliesToReference.Add(new CheckableImageAndDescription(image, "System.Xml"));
			AssembliesToReference.Add(new CheckableImageAndDescription(image, "System.Xml.Linq"));
		}
		
		public string Title { get; set; }
		
		public string AccessLevel {
			get { return accesslevel; }
		}
		
		Modifiers selectedModifier;
		
		public Modifiers SelectedModifier {
			get { return selectedModifier; }
			set {
				selectedModifier = value;
				OnPropertyChanged();
			}
		}
		
		bool generateAsyncOperations;
		
		public bool GenerateAsyncOperations {
			get { return generateAsyncOperations; }
			set {
				generateAsyncOperations = value;
				OnPropertyChanged();
			}
		}
		
		bool generateMessageContract;
		
		public bool GenerateMessageContract {
			get { return generateMessageContract; }
			set {
				generateMessageContract = value;
				OnPropertyChanged();
			}
		}
		
		CollectionTypes collectionType;
		
		public CollectionTypes CollectionType {
			get { return collectionType; }
			set {
				collectionType = value;
				OnPropertyChanged();
			}
		}
		
		DictionaryCollectionTypes dictionaryCollectionType;
		
		public DictionaryCollectionTypes DictionaryCollectionType {
			get { return dictionaryCollectionType; }
			set {
				dictionaryCollectionType = value;
				OnPropertyChanged();
			}
		}
		
		bool useReferencedAssemblies;
		
		public bool UseReferencedAssemblies {
			get { return useReferencedAssemblies; }
			set { 
				useReferencedAssemblies = value;
				ReuseTypes = useReferencedAssemblies;
				OnPropertyChanged();
			}
		}
		
		bool reuseTypes;
		
		public bool ReuseTypes {
			get { return reuseTypes; }
			set {
				reuseTypes = value;
				OnPropertyChanged();
			}
		}
		
		bool reuseReferencedTypes;
		
		public bool ReuseReferencedTypes {
			get { return reuseReferencedTypes; }
			set { 
				reuseReferencedTypes = value;
				ListViewEnable = value;
				OnPropertyChanged();
			}
		}
		
		bool listViewEnable;
		
		public bool ListViewEnable {
			get { return listViewEnable; }
			set {
				listViewEnable = value;
				OnPropertyChanged();
			}
		}
		
		public ObservableCollection <CheckableImageAndDescription> AssembliesToReference { get; private set; }
		
		public string CompatibilityText 
		{
			get { return compatibilityText + c_2; }
		}	
	}
}
