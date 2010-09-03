// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ICSharpCode.Core
{
	/// <summary>
	/// TextWriter that writes into System.Diagnostics.Debug
	/// </summary>
	public class DebugTextWriter : TextWriter
	{
		public override Encoding Encoding {
			get {
				return Encoding.Unicode;
			}
		}
		
		public override void Write(char value)
		{
			Debug.Write(value.ToString());
		}
		
		public override void Write(char[] buffer, int index, int count)
		{
			Debug.Write(new string(buffer, index, count));
		}
		
		public override void Write(string value)
		{
			Debug.Write(value);
		}
		
		public override void WriteLine()
		{
			Debug.WriteLine(string.Empty);
		}
		
		public override void WriteLine(string value)
		{
			Debug.WriteLine(value);
		}
	}
}
