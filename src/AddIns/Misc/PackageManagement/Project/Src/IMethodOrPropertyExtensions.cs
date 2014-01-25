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
//namespace ICSharpCode.PackageManagement
//{
//	public static class IMethodOrPropertyExtensions
//	{
//		public static FilePosition GetStartPosition(this IMethodOrProperty method)
//		{
//			return method.Region.ToStartPosition(method.CompilationUnit);
//		}
//		
//		public static FilePosition GetEndPosition(this IMethodOrProperty method)
//		{
//			if (method.DeclaringTypeIsInterface()) {
//				return method.Region.ToEndPosition(method.CompilationUnit);
//			}
//			return method.BodyRegion.ToEndPosition(method.CompilationUnit);
//		}
//		
//		public static bool DeclaringTypeIsInterface(this IMethodOrProperty method)
//		{
//			return method.DeclaringType.ClassType == ClassType.Interface;
//		}
//		
//		public static bool IsConstructor(this IMethodOrProperty methodOrProperty)
//		{
//			var method = methodOrProperty as IMethod;
//			if (method != null) {
//				return method.IsConstructor;
//			}
//			return false;
//		}
//		
//		public static bool HasTypeParameters(this IMethodOrProperty methodOrProperty)
//		{
//			var method = methodOrProperty as IMethod;
//			if ((method != null) && (method.TypeParameters != null)) {
//				return method.TypeParameters.Count > 0;
//			}
//			return false;
//		}
//	}
//}
