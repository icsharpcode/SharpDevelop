// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

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
		const int TYPE_CLASS = 0;
		const int TYPE_CONSTRUCTOR = 1;
		const int TYPE_METHOD = 2;
		const int TYPE_PROPERTY = 3;
		const int TYPE_FIELD = 4;
		const int TYPE_EVENT = 5;
		
		class EntityItem : IComparable<EntityItem>
		{
			IEntity entity;
			IImage image;
			string text;
			int typeCode; // type code is used for sorting by type
			
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
			
			public string Text {
				get { return text; }
			}
			
			public ImageSource Image {
				get {
					return image.ImageSource;
				}
			}
			
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
		}
		
		public QuickClassBrowser()
		{
			InitializeComponent();
			Update(null);
		}
		
		List<EntityItem> classItems;
		List<EntityItem> memberItems;
		
		public void Update(ICompilationUnit compilationUnit)
		{
			classItems = new List<EntityItem>();
			if (compilationUnit != null) {
				AddClasses(compilationUnit.Classes);
			}
			classItems.Sort();
			classComboBox.ItemsSource = classItems;
		}
		
		void AddClasses(IEnumerable<IClass> classes)
		{
			foreach (IClass c in classes) {
				classItems.Add(new EntityItem(c, TYPE_CLASS));
				AddClasses(c.InnerClasses);
			}
		}
		
		public void SelectItemAtCaretPosition(Location location)
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
					JumpTo(item, member.Region);
				}
			}
		}
		
		void JumpTo(EntityItem item, DomRegion region)
		{
			if (region.IsEmpty)
				return;
			if (item.IsInSamePart) {
				Action<DomRegion> jumpAction = this.JumpAction;
				if (jumpAction != null) {
					jumpAction(region);
				}
			} else {
				FileService.JumpToFilePosition(item.Entity.CompilationUnit.FileName, region.BeginLine, region.BeginColumn);
			}
		}
		
		public Action<DomRegion> JumpAction { get; set; }
	}
}