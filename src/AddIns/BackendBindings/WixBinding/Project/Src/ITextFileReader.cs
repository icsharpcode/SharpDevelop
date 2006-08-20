// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Intended to be used to hide the SharpDevelop workbench and the file system
	/// from the user of this class when they want to read a text file.
	/// </summary>
	public interface ITextFileReader
	{
		TextReader Create(string fileName);
	}
}
