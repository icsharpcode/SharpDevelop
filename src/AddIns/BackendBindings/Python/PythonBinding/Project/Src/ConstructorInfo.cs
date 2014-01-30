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
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.PythonBinding
{
	public class PythonConstructorInfo
	{
		ConstructorDeclaration constructor;
		List<FieldDeclaration> fields = new List<FieldDeclaration>();
		
		PythonConstructorInfo(ConstructorDeclaration constructor, List<FieldDeclaration> fields)
		{
			this.constructor = constructor;
			this.fields = fields;
		}
		
		/// <summary>
		/// Gets the constructor information from a type declaration. Returns null if there is no 
		/// constructor defined or if there are no fields defined.
		/// </summary>
		public static PythonConstructorInfo GetConstructorInfo(TypeDeclaration type)
		{
			List<FieldDeclaration> fields = new List<FieldDeclaration>();
			ConstructorDeclaration constructor = null;
			foreach (INode node in type.Children) {
				ConstructorDeclaration currentConstructor = node as ConstructorDeclaration;
				FieldDeclaration field = node as FieldDeclaration;
				if (currentConstructor != null) {
					constructor = currentConstructor;
				} else if (field != null) {
					fields.Add(field);
				}
			}
			
			if ((fields.Count > 0) || (constructor != null)) {
				return new PythonConstructorInfo(constructor, fields);
			}
			return null;
		}
		
		public ConstructorDeclaration Constructor {
			get { return constructor; }
		}
		
		public List<FieldDeclaration> Fields {
			get { return fields; }
		}
	}
}
