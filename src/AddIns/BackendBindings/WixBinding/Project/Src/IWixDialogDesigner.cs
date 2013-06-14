// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;

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
		
		/// <summary>
		/// Gets the text editor in the primary view content.
		/// </summary>
		ITextEditor PrimaryViewContentTextEditor { get; }
	}
}
