// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace SearchAndReplace
{
	/// <summary>
	/// Builds a text iterator object.
	/// </summary>
	public interface ITextIteratorBuilder
	{
		ITextIterator BuildTextIterator(ProvidedDocumentInformation info);
	}
}
