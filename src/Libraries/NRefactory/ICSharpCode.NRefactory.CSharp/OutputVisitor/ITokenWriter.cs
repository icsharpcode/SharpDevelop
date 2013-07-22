// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;

namespace ICSharpCode.NRefactory.CSharp
{
	public interface ITokenWriter
	{
		void StartNode(AstNode node);
		void EndNode(AstNode node);
		
		/// <summary>
		/// Writes an identifier.
		/// </summary>
		void WriteIdentifier(Identifier identifier);
		
		/// <summary>
		/// Writes a keyword to the output.
		/// </summary>
		void WriteKeyword(Role role, string keyword);
		
		/// <summary>
		/// Writes a token to the output.
		/// </summary>
		void WriteToken(Role role, string token);
		
		/// <summary>
		/// Writes a primitive/literal value
		/// </summary>
		void WritePrimitiveValue(object value);
		
		void Space();
		void Indent();
		void Unindent();
		void NewLine();
		
		void WriteComment(CommentType commentType, string content);
		void WritePreProcessorDirective(PreProcessorDirectiveType type, string argument);
	}
	
	public abstract class DecoratingTokenWriter : ITokenWriter
	{
		ITokenWriter decoratedWriter;
		
		protected DecoratingTokenWriter(ITokenWriter decoratedWriter)
		{
			if (decoratedWriter == null)
				throw new ArgumentNullException("decoratedWriter");
			this.decoratedWriter = decoratedWriter;
		}
		
		public virtual void StartNode(AstNode node)
		{
			decoratedWriter.StartNode(node);
		}
		
		public virtual void EndNode(AstNode node)
		{
			decoratedWriter.EndNode(node);
		}
		
		public virtual void WriteIdentifier(Identifier identifier)
		{
			decoratedWriter.WriteIdentifier(identifier);
		}
		
		public virtual void WriteKeyword(Role role, string keyword)
		{
			decoratedWriter.WriteKeyword(role, keyword);
		}
		
		public virtual void WriteToken(Role role, string token)
		{
			decoratedWriter.WriteToken(role, token);
		}
		
		public virtual void WritePrimitiveValue(object value)
		{
			decoratedWriter.WritePrimitiveValue(value);
		}
		
		public virtual void Space()
		{
			decoratedWriter.Space();
		}
		
		public virtual void Indent()
		{
			decoratedWriter.Indent();
		}
		
		public virtual void Unindent()
		{
			decoratedWriter.Unindent();
		}
		
		public virtual void NewLine()
		{
			decoratedWriter.NewLine();
		}
		
		public virtual void WriteComment(CommentType commentType, string content)
		{
			decoratedWriter.WriteComment(commentType, content);
		}
		
		public virtual void WritePreProcessorDirective(PreProcessorDirectiveType type, string argument)
		{
			decoratedWriter.WritePreProcessorDirective(type, argument);
		}
	}
}


