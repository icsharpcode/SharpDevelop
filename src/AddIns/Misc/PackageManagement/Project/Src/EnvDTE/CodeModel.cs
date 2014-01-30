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
//	public class CodeModel : MarshalByRefObject, global::EnvDTE.CodeModel
//	{
//		IProjectContent projectContent;
//		CodeElementsInNamespace codeElements;
//		
//		public CodeModel(IProjectContent projectContent)
//		{
//			this.projectContent = projectContent;
//		}
//		
//		public global::EnvDTE.CodeElements CodeElements {
//			get {
//				if (codeElements == null) {
//					codeElements = new CodeElementsInNamespace(projectContent, String.Empty);
//				}
//				return codeElements;
//			}
//		}
//		
//		public global::EnvDTE.CodeType CodeTypeFromFullName(string name)
//		{
//			IClass matchedClass = projectContent.GetClass(name, 0);
//			if (matchedClass != null) {
//				return CreateCodeTypeForClass(matchedClass);
//			}
//			return null;
//		}
//		
//		CodeType CreateCodeTypeForClass(IClass c)
//		{
//			if (c.ClassType == ClassType.Interface) {
//				return new CodeInterface(projectContent, c);
//			}
//			return new CodeClass2(projectContent, c);
//		}
//		
//		public string Language {
//			get { return projectContent.GetCodeModelLanguage(); }
//		}
//	}
//}
