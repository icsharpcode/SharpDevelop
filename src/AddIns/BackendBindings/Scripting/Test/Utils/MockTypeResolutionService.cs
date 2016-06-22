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
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockTypeResolutionService : ITypeResolutionService
	{
		string lastTypeNameResolved;
		
		public MockTypeResolutionService()
		{
		}
		
		/// <summary>
		/// Returns the last type name passed to the GetType method.
		/// </summary>
		public string LastTypeNameResolved {
			get { return lastTypeNameResolved; }
		}
		
		public Assembly GetAssembly(AssemblyName name)
		{
			throw new NotImplementedException();
		}
		
		public Assembly GetAssembly(AssemblyName name, bool throwOnError)
		{
			throw new NotImplementedException();
		}
		
		public Type GetType(string name)
		{
			lastTypeNameResolved = name;
			if (name == "Form") {
				return typeof(Form);
			} 
			
			Type type = typeof(Form).Assembly.GetType(name, false);
			if (type != null) {
				return type;
			}
			
			type = typeof(Color).Assembly.GetType(name, false);
			if (type != null) {
				return type;
			}
			
			return Type.GetType(name);
		}
		
		public Type GetType(string name, bool throwOnError)
		{
			throw new NotImplementedException();
		}
		
		public Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			throw new NotImplementedException();
		}
		
		public void ReferenceAssembly(AssemblyName name)
		{
		}
		
		public string GetPathOfAssembly(AssemblyName name)
		{
			throw new NotImplementedException();
		}
	}
}
