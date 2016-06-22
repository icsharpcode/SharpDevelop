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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SettingsEditor
{
	/// <summary>
	/// Interaction logic for SettingsViewXaml.xaml
	/// </summary>
	public partial class SettingsView : UserControl,ISettingsEntryHost
	{
		public event EventHandler SettingsChanged;
		public event EventHandler SelectionChanged;
		
		// Remove empty constr from settingsentry
		
		static readonly Type[] defaultAvailableTypes = new Type[] {
			typeof(bool),
			typeof(byte),
			typeof(char),
			typeof(decimal),
			typeof(double),
			typeof(float),
			typeof(int),
			typeof(long),
			typeof(sbyte),
			typeof(short),
			typeof(string),
			typeof(System.Collections.Specialized.StringCollection),
			typeof(System.DateTime),
			typeof(System.Drawing.Color),
			typeof(System.Drawing.Font),
			typeof(System.Drawing.Point),
			typeof(System.Drawing.Size),
			typeof(System.Guid),
			typeof(System.TimeSpan),
			typeof(uint),
			typeof(ulong),
			typeof(ushort)
		};
		
		List<string> typeNames = new List<string>();
		List<Type> types = new List<Type>();
		IAmbience ambience;
		ICompilation compilation;
		
		public SettingsView()
		{
			
			InitializeComponent();
			ambience = AmbienceService.GetCurrentAmbience();
			compilation = MinimalCorlib.Instance.CreateCompilation();
			
			foreach (Type type in defaultAvailableTypes) {
				types.Add(type);
				typeNames.Add(ambience.ConvertType(type.ToTypeReference().Resolve(compilation)));
			}
			foreach (SpecialTypeDescriptor d in SpecialTypeDescriptor.Descriptors) {
				types.Add(d.type);
				typeNames.Add(d.name);
			}
			Entries = new ObservableCollection<SettingsVM>();
			
			
		}
		
		
		public void ShowEntries(IList<SettingsEntry> list)
		{
			foreach (var element in list) {
				Entries.Add(new SettingsVM(element));
			}
			
			Entries.CollectionChanged +=   ((s,e)=>
			                                {
			                                	OnSettingsChanged(e);
			                                });
			
			foreach (var element in Entries) {
				element.PropertyChanged += ((s,e) => {
				                            	OnSettingsChanged(e);
				                            });
				
			}
			
			this.DataContext = this;
			if (Entries.Count > 0) {
				datagrid.SelectedItem = Entries[0];
			}
		}
		
		
		public IEnumerable<SettingsEntry> GetAllEntries()
		{
			List<SettingsEntry> l = new List<SettingsEntry>();
			foreach (var element in Entries) {
				var s = element.Entry;
				if (!String.IsNullOrEmpty(s.Name)) {
					l.Add(s);
				}
			}
			l.Sort(delegate(SettingsEntry a, SettingsEntry b) {
			       	return a.Name.CompareTo(b.Name);
			       });
			return l;
		}
		
		
		public List<SettingsEntryPropertyGridWrapper> GetSelectedEntriesForPropertyGrid()
		{
			List<SettingsEntryPropertyGridWrapper> l = new List<SettingsEntryPropertyGridWrapper>();
				
			if (datagrid.SelectedItems.Count > 0) {
				foreach (var element in datagrid.SelectedItems) {
					var vm = element as SettingsVM;
					if (vm != null) {
						var settings = vm.Entry;
						if (settings != null) {
							l.Add(new SettingsEntryPropertyGridWrapper(settings));
						}
					}
				}
			} 
			return l;
		}
	
		#region Properties
		
		public ObservableCollection<SettingsVM> Entries {get;set;}
		
		public List<string> TypeNames {
			get {return typeNames;}
		}
		
		#endregion
		
		#region ISettingsEntryHost	
		
		public string GetDisplayNameForType(Type type)
		{
			foreach (SpecialTypeDescriptor d in SpecialTypeDescriptor.Descriptors) {
				if (type == d.type)
					return d.name;
			}
			return ambience.ConvertType(type.ToTypeReference().Resolve(compilation));
		}
		
		
		public Type GetTypeByDisplayName(string displayName)
		{
			for (int i = 0; i < typeNames.Count; i++) {
				if (typeNames[i] == displayName)
					return types[i];
			}
			return null;
		}
		
		#endregion
		
		protected virtual void OnSettingsChanged(EventArgs e)
		{
			if (SettingsChanged != null) {
				SettingsChanged(this, e);
			}
		}
		
		
		void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (SelectionChanged != null)
				SelectionChanged(this, e);
		}
		
		
		void Datagrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			var settings = new SettingsEntry(this);
			settings.Type = typeof(string);
			var vm = new SettingsVM(settings);
			e.NewItem = vm;
		}
	}
	
	
	public class SettingsVM: ViewModelBase
	{
		
		public SettingsVM ()
		{
		}
		
		public SettingsVM (SettingsEntry entry)
		{
			this.Entry = entry;
		}
		
		public SettingsEntry Entry {get; private set;}
				
		public string Name 
		{
			get {return this.Entry.Name;}
			set {Entry.Name = value;
				base.OnPropertyChanged("Name");
			}
				
		}
		
		public string Type
		{
			get {return Entry.WrappedSettingType;}
			set {Entry.WrappedSettingType = value;
				base.OnPropertyChanged("Type");
			}
		}
		
		
		public string SerializedValue {
			get {return Entry.SerializedValue;}
			set {Entry.SerializedValue = value;
				base.OnPropertyChanged("SerializedValue");}
		}
	
		
		
		public SettingScope Scope {
			get { return Entry.Scope; }
			set { Entry.Scope = value;
				Console.WriteLine(Scope.ToString());
				base.OnPropertyChanged("Scope");}
		}
		
	}
}
