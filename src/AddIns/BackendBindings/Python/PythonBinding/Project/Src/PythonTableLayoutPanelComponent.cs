// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.PythonBinding
{
	public class PythonTableLayoutPanelComponent : PythonDesignerComponent
	{
		public PythonTableLayoutPanelComponent(PythonDesignerComponent parent, IComponent component) 
			: base(parent, component)
		{
		}
		
		public override void AppendMethodCallWithArrayParameter(PythonCodeBuilder codeBuilder, string propertyOwnerName, object propertyOwner, PropertyDescriptor propertyDescriptor)
		{
			if (IsStylesProperty(propertyDescriptor.Name)) {
				ICollection collection = propertyDescriptor.GetValue(propertyOwner) as ICollection;
				if (collection != null) {
					AppendStylesCollection(codeBuilder, collection, propertyOwnerName);
				}
			} else {
				base.AppendMethodCallWithArrayParameter(codeBuilder, propertyOwnerName, propertyOwner, propertyDescriptor);
			}
		}

		protected override bool ShouldAppendCollectionContent {
			get { return false; }
		}
		
		bool IsStylesProperty(string name)
		{
			return "ColumnStyles".Equals(name, StringComparison.InvariantCultureIgnoreCase) || "RowStyles".Equals(name, StringComparison.InvariantCultureIgnoreCase);
		}

		void AppendStylesCollection(PythonCodeBuilder codeBuilder, ICollection collection, string propertyOwnerName)
		{
			string newPropertyOwnerName = propertyOwnerName;
			SizeType sizeType = SizeType.Absolute;
			float width = 0;
			foreach (object item in collection) {
				ColumnStyle columnStyle = item as ColumnStyle;
				RowStyle rowStyle = item as RowStyle;
				if (columnStyle != null) {
					sizeType = columnStyle.SizeType;
					width = columnStyle.Width;
					newPropertyOwnerName = propertyOwnerName + ".ColumnStyles";
				}
				if (rowStyle != null) {
					sizeType = rowStyle.SizeType;
					width = rowStyle.Height;
					newPropertyOwnerName = propertyOwnerName + ".RowStyles";
				}
				AppendStyle(codeBuilder, newPropertyOwnerName, item.GetType(), sizeType, width);
			}		
		}
		
		void AppendStyle(PythonCodeBuilder codeBuilder, string propertyOwnerName, Type type, SizeType sizeType, float value)
		{
			codeBuilder.AppendIndented(propertyOwnerName + ".Add(");
			codeBuilder.Append(type.FullName + "(");
			codeBuilder.Append(typeof(SizeType).FullName + "." + sizeType + ", ");
			codeBuilder.Append(value + ")");
			codeBuilder.Append(")");
			codeBuilder.AppendLine();
		}
	}
}
