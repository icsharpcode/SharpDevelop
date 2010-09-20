// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
