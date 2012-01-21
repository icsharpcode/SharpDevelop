// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.InteropServices;

namespace ICSharpCode.CodeQuality
{
	/// <summary>
	/// Description of Utils.
	/// </summary>
	public class Utils
	{
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
	}
}
