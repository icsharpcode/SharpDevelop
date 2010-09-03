// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
