// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// TextWriter implementation that writes into a MessageViewCategory.
	/// </summary>
	public class MessageViewCategoryTextWriter : TextWriter
	{
		readonly IOutputCategory target;
		
		public MessageViewCategoryTextWriter(IOutputCategory target)
		{
			this.target = target;
		}
		
		public override Encoding Encoding {
			get { return Encoding.Unicode; }
		}
		
		public override void Write(char value)
		{
			target.AppendText(value.ToString());
		}
		
		public override void Write(string value)
		{
			if (value != null)
				target.AppendText(value);
		}
		
		public override void Write(char[] buffer, int index, int count)
		{
			target.AppendText(new string(buffer, index, count));
		}
	}
}
