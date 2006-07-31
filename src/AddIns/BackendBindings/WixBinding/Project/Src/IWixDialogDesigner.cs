// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WixBinding
{
	public interface IWixDialogDesigner
	{
		/// <summary>
		/// Gets the dialog id that is currently being designed.
		/// </summary>
		string DialogId {get;}
		
		/// <summary>
		/// Gets the Wix Document XML that is loaded in the text editor
		/// atached to the designer.
		/// </summary>
		string GetDocumentXml();
		
		/// <summary>
		/// Gets the Wix document filename.
		/// </summary>
		string DocumentFileName {get;}
		
		/// <summary>
		/// Gets the WixProject associated with the document currently 
		/// being designed.
		/// </summary>
		WixProject Project {get;}
	}
}
