// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Xml;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpAssembly.Metadata.Rows;
using ICSharpCode.SharpAssembly.Metadata;
using ICSharpCode.SharpAssembly.PE;
using ICSharpCode.SharpAssembly.Assembly;
using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	[Serializable]
	public class SharpAssemblyParameter : DefaultParameter
	{
		public SharpAssemblyParameter(SA.SharpAssembly asm, Param[] paramTable, uint index, IReturnType type) : base(String.Empty)
		{
			if (asm == null) {
				throw new System.ArgumentNullException("asm");
			}
			if (paramTable == null) {
				throw new System.ArgumentNullException("paramTable");
			}
			if (index > paramTable.GetUpperBound(0) || index < 1) {
				throw new System.ArgumentOutOfRangeException("index", index, String.Format("must be between 1 and {0}!", paramTable.GetUpperBound(0)));
			}
			AssemblyReader assembly = asm.Reader;
			
			Param param = asm.Tables.Param[index];
			
			Name = assembly.GetStringFromHeap(param.Name);

			if (param.IsFlagSet(Param.FLAG_OUT)) {
				Modifiers |= ParameterModifiers.Out;
			}
			
			// Attributes
			ArrayList attrib = asm.Attributes.Param[index] as ArrayList;
			if (attrib == null) goto noatt;
			
			foreach(SharpCustomAttribute customattribute in attrib) {
				SharpAssemblyAttribute newatt = new SharpAssemblyAttribute(asm, customattribute);
				if (newatt.Name == "System.ParamArrayAttribute") Modifiers |= ParameterModifiers.Params;
				Attributes.Add(newatt);
			}
		
		noatt:
			
			if (type == null) {
				returnType = SharpAssemblyReturnType.Create("PARAMETER_UNKNOWN");
			} else {
				if (type.Name.EndsWith("&")) {
					Modifiers |= ParameterModifiers.Ref;
				}
				returnType = type;
			}
			
		}
		
		public SharpAssemblyParameter(SA.SharpAssembly asm, string paramName, IReturnType type) : base(String.Empty)
		{
			Name = paramName;
 			if (type.Name.EndsWith("&")) {
				Modifiers |= ParameterModifiers.Ref;
			}
			returnType = type;
		}
		
		public override string ToString()
		{
			return "Parameter : " + returnType.FullyQualifiedName;
		}
	}
}
