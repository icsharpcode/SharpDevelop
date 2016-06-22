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
