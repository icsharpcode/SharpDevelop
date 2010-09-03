// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
