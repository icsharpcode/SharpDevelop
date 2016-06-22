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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeClass : CodeType, global::EnvDTE.CodeClass
	{
		public CodeClass(CodeModelContext context, ITypeDefinition typeDefinition)
			: base(context, typeDefinition, typeDefinition.TypeArguments.ToArray())
		{
		}
		
		public CodeClass()
		{
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementClass; }
		}
		
		public virtual global::EnvDTE.CodeElements ImplementedInterfaces {
			get {
				var interfaces = new CodeElementsList<CodeType>();
				foreach (IType baseType in typeDefinition.DirectBaseTypes.Where(t => t.Kind == TypeKind.Interface)) {
					CodeType element = Create(context, baseType);
					if (element != null) {
						interfaces.Add(element);
					}
				}
				return interfaces;
			}
		}
		
		public virtual global::EnvDTE.CodeVariable AddVariable(string name, object type, object Position = null, global::EnvDTE.vsCMAccess Access = global::EnvDTE.vsCMAccess.vsCMAccessPublic, object Location = null)
		{
			IType fieldType = FindType((string)type);
			
			context.CodeGenerator.AddFieldAtStart(typeDefinition, Access.ToAccessibility(), fieldType, name);
			
			ReloadTypeDefinition();
			
			IField field = typeDefinition.Fields.FirstOrDefault(f => f.Name == name);
			if (field != null) {
				return new CodeVariable(context, field);
			}
			return null;
		}
	}
}
