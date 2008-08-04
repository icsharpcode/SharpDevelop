// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
