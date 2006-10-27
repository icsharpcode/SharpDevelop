// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
