// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Return type used for MethodGroupResolveResult.
	/// </summary>
	public class MethodGroupReturnType : AbstractReturnType
	{
		public MethodGroupReturnType()
		{
		}
		
		public override IClass GetUnderlyingClass()
		{
			return null;
		}
		
		public override List<IMethod> GetMethods()
		{
			return new List<IMethod>();
		}
		
		public override List<IProperty> GetProperties()
		{
			return new List<IProperty>();
		}
		
		public override List<IField> GetFields()
		{
			return new List<IField>();
		}
		
		public override List<IEvent> GetEvents()
		{
			return new List<IEvent>();
		}
	}
}
