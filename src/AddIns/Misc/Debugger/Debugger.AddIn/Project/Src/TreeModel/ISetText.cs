// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Debugger.AddIn.TreeModel
{
	public interface ISetText
	{
		bool CanSetText { get; }
		
		bool SetText(string text);
	}
}
