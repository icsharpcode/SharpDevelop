// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	public class ForwardTextIteratorBuilder : ITextIteratorBuilder
	{
		public ITextIterator BuildTextIterator(ProvidedDocumentInformation info)
		{
			Debug.Assert(info != null);
			return new ForwardTextIterator(info);
		}
	}
}
