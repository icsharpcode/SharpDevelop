// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>
using System;

namespace ICSharpCode.SharpDevelop.Dom.VBNet
{
	/// <summary>
	/// Description of IVBNetOptionProvider.
	/// </summary>
	public interface IVBNetOptionProvider
	{
		bool? OptionInfer { get; }
		bool? OptionStrict { get; }
		bool? OptionExplicit { get; }
		CompareKind? OptionCompare { get; }
	}
	
	public enum CompareKind
	{
		Binary,
		Text
	}
}
