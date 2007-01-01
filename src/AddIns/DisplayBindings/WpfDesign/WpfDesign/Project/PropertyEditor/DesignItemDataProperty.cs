// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// wraps a DesignItemDataProperty for the property editor/grid.
	/// </summary>
	sealed class DesignItemDataProperty : IPropertyEditorDataProperty
	{
		readonly DesignItemDataSource ownerDataSource;
		readonly DesignItemProperty property;
		
		internal DesignItemDataProperty(DesignItemDataSource ownerDataSource, DesignItemProperty property)
		{
			Debug.Assert(ownerDataSource != null);
			Debug.Assert(property != null);
			
			this.ownerDataSource = ownerDataSource;
			this.property = property;
		}
		
		public IPropertyEditorDataSource OwnerDataSource {
			get { return ownerDataSource; }
		}
		
		public string Category {
			get { return "Misc"; }
		}
		
		public string Name {
			get { return property.Name; }
		}
		
		public string Description {
			get { return "Description for " + property.Name; }
		}
		
		public bool IsSet {
			get {
				return property.IsSet;
			}
			set {
				if (value != property.IsSet) {
					if (value) {
						// copy default value to local value
						property.SetValue(property.ValueOnInstance);
					} else {
						property.Reset();
					}
				}
			}
		}
		
		public object Value {
			get {
				return property.ValueOnInstance;
			}
			set {
				property.SetValue(value);
			}
		}
		
		public bool CanUseCustomExpression {
			get {
				return true;
			}
		}
		
		public void SetCustomExpression(string expression)
		{
			throw new NotImplementedException();
		}
	}
}
