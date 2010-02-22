// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary>
	/// Description of StringHelper.
	/// </summary>
	public class StringHelper
	{
		public static string Repeat(char c, int count)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(c, count);
			return sb.ToString();
		}
	}
}
