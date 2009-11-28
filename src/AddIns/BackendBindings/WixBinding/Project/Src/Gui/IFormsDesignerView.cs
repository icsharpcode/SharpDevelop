// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.WixBinding
{
	public interface IFormsDesignerView
	{
		OpenedFile PrimaryFile { get; }
	}
}
