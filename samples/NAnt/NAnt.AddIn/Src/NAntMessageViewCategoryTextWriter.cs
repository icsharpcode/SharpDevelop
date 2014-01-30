// SharpDevelop samples
// Copyright (c) 2014, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.NAnt
{
	/// <summary>
	/// TextWriter implementation that writes into a MessageViewCategory.
	/// </summary>
	public class NAntMessageViewCategoryTextWriter : TextWriter
	{
		readonly IOutputCategory target;
		StringBuilder output;
		
		public NAntMessageViewCategoryTextWriter(IOutputCategory target)
		{
			this.target = target;
			this.output = new StringBuilder();
		}
		
		public override Encoding Encoding {
			get { return Encoding.Unicode; }
		}
		
		public override void Write(char value)
		{
			target.AppendText(value.ToString());
			output.Append(value.ToString());
		}
		
		public override void Write(string value)
		{
			if (value != null) {
				target.AppendText(value);
				output.Append(value);
			}
		}
		
		public override void Write(char[] buffer, int index, int count)
		{
			string text = new string(buffer, index, count);
			target.AppendText(text);
			output.Append(text);
		}
		
		public string Output {
			get { return output.ToString(); }
		}
	}
}
