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

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class NamespaceName
	{
		public NamespaceName(string parentNamespace, string name)
			: this(GetQualifiedNamespaceName(parentNamespace, name))
		{
		}
		
		static string GetQualifiedNamespaceName(string parentNamespace, string name)
		{
			if (String.IsNullOrEmpty(parentNamespace)) {
				return name;
			}
			return String.Format("{0}.{1}", parentNamespace, name);
		}
		
		public NamespaceName(string qualifiedName)
		{
			this.QualifiedName = qualifiedName;
			LastPart = GetLastPartOfNamespace();
		}
		
		string GetLastPartOfNamespace()
		{
			int index = QualifiedName.LastIndexOf('.');
			return QualifiedName.Substring(index + 1);
		}
		
		public string LastPart { get; private set; }
		public string QualifiedName { get; private set; }
		
		public NamespaceName CreateChildNamespaceName(string name)
		{
			return new NamespaceName(QualifiedName, name);
		}
	}
}
