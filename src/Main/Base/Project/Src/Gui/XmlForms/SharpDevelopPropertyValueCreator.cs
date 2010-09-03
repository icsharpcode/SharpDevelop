// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	public class SharpDevelopPropertyValueCreator : IPropertyValueCreator
	{
		public bool CanCreateValueForType(Type propertyType)
		{
			return propertyType == typeof(Icon) || propertyType == typeof(Image);
		}
		
		public object CreateValue(Type propertyType, string valueString)
		{
			
			if (propertyType == typeof(Icon)) {
				return WinFormsResourceService.GetIcon(valueString);
			}
			
			if (propertyType == typeof(Image)) {
				return WinFormsResourceService.GetBitmap(valueString);
			}
			
			return null;
		}
	}
}
