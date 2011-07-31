// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
