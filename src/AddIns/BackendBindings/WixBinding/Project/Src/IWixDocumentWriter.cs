// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
