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
