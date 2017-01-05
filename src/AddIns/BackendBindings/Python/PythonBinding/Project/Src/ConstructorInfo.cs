// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.PythonBinding
{
	public class PythonConstructorInfo
	{
		private ConstructorDeclaration constructor;
		private List<FieldDeclaration> fields = new List<FieldDeclaration>();
		private List<PropertyDeclaration> properties = new List<PropertyDeclaration>();

		private PythonConstructorInfo(ConstructorDeclaration constructor,
			List<FieldDeclaration> fields, List<PropertyDeclaration> properties)
		{
			this.constructor = constructor;
			this.fields = fields;
			this.properties = properties;
		}

		/// <summary>
		///     Gets the constructor information from a type declaration.
		/// </summary>
		/// <returns>Returns null if there is no constructor defined or 
		/// if there are no fields/properties defined.</returns>
		public static PythonConstructorInfo GetConstructorInfo(TypeDeclaration type)
		{
			List<FieldDeclaration> fields = new List<FieldDeclaration>();
			List<PropertyDeclaration> properties = new List<PropertyDeclaration>();
			ConstructorDeclaration constructor = null;
			foreach (INode node in type.Children) {
				ConstructorDeclaration currentConstructor = node as ConstructorDeclaration;
				FieldDeclaration field = node as FieldDeclaration;
				PropertyDeclaration property = node as PropertyDeclaration;
				if (currentConstructor != null) {
					constructor = currentConstructor;
				} else if (field != null) {
					fields.Add(field);
				} else if (property != null) {
					if (property.HasGetRegion && property.GetRegion.Block.IsNull
						&& property.HasSetRegion && property.SetRegion.Block.IsNull) {
						properties.Add(property); // basically anonymous backed property
					}
				}
			}

			if ((properties.Count > 0) || (fields.Count > 0) || (constructor != null)) {
				return new PythonConstructorInfo(constructor, fields, properties);
			}
			return null;
		}

		public ConstructorDeclaration Constructor {
			get { return constructor; }
		}

		public List<FieldDeclaration> Fields {
			get { return fields; }
		}

		public List<PropertyDeclaration> Properties {
			get { return properties; }
		}
	}
}