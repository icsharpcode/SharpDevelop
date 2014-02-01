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
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	/// <summary>
	/// This interface is used to filter the values defined in the xml files.
	/// It could be used for the localization of control texts.
	/// </summary>
	[Obsolete("XML Forms are obsolete")]
	public class SharpDevelopStringValueFilter : IStringValueFilter
	{
		/// <summary>
		/// Is called for each value string in the definition xml file.
		/// </summary>
		/// <returns>
		/// The filtered text value
		/// </returns>
		public string GetFilteredValue(string originalValue)
		{
//			bool useFlatStyle = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)PropertyService.Get("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
//			
//			StringParser.Properties["BORDERSTYLE"] = useFlatStyle ? BorderStyle.FixedSingle.ToString() : BorderStyle.Fixed3D.ToString();
//			StringParser.Properties["FLATSTYLE"]   = useFlatStyle ? FlatStyle.Flat.ToString() : FlatStyle.Standard.ToString();			
			string back = StringParser.Parse(originalValue);
			return back;
		}
	}
}
