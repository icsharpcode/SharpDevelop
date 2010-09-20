// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
