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
	public class WixNamespaceManager : XmlNamespaceManager
	{
		public const string Prefix = "w";
		public const string Namespace = "http://schemas.microsoft.com/wix/2006/wi";
		
		public WixNamespaceManager(XmlNameTable nameTable) : base(nameTable)
		{
			AddNamespace(Prefix, Namespace);
		}
	}
}
