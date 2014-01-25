// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;
using Microsoft.Scripting;

namespace ICSharpCode.PythonBinding
{
	public static class PythonMethodOrClassBodyRegion
	{
		/// <summary>
		/// Gets the body region for a class or a method.
		/// </summary>
		/// <remarks>
		/// Note that SharpDevelop line numbers are zero based but the
		/// DomRegion values are one based. IronPython columns and lines are one based.
		/// </remarks>
		/// <param name="body">The body statement.</param>
		/// <param name="header">The location of the header. This gives the end location for the
		/// method or class definition up to the colon.</param>
		public static DomRegion GetBodyRegion(Statement body, SourceLocation header)
		{
			int columnAfterColonCharacter = header.Column + 1;
			SourceLocation bodyEnd = GetBodyEndLocation(body);
			return new DomRegion(header.Line, columnAfterColonCharacter, bodyEnd.Line, bodyEnd.Column);
		}
		
		static SourceLocation GetBodyEndLocation(Statement body)
		{
			if (body.Parent != null) {
				return body.End;
			}
			return SourceLocation.Invalid;
		}
		
		public static DomRegion GetBodyRegion(FunctionDefinition methodDefinition)
		{
			return GetBodyRegion(methodDefinition.Body, methodDefinition.Header);
		}
		
		public static DomRegion GetBodyRegion(ClassDefinition classDefinition)
		{
			return GetBodyRegion(classDefinition.Body, classDefinition.Header);
		}
	}
}
