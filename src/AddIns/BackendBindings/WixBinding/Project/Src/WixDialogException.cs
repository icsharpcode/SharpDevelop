// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WixBinding
{
	public class WixDialogException : Exception
	{
		string elementName = String.Empty;
		string id = String.Empty;
		
		public WixDialogException()
		{
		}
		
		public WixDialogException(string message, string elementName, string id) : base(message)
		{
			this.elementName = elementName;
			this.id = id;
		}
		
		/// <summary>
		/// Gets the element where the exception occurred.
		/// </summary>
		public string ElementName {
			get {
				return elementName;
			}
		}
		
		/// <summary>
		/// Gets the control id for the element where the exception occurred.
		/// </summary>
		public string Id {
			get {
				return id;
			}
		}
	}
}
