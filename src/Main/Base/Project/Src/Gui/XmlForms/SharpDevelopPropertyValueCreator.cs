// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.SharpDevelop.WinForms;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	[Obsolete("XML Forms are obsolete")]
	public class SharpDevelopPropertyValueCreator : IPropertyValueCreator
	{
		public bool CanCreateValueForType(Type propertyType)
		{
			return propertyType == typeof(Icon) || propertyType == typeof(Image);
		}
		
		public object CreateValue(Type propertyType, string valueString)
		{
			
			if (propertyType == typeof(Icon)) {
				return SD.ResourceService.GetIcon(valueString);
			}
			
			if (propertyType == typeof(Image)) {
				return SD.ResourceService.GetBitmap(valueString);
			}
			
			return null;
		}
	}
}
