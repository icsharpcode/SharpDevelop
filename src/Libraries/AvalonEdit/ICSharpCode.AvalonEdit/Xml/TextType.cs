// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Xml
{
	/// <summary> Identifies the context in which the text occured </summary>
	enum TextType
	{
		/// <summary> Ends with non-whitespace </summary>
		WhiteSpace,
		
		/// <summary> Ends with "&lt;";  "]]&gt;" is error </summary>
		CharacterData,
		
		/// <summary> Ends with "-->";  "--" is error </summary>
		Comment,
		
		/// <summary> Ends with "]]&gt;" </summary>
		CData,
		
		/// <summary> Ends with "?>" </summary>
		ProcessingInstruction,
		
		/// <summary> Ends with "&lt;" or ">" </summary>
		UnknownBang,
		
		/// <summary> Unknown </summary>
		Other
	}
}
