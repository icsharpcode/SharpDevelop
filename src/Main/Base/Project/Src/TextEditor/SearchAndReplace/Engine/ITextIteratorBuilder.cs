// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
