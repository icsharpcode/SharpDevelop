// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	public class WixDialogElement : WixElementBase
	{
		public const string DialogElementName = "Dialog";
		
		public WixDialogElement(WixDocument document)
			: base(DialogElementName, document)
		{
		}
	}
}
