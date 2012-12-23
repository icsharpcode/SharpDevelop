// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeInterface : CodeType, global::EnvDTE.CodeInterface
	{
		string fullName;
		
		public CodeInterface(IProjectContent projectContent, IClass c)
			: base(projectContent, c)
		{
			fullName = base.FullName;
		}
		
		public CodeInterface(IProjectContent projectContent, IReturnType type, IClass c)
			: base(projectContent, c)
		{
			fullName = type.GetFullName();
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementInterface; }
		}
		
		/// <summary>
		/// Returns null if base type is not an interface.
		/// </summary>
		public static CodeInterface CreateFromBaseType(IProjectContent projectContent, IReturnType baseType)
		{
			IClass baseTypeClass = baseType.GetUnderlyingClass();
			if (baseTypeClass.ClassType == ClassType.Interface) {
				return new CodeInterface(projectContent, baseType, baseTypeClass);
			}
			return null;
		}
		
		public global::EnvDTE.CodeFunction AddFunction(string name, global::EnvDTE.vsCMFunction kind, object type, object Position = null, global::EnvDTE.vsCMAccess Access = global::EnvDTE.vsCMAccess.vsCMAccessPublic)
		{
			var codeGenerator = new ClassCodeGenerator(Class);
			return codeGenerator.AddPublicMethod(name, (string)type);
		}
		
		public override string FullName {
			get { return fullName; }
		}
	}
}
