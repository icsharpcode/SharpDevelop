// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>
using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Description of IOption.
	/// </summary>
	public interface IOption : IFreezable
	{
		DomRegion Region {
			get;
		}
		
		OptionType Type {
			get;
		}
		
		bool Value {
			get;
		}
	}
}
