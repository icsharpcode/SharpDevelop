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

//using System;
//using ICSharpCode.SharpDevelop.Dom;
//
//namespace ICSharpCode.PackageManagement.EnvDTE
//{
//	public class CodeInterface : CodeType, global::EnvDTE.CodeInterface
//	{
//		string fullName;
//		
//		public CodeInterface(IProjectContent projectContent, IClass c)
//			: base(projectContent, c)
//		{
//			fullName = base.FullName;
//		}
//		
//		public CodeInterface(IProjectContent projectContent, IReturnType type, IClass c)
//			: base(projectContent, c)
//		{
//			fullName = type.GetFullName();
//		}
//		
//		public override global::EnvDTE.vsCMElement Kind {
//			get { return global::EnvDTE.vsCMElement.vsCMElementInterface; }
//		}
//		
//		/// <summary>
//		/// Returns null if base type is not an interface.
//		/// </summary>
//		public static CodeInterface CreateFromBaseType(IProjectContent projectContent, IReturnType baseType)
//		{
//			IClass baseTypeClass = baseType.GetUnderlyingClass();
//			if (baseTypeClass.ClassType == ClassType.Interface) {
//				return new CodeInterface(projectContent, baseType, baseTypeClass);
//			}
//			return null;
//		}
//		
//		public global::EnvDTE.CodeFunction AddFunction(string name, global::EnvDTE.vsCMFunction kind, object type, object Position = null, global::EnvDTE.vsCMAccess Access = global::EnvDTE.vsCMAccess.vsCMAccessPublic)
//		{
//			var codeGenerator = new ClassCodeGenerator(Class);
//			return codeGenerator.AddPublicMethod(name, (string)type);
//		}
//		
//		public override string FullName {
//			get { return fullName; }
//		}
//	}
//}
