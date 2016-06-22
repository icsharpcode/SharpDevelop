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
