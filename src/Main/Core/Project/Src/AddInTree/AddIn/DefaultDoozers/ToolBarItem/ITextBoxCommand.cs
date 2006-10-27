// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="John Simons" email="johnsimons007@yahoo.com.au"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	public interface ITextBoxCommand : ICommand
	{
		bool IsEnabled {
			get;
			set;
		}
	}
}
