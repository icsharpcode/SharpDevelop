// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Panel with two combo boxes. Used to quickly navigate to entities in the current file.
	/// </summary>
	public partial class QuickClassBrowser : UserControl
	{
		// type codes are used for sorting entities by type
		const int TYPE_CLASS = 0;
		const int TYPE_CONSTRUCTOR = 1;
		const int TYPE_METHOD = 2;
		const int TYPE_PROPERTY = 3;
		const int TYPE_FIELD = 4;
		const int TYPE_EVENT = 5;
		
		/// <summary>
		/// ViewModel used for combobox items.
		/// </summary>
		class EntityItem : IComparable<EntityItem>, System.ComponentModel.INotifyPropertyChanged
		{
			IEntity entity;
			IImage image;
			string text;
			int typeCode; // type code is used for sorting entities by type
			
			public IEntity Entity {
				get { return entity; }
			}
			
			public EntityItem(IEntity entity, int typeCode)
			{
				this.IsInSamePart = true;
				this.entity = entity;
				this.typeCode = typeCode;
				IAmbience ambience = entity.ProjectContent.Language.GetAmbience();
				if (entity is IClass)
					ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList | ConversionFlags.UseFullyQualifiedMemberNames;
				else
					ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList | ConversionFlags.ShowParameterList | ConversionFlags.ShowParameterNames;
				text = ambience.Convert(entity);
				image = ClassBrowserIconService.GetIcon(entity);
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
					return image.ImageSource;
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
				int r = this.typeCode.CompareTo(other.typeCode);
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
		public void Update(ICompilationUnit compilationUnit)
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
		
		void DoUpdate(ICompilationUnit compilationUnit)
		{
			classItems = new List<EntityItem>();
			if (compilationUnit != null) {
				AddClasses(compilationUnit.Classes);
			}
			classItems.Sort();
			classComboBox.ItemsSource = classItems;
		}
		
		bool IsDropDownOpen {
			get { return classComboBox.IsDropDownOpen || membersComboBox.IsDropDownOpen; }
		}
		
		// Delayed execution - avoid changing combo boxes while the user is browsing the dropdown list.
		bool runUpdateWhenDropDownClosed;
		ICompilationUnit runUpdateWhenDropDownClosedCU;
		bool runSelectItemWhenDropDownClosed;
		Location runSelectItemWhenDropDownClosedLocation;
		
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
		
		void AddClasses(IEnumerable<IClass> classes)
		{
			foreach (IClass c in classes) {
				classItems.Add(new EntityItem(c, TYPE_CLASS));
				AddClasses(c.InnerClasses);
			}
		}
		
		/// <summary>
		/// Selects the class and member closest to the specified location.
		/// </summary>
		public void SelectItemAtCaretPosition(Location location)
		{
			runSelectItemWhenDropDownClosed = true;
			runSelectItemWhenDropDownClosedLocation = location;
			if (!IsDropDownOpen)
				ComboBox_DropDownClosed(null, null);
		}
		
		void DoSelectItem(Location location)
		{
			EntityItem matchInside = null;
			EntityItem nearestMatch = null;
			int nearestMatchDistance = int.MaxValue;
			foreach (EntityItem item in classItems) {
				if (item.IsInSamePart) {
					IClass c = (IClass)item.Entity;
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
					IMember member = (IMember)item.Entity;
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
			IClass selectedClass = item != null ? item.Entity as IClass : null;
			memberItems = new List<EntityItem>();
			if (selectedClass != null) {
				IClass compoundClass = selectedClass.GetCompoundClass();
				foreach (var m in compoundClass.Methods) {
					AddMember(selectedClass, m, m.IsConstructor ? TYPE_CONSTRUCTOR : TYPE_METHOD);
				}
				foreach (var m in compoundClass.Properties) {
					AddMember(selectedClass, m, TYPE_PROPERTY);
				}
				foreach (var m in compoundClass.Fields) {
					AddMember(selectedClass, m, TYPE_FIELD);
				}
				foreach (var m in compoundClass.Events) {
					AddMember(selectedClass, m, TYPE_EVENT);
				}
				memberItems.Sort();
				if (jumpOnSelectionChange) {
					AnalyticsMonitorService.TrackFeature(GetType(), "JumpToClass");
					JumpTo(item, selectedClass.Region);
				}
			}
			membersComboBox.ItemsSource = memberItems;
		}
		
		void AddMember(IClass selectedClass, IMember member, int typeCode)
		{
			bool isInSamePart = (member.DeclaringType == selectedClass);
			memberItems.Add(new EntityItem(member, typeCode) { IsInSamePart = isInSamePart });
		}
		
		void membersComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			EntityItem item = membersComboBox.SelectedItem as EntityItem;
			if (item != null) {
				IMember member = item.Entity as IMember;
				if (member != null && jumpOnSelectionChange) {
					AnalyticsMonitorService.TrackFeature(GetType(), "JumpToMember");
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
				FileService.JumpToFilePosition(item.Entity.CompilationUnit.FileName, region.BeginLine, region.BeginColumn);
			}
		}
		
		/// <summary>
		/// Action used for jumping to a position inside the current file.
		/// </summary>
		public Action<int, int> JumpAction { get; set; }
	}
}
