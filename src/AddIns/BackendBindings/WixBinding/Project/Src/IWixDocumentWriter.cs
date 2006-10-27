// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Intended to be used to hide the SharpDevelop workbench and the file system
	/// from the user of this class when they want to save the changes to a Wix
	/// document.
	/// </summary>
	public interface IWixDocumentWriter
	{
		/// <summary>
		/// Saves the changes to the Wix document.
		/// </summary>
		/// <param name="document">The Wix document to save.</param>
		void Write(WixDocument document);
	}
}
