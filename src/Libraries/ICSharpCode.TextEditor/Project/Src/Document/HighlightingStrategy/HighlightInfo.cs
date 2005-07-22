// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.TextEditor.Document
{
	public class HighlightInfo
	{
		public bool BlockSpanOn = false;
		public bool Span        = false;
		public Span CurSpan     = null;
		
		public HighlightInfo(Span curSpan, bool span, bool blockSpanOn)
		{
			this.CurSpan     = curSpan;
			this.Span        = span;
			this.BlockSpanOn = blockSpanOn;
		}
	}
}
