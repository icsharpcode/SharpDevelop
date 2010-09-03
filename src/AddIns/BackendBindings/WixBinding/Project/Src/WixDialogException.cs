// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
