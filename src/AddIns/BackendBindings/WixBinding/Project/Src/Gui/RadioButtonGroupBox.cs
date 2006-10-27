// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.WixBinding
{
	public class RadioButtonGroupBox : Panel
	{
		string propertyName = String.Empty;
		
		public RadioButtonGroupBox()
		{
		}
		
		/// <summary>
		/// Gets or sets the property that this radio button group maps to.
		/// </summary>
		public string PropertyName {
			get {
				return propertyName;
			}
			set {
				propertyName = value;
			}
		}
	}
}
