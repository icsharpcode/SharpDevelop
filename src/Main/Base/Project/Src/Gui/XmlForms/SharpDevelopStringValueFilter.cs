// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	/// <summary>
	/// This interface is used to filter the values defined in the xml files.
	/// It could be used for the localization of control texts.
	/// </summary>
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
