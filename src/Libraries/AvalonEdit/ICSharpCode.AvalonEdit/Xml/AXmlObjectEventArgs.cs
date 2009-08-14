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
	/// <summary> Holds event args for event caused by <see cref="AXmlObject"/> </summary>
	public class AXmlObjectEventArgs: EventArgs
	{
		/// <summary> The object that caused the event </summary>
		public AXmlObject Object { get; set; }
	}
}
