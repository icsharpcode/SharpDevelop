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
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Dummy ambience implementation.
	/// </summary>
	internal class DefaultAmbience : IAmbience
	{
		public ConversionFlags ConversionFlags { get; set; }

		public string ConvertSymbol(ISymbol symbol)
		{
			return symbol.Name;
		}
		
		public string ConvertEntity(IEntity e)
		{
			return e.Name;
		}
		
		public string ConvertType(IType type)
		{
			return type.Name;
		}
		
		public string ConvertVariable(IVariable variable)
		{
			return variable.Name;
		}
		
		public string WrapComment(string comment)
		{
			return "// " + comment;
		}
		
		public string ConvertConstantValue(object constantValue)
		{
			if (constantValue == null)
				return "null";
			if (constantValue is char)
				return "'" + constantValue + "'";
			if (constantValue is String)
				return "\"" + constantValue + "\"";
			return constantValue.ToString();
		}
	}
}
