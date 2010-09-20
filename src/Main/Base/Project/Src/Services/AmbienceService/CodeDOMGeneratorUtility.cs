// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;

using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop
{
	public class CodeDOMGeneratorUtility 
	{
		public CodeGeneratorOptions CreateCodeGeneratorOptions {
			get {
				CodeGeneratorOptions options = new CodeGeneratorOptions();
				options.BlankLinesBetweenMembers = AmbienceService.CodeGenerationProperties.Get("BlankLinesBetweenMembers", true);
				options.BracingStyle             = AmbienceService.CodeGenerationProperties.Get("StartBlockOnSameLine", true) ? "Block" : "C";
				options.ElseOnClosing            = AmbienceService.CodeGenerationProperties.Get("ElseOnClosing", true);
				
				options.IndentString = EditorControlService.GlobalOptions.IndentationString;
				
				return options;
			}
		}
		
		public CodeTypeReference GetTypeReference(string type)
		{
			if (AmbienceService.UseFullyQualifiedNames) {
				return new CodeTypeReference(type);
			} else {
				string[] arr = type.Split('.');
				string shortName = arr[arr.Length - 1];
				if (type.Length - shortName.Length - 1 > 0) {
					string n = type.Substring(0, type.Length - shortName.Length - 1);
					namespaces[n] = "";
				}
				return new CodeTypeReference(shortName);
			}
		}
		
		public CodeTypeReference GetTypeReference(Type type)
		{
			if (AmbienceService.UseFullyQualifiedNames) {
				return new CodeTypeReference(type.FullName);
			} else {
				namespaces[type.Namespace] = "";
				return new CodeTypeReference(type.Name);
			}
		}
		
		public CodeTypeReferenceExpression GetTypeReferenceExpression(string type)
		{
			return new CodeTypeReferenceExpression(GetTypeReference(type));
		}

		public CodeTypeReferenceExpression GetTypeReferenceExpression(Type type)
		{
			return new CodeTypeReferenceExpression(GetTypeReference(type));
		}
		
		/// <summary>
		/// Adds a namespace import to the namespace import list.
		/// </summary>
		public void AddNamespaceImport(string ns)
		{
			namespaces[ns] = "";
		}
		
		/// <summary>
		/// Generates the namespace imports that caused of the usage of short type names
		/// </summary>
		public void GenerateNamespaceImports(CodeNamespace cnamespace)
		{
			foreach (string ns in namespaces.Keys) {
				cnamespace.Imports.Add(new CodeNamespaceImport(ns));
			}
		}
		
		Hashtable namespaces = new Hashtable();
	}	
}
