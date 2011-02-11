// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.NRefactory.CSharp
{
	/// <summary>
	/// Writes C# code into a TextWriter.
	/// </summary>
	public class TextWriterOutputFormatter : IOutputFormatter
	{
		readonly TextWriter textWriter;
		
		public TextWriterOutputFormatter(TextWriter textWriter)
		{
			if (textWriter == null)
				throw new ArgumentNullException("textWriter");
			this.textWriter = textWriter;
		}
		
		public void WriteIdentifier(string ident)
		{
			textWriter.Write(ident);
		}
		
		public void WriteKeyword(string keyword)
		{
			textWriter.Write(keyword);
		}
		
		public void WriteToken(string token)
		{
			textWriter.Write(token);
		}
		
		public void Space()
		{
			textWriter.Write(' ');
		}
		
		public void OpenBrace()
		{
			textWriter.Write('{');
		}
		
		public void CloseBrace()
		{
			textWriter.Write('}');
		}
		
		public void NewLine()
		{
			textWriter.WriteLine();
		}
		
		public void WriteComment(CommentType commentType, string content)
		{
			throw new NotImplementedException();
		}
	}
}
