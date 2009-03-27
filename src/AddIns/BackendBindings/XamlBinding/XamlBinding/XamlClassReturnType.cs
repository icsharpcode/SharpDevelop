// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 2569 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlClassReturnType.
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
