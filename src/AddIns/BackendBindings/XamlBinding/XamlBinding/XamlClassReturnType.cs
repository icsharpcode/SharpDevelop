// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// IReturnType that gets created by XamlCompilationUnit.CreateType and will
	/// run XamlCompilationUnit.FindType on demand.
	/// </summary>
	public class XamlClassReturnType : ProxyReturnType
	{
		XamlCompilationUnit compilationUnit;
		string xmlNamespace;
		string className;

		public XamlClassReturnType(XamlCompilationUnit compilationUnit, string xmlNamespace, string className)
		{
			if (compilationUnit == null)
				throw new ArgumentNullException("compilationUnit");

			this.compilationUnit = compilationUnit;
			this.xmlNamespace = xmlNamespace;
			this.className = className ?? "";
		}

		public override IReturnType BaseType
		{
			get
			{
				return compilationUnit.FindType(xmlNamespace, className);
			}
		}

		public override string Name
		{
			get { return className; }
		}
	}
}
