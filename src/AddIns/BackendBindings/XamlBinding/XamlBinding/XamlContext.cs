// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public enum XamlContext {
		/// <summary>
		/// After '&lt;'
		/// </summary>
		AtTag,
		/// <summary>
		/// Inside '&lt;TagName &gt;'
		/// </summary>
		InTag,
		/// <summary>
		/// Inside '="Value"'
		/// </summary>
		InAttributeValue,
		/// <summary>
		/// Inside '="{}"'
		/// </summary>
		InMarkupExtension
	}
}
