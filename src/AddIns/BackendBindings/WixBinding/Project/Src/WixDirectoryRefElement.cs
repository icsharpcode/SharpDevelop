// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.WixBinding
{
	public class WixDirectoryRefElement : WixDirectoryElementBase
	{
		public const string DirectoryRefElementName = "DirectoryRef";

		public WixDirectoryRefElement(WixDocument document)
			: base(DirectoryRefElementName, document)
		{		
		}
	}
}
