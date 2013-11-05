// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeClass : CodeType, global::EnvDTE.CodeClass
	{
		public CodeClass(CodeModelContext context, ITypeDefinitionModel typeModel)
			: base(context, typeModel)
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
				var list = new CodeElementsList<CodeType>();
				var td = typeModel.Resolve();
				if (td != null) {
					foreach (var baseType in td.GetAllBaseTypes().Where(t => t.Kind == TypeKind.Interface)) {
						CodeType element = Create(context, baseType);
						if (element != null)
							list.Add(element);
					}
				}
				return list;
			}
		}
		
		public virtual global::EnvDTE.CodeVariable AddVariable(string name, object type, object Position = null, global::EnvDTE.vsCMAccess Access = global::EnvDTE.vsCMAccess.vsCMAccessPublic, object Location = null)
		{
			var fieldTypeName = new FullTypeName((string)type);
			var td = typeModel.Resolve();
			if (td == null)
				return null;
			IType fieldType = td.Compilation.FindType(fieldTypeName);
			context.CodeGenerator.AddField(td, Access.ToAccessibility(), fieldType, name);
			var fieldModel = typeModel.Members.OfType<IFieldModel>().FirstOrDefault(f => f.Name == name);
			if (fieldModel != null) {
				return new CodeVariable(context, fieldModel);
			}
			return null;
		}
	}
}
