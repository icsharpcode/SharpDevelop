// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.PrettyPrinter;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class NRefactoryInformationProvider : IEnvironmentInformationProvider
	{
		IProjectContent pc;
		IClass callingClass;
		
		public NRefactoryInformationProvider(IProjectContent pc, IClass callingClass)
		{
			this.pc = pc;
			this.callingClass = callingClass;
		}
		
		public bool HasField(string fullTypeName, string fieldName)
		{
			IClass c = pc.GetClass(fullTypeName);
			if (c == null)
				return false;
			foreach (IField field in c.DefaultReturnType.GetFields()) {
				if (field.Name == fieldName)
					return true;
			}
			return false;
		}
	}
}
