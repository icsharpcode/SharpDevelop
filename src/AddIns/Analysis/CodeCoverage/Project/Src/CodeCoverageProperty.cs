// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageProperty
	{
		string name = String.Empty;
		CodeCoverageMethod getter;
		CodeCoverageMethod setter;
		
		public CodeCoverageProperty()
		{
		}
		
		public CodeCoverageProperty(CodeCoverageMethod method)
		{
			AddMethod(method);
		}
		
		public List<CodeCoverageMethod> GetMethods()
		{
			List<CodeCoverageMethod> methods = new List<CodeCoverageMethod>();
			if (getter != null) {
				methods.Add(getter);
			}
			if (setter != null) {
				methods.Add(setter);
			}
			return methods;
		}
		
		/// <summary>
		/// Gets the property name.
		/// </summary>
		public string Name {
			get { return name; }
		}
		
		public CodeCoverageMethod Getter {
			get { return getter; }
		}
		
		public CodeCoverageMethod Setter {
			get { return setter; }
		}
		
		/// <summary>
		/// Adds a getter or setter to the property.
		/// </summary>
		public void AddMethod(CodeCoverageMethod method)
		{
			name = GetPropertyName(method);
			if (method.IsGetter) {
				getter = method;
			} else {
				setter = method;
			}
		}
		
		/// <summary>
		/// Strips the get_ or set_ part from a method name and returns the property name.
		/// </summary>
		public static string GetPropertyName(CodeCoverageMethod method)
		{
			if (method.IsProperty) {
				return method.Name.Substring(4);
			}
			return String.Empty;
		}
	}
}
