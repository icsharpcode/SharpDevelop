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
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Panel with two combo boxes. Used to quickly navigate to entities in the current file.
	/// </summary>
	public partial class QuickClassBrowser : UserControl
	{
		/// <summary>
		/// ViewModel used for combobox items.
		/// </summary>
		class EntityItem : IComparable<EntityItem>, System.ComponentModel.INotifyPropertyChanged
		{
			IUnresolvedEntity entity;
			ImageSource image;
			string text;
			
			public IUnresolvedEntity Entity {
				get { return entity; }
			}
			
			public EntityItem(IUnresolvedTypeDefinition typeDef, ICompilation compilation)
			{
				this.IsInSamePart = true;
				this.entity = typeDef;
				var resolvedDefinition = typeDef.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
				if (resolvedDefinition != null) {
					var ambience = compilation.GetAmbience();
					ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList | ConversionFlags.ShowDeclaringType;
					this.text = ambience.ConvertSymbol(resolvedDefinition);
				} else {
					this.text = typeDef.Name;
				}
				this.image = CompletionImage.GetImage(typeDef);
			}
			
			public EntityItem(IMember member, IAmbience ambience)
			{
				this.IsInSamePart = true;
				this.entity = member.UnresolvedMember;
				ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList | ConversionFlags.ShowParameterList | ConversionFlags.ShowParameterNames;
				text = ambience.ConvertSymbol(member);
				image = CompletionImage.GetImage(member);
			}
			
			/// <summary>
			/// Text to display in combo box.
			/// </summary>
			public string Text {
				get { return text; }
			}
			
			/// <summary>
			/// Image to use in combox box
			/// </summary>
			public ImageSource Image {
				get {
					return image;
				}
			}
			
			/// <summary>
			/// Gets/Sets whether the item is in the current file.
			/// </summary>
			/// <returns>
			/// <c>true</c>: item is in current file;
			/// <c>false</c>: item is in another part of the partial class
			/// </returns>
			public bool IsInSamePart { get; set; }
			
			public int CompareTo(EntityItem other)
			{
				int r = this.Entity.SymbolKind.CompareTo(other.Entity.SymbolKind);
				if (r != 0)
					return r;
				r = string.Compare(text, other.text, StringComparison.OrdinalIgnoreCase);
				if (r != 0)
					return r;
				return string.Compare(text, other.text, StringComparison.Ordinal);
			}
			
			/// <summary>
			/// ToString override is necessary to support keyboard navigation in WPF
			/// </summary>
			public override string ToString()
			{
				return text;
			}
			
			// I'm not sure if it actually was a leak or caused by something else, but I saw QCB.EntityItem being alive for longer
			// than it should when looking at the heap with WinDbg.
			// Maybe this was caused by http://support.microsoft.com/kb/938416/en-us, so I'm adding INotifyPropertyChanged to be sure.
			event System.ComponentModel.PropertyChangedEventHandler System.ComponentModel.INotifyPropertyChanged.PropertyChanged {
				add { }
				remove { }
			}
		}
		
		public QuickClassBrowser()
		{
			InitializeComponent();
		}
		
		/// <summary>
		/// Updates the list of available classes.
		/// This causes the classes combo box to lose its current selection,
		/// so the members combo box will be cleared.
		/// </summary>
		public void Update(IUnresolvedFile compilationUnit)
		{
			runUpdateWhenDropDownClosed = true;
			runUpdateWhenDropDownClosedCU = compilationUnit;
			if (!IsDropDownOpen)
				ComboBox_DropDownClosed(null, null);
		}
		
		// The lists of items currently visible in the combo boxes.
		// These should never be null.
		List<EntityItem> classItems = new List<EntityItem>();
		List<EntityItem> memberItems = new List<EntityItem>();
		
		void DoUpdate(IUnresolvedFile unresolvedFile)
		{
			classItems = new List<EntityItem>();
			if (unresolvedFile != null) {
				ICompilation compilation = SD.ParserService.GetCompilationForFile(FileName.Create(unresolvedFile.FileName));
				AddClasses(unresolvedFile.TopLevelTypeDefinitions, compilation);
			}
			classItems.Sort();
			classComboBox.ItemsSource = classItems;
		}
		
		bool IsDropDownOpen {
			get { return classComboBox.IsDropDownOpen || membersComboBox.IsDropDownOpen; }
		}
		
		// Delayed execution - avoid changing combo boxes while the user is browsing the dropdown list.
		bool runUpdateWhenDropDownClosed;
		IUnresolvedFile runUpdateWhenDropDownClosedCU;
		bool runSelectItemWhenDropDownClosed;
		TextLocation runSelectItemWhenDropDownClosedLocation;
		
		void ComboBox_DropDownClosed(object sender, EventArgs e)
		{
			if (runUpdateWhenDropDownClosed) {
				runUpdateWhenDropDownClosed = false;
				DoUpdate(runUpdateWhenDropDownClosedCU);
				runUpdateWhenDropDownClosedCU = null;
			}
			if (runSelectItemWhenDropDownClosed) {
				runSelectItemWhenDropDownClosed = false;
				DoSelectItem(runSelectItemWhenDropDownClosedLocation);
			}
		}
		
		void AddClasses(IEnumerable<IUnresolvedTypeDefinition> classes, ICompilation compilation)
		{
			foreach (var c in classes) {
				if (c.IsSynthetic)
					continue;
				classItems.Add(new EntityItem(c, compilation));
				AddClasses(c.NestedTypes, compilation);
			}
		}
		
		/// <summary>
		/// Selects the class and member closest to the specified location.
		/// </summary>
		public void SelectItemAtCaretPosition(TextLocation location)
		{
			runSelectItemWhenDropDownClosed = true;
			runSelectItemWhenDropDownClosedLocation = location;
			if (!IsDropDownOpen)
				ComboBox_DropDownClosed(null, null);
		}
		
		void DoSelectItem(TextLocation location)
		{
			EntityItem matchInside = null;
			EntityItem nearestMatch = null;
			int nearestMatchDistance = int.MaxValue;
			foreach (EntityItem item in classItems) {
				if (item.IsInSamePart) {
					IUnresolvedTypeDefinition c = (IUnresolvedTypeDefinition)item.Entity;
					if (c.Region.IsInside(location.Line, location.Column)) {
						matchInside = item;
						// when there are multiple matches inside (nested classes), use the last one
					} else {
						// Not a perfect match?
						// Try to first the nearest match. We want the classes combo box to always
						// have a class selected if possible.
						int matchDistance = Math.Min(Math.Abs(location.Line - c.Region.BeginLine),
						                             Math.Abs(location.Line - c.Region.EndLine));
						if (matchDistance < nearestMatchDistance) {
							nearestMatchDistance = matchDistance;
							nearestMatch = item;
						}
					}
				}
			}
			jumpOnSelectionChange = false;
			try {
				classComboBox.SelectedItem = matchInside ?? nearestMatch;
				// the SelectedItem setter will update the list of member items
			} finally {
				jumpOnSelectionChange = true;
			}
			matchInside = null;
			foreach (EntityItem item in memberItems) {
				if (item.IsInSamePart) {
					IUnresolvedMember member = (IUnresolvedMember)item.Entity;
					if (member.Region.IsInside(location.Line, location.Column) || member.BodyRegion.IsInside(location.Line, location.Column)) {
						matchInside = item;
					}
				}
			}
			jumpOnSelectionChange = false;
			try {
				membersComboBox.SelectedItem = matchInside;
			} finally {
				jumpOnSelectionChange = true;
			}
		}
		
		bool jumpOnSelectionChange = true;
		
		void classComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// The selected class was changed.
			// Update the list of member items to be the list of members of the current class.
			EntityItem item = classComboBox.SelectedItem as EntityItem;
			IUnresolvedTypeDefinition selectedClass = item != null ? item.Entity as IUnresolvedTypeDefinition : null;
			memberItems = new List<EntityItem>();
			if (selectedClass != null) {
				ICompilation compilation = SD.ParserService.GetCompilationForFile(FileName.Create(selectedClass.UnresolvedFile.FileName));
				var context = new SimpleTypeResolveContext(compilation.MainAssembly);
				ITypeDefinition compoundClass = selectedClass.Resolve(context).GetDefinition();
				if (compoundClass != null) {
					var ambience = compilation.GetAmbience();
					foreach (var member in compoundClass.Members) {
						if (member.IsSynthetic)
							continue;
						bool isInSamePart = string.Equals(member.Region.FileName, selectedClass.Region.FileName, StringComparison.OrdinalIgnoreCase);
						memberItems.Add(new EntityItem(member, ambience) { IsInSamePart = isInSamePart });
					}
					memberItems.Sort();
					if (jumpOnSelectionChange) {
						SD.AnalyticsMonitor.TrackFeature(GetType(), "JumpToClass");
						JumpTo(item, selectedClass.Region);
					}
				}
			}
			membersComboBox.ItemsSource = memberItems;
		}
		
		void membersComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			EntityItem item = membersComboBox.SelectedItem as EntityItem;
			if (item != null) {
				IUnresolvedMember member = item.Entity as IUnresolvedMember;
				if (member != null && jumpOnSelectionChange) {
					SD.AnalyticsMonitor.TrackFeature(GetType(), "JumpToMember");
					JumpTo(item, member.Region);
				}
			}
		}
		
		void JumpTo(EntityItem item, DomRegion region)
		{
			if (region.IsEmpty)
				return;
			Action<int, int> jumpAction = this.JumpAction;
			if (item.IsInSamePart && jumpAction != null) {
				jumpAction(region.BeginLine, region.BeginColumn);
			} else {
				FileService.JumpToFilePosition(region.FileName, region.BeginLine, region.BeginColumn);
			}
		}
		
		/// <summary>
		/// Action used for jumping to a position inside the current file.
		/// </summary>
		public Action<int, int> JumpAction { get; set; }
	}
}
