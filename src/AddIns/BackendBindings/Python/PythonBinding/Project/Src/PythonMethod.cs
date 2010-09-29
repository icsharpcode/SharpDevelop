// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;
using Microsoft.Scripting;

namespace ICSharpCode.PythonBinding
{
	public class PythonMethod : DefaultMethod
	{
		public PythonMethod(IClass declaringType, FunctionDefinition methodDefinition)
			: this(declaringType, methodDefinition, methodDefinition.Name)
		{
		}
		
		public PythonMethod(IClass declaringType, FunctionDefinition methodDefinition, string name)
			: base(declaringType, name)
		{
			ReturnType = new DefaultReturnType(declaringType);
			Modifiers = ModifierEnum.Public;
			
			GetMethodRegions(methodDefinition);
			AddParameters(methodDefinition);
			
			declaringType.Methods.Add(this);
		}
		
		void GetMethodRegions(FunctionDefinition methodDefinition)
		{
			GetBodyRegion(methodDefinition);
			GetMethodRegion(methodDefinition);
		}
		
		void GetBodyRegion(FunctionDefinition methodDefinition)
		{
			BodyRegion = PythonMethodOrClassBodyRegion.GetBodyRegion(methodDefinition);
		}
		
		/// <summary>
		/// Gets the region of a method. This does not include the body.
		/// </summary>
		void GetMethodRegion(FunctionDefinition methodDefinition)
		{
			SourceLocation start = methodDefinition.Start;
			SourceLocation end = methodDefinition.Header;
			Region = new DomRegion(start.Line, start.Column, end.Line, end.Column + 1);
		}
		
		void AddParameters(FunctionDefinition methodDefinition)
		{
			bool ignoreFirstMethodParameter = !DeclaringTypeIsPythonModule;
			AddParameters(methodDefinition, ignoreFirstMethodParameter);
		}
		
		bool DeclaringTypeIsPythonModule {
			get { return DeclaringType is PythonModule; }
		}
		
		void AddParameters(FunctionDefinition methodDefinition, bool ignoreFirstMethodParameter)
		{
			foreach (IParameter parameter in ConvertParameters(methodDefinition.Parameters, ignoreFirstMethodParameter)) {
				Parameters.Add(parameter);
			}
		}
		
		/// <summary>
		/// Converts from Python AST expressions to parameters.
		/// </summary>
		/// <remarks>If the parameters belong to a class method then the first
		/// "self" parameter can be ignored.</remarks>
		IParameter[] ConvertParameters(IList<Parameter> parameters, bool ignoreFirstParameter)
		{
			int startingIndex = GetStartingIndex(ignoreFirstParameter);
			return ConvertParameters(parameters, startingIndex);
		}
		
		int GetStartingIndex(bool ignoreFirstParameter)
		{
			if (ignoreFirstParameter) {
				return 1;
			}
			return 0;
		}
		
		IParameter[] ConvertParameters(IList<Parameter> parameters, int startingIndex)
		{
			List<IParameter> convertedParameters = new List<IParameter>();
			for (int i = startingIndex; i < parameters.Count; ++i) {				
				DefaultParameter parameter = new DefaultParameter(parameters[i].Name, null, new DomRegion());
				convertedParameters.Add(parameter);
			}
			return convertedParameters.ToArray();
		}
	}
}
