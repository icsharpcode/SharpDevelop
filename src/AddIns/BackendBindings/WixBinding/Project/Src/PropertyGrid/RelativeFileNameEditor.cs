// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Used to editor relative filenames in the property grid.
	/// </summary>
	public class RelativeFileNameEditor : FileNameEditor
	{
		public RelativeFileNameEditor()
		{
		}
		
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			WixDocument document = GetWixDocument(context);
			
			// Convert relative path to full path for editing.
			string relativePath = (string)value;
			string fullPath = document.GetFullPath(relativePath);
			string newFullPath = (string)base.EditValue(context, provider, fullPath);
			
			// Convert full path back to relative path.
			return document.GetRelativePath(newFullPath);
		}
		
		
		/// <summary>
		/// Gets the Wix document associated with the property 
		/// descriptor.
		/// </summary>
		WixDocument GetWixDocument(ITypeDescriptorContext context)
		{
			WixDocument document = null;
			if (context != null) {
				WixXmlAttributePropertyDescriptor propertyDescriptor = context.PropertyDescriptor as WixXmlAttributePropertyDescriptor;
				if (propertyDescriptor != null) {
					document = propertyDescriptor.WixXmlAttribute.Document;
				}
			}
			
			if (document != null) {
				return document;
			}
			return new WixDocument();
		}
	}
}
