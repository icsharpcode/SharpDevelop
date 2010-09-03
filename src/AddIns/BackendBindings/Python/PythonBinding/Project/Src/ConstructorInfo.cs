// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
