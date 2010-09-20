// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;

namespace ICSharpCode.CodeCoverage.Tests
{
	/// <summary>
	/// Helper class that implements the Text Editor library's IDocument interface.
	/// </summary>
	public class MockDocument
	{
		public static IDocument Create()
		{
			return new DocumentFactory().CreateDocument();
		}
	}
}
