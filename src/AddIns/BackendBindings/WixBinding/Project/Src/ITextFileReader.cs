// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
