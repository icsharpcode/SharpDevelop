// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class SignatureComparer : IEqualityComparer<IMember>
	{
		ParameterListComparer parameterListComparer = new ParameterListComparer();
		
		public bool Equals(IMember x, IMember y)
		{	
			if (x.EntityType != y.EntityType)
				return false;
			
			if (x.Name != y.Name)
				return false;
			
			if (x is IMethod && y is IMethod)
				return parameterListComparer.Equals(x as IMethod, y as IMethod);
			
			return true;
		}
		
		public int GetHashCode(IMember obj)
		{
			int hashCode = obj.Name.GetHashCode();
			
			if (obj is IMethod)
				hashCode ^= parameterListComparer.GetHashCode(obj as IMethod);
			
			return hashCode;
		}
	}
}
