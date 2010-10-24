// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.FormsDesigner.Services
{
	public struct AssemblyInfo
	{
		public readonly string FullNameOrPath;
		public readonly bool IsInGac;
		
		public AssemblyInfo(string fullNameOrPath, bool isInGac)
		{
			this.FullNameOrPath = fullNameOrPath;
			this.IsInGac = isInGac;
		}
	}
	
	public interface ITypeLocator
	{
		AssemblyInfo LocateType(string name, out AssemblyInfo[] referencedAssemblies);
	}
	
	public interface IGacWrapper
	{
		bool IsGacAssembly(string path);
	}
}
