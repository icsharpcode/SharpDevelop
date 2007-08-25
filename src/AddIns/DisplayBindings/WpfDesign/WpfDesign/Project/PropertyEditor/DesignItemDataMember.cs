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
	class DesignItemDataMember : IPropertyEditorDataMember
	{
		protected readonly DesignItemDataSource ownerDataSource;
		protected readonly DesignItemProperty property;
		
		protected DesignItemDataMember(DesignItemDataSource ownerDataSource, DesignItemProperty property)
		{
			Debug.Assert(ownerDataSource != null);
			Debug.Assert(property != null);
			
			this.ownerDataSource = ownerDataSource;
			this.property = property;
		}
		
		public IPropertyEditorDataSource OwnerDataSource {
			get { return ownerDataSource; }
		}
		
		public string Name {
			get { return property.Name; }
		}
		
		public object GetDescription()
		{
			IPropertyDescriptionService p = ownerDataSource.DesignItem.Services.GetService<IPropertyDescriptionService>();
			if (p != null)
				return p.GetDescription(property);
			else
				return null;
		}
		
		/// <summary>
		/// Gets the type that declares the property.
		/// </summary>
		public Type DeclaringType {
			get { return property.DeclaringType; }
		}
		
		/// <summary>
		/// Gets the type of the property value.
		/// </summary>
		public Type ReturnType {
			get { return property.ReturnType; }
		}
	}
}
