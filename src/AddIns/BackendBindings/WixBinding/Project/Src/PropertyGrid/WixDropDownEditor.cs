// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.SharpDevelop.Widgets.DesignTimeSupport;

namespace ICSharpCode.WixBinding
{
	public class WixDropDownEditor : DropDownEditor
	{
		protected override Control CreateDropDownControl(ITypeDescriptorContext context, IWindowsFormsEditorService editorService)
		{
			return new DropDownEditorListBox(editorService, GetDropDownItems(context));
		}
		
		IEnumerable<string> GetDropDownItems(ITypeDescriptorContext context)
		{
			if (context != null) {
				WixXmlAttributePropertyDescriptor propertyDescriptor = context.PropertyDescriptor as WixXmlAttributePropertyDescriptor;
				if (propertyDescriptor != null && propertyDescriptor.WixXmlAttribute.HasValues) {
					return propertyDescriptor.WixXmlAttribute.Values;
				}
			}
			return new string[0];
		}
	}
}
