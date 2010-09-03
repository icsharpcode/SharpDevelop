// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace Grunwald.BooBinding.Designer
{
	public class BooDesignerGenerator : AbstractDesignerGenerator
	{
		protected override string GenerateFieldDeclaration(CodeDOMGenerator domGenerator, CodeMemberField field)
		{
			// TODO: add support for modifiers
			// (or implement code generation for fields in the Boo CodeDomProvider)
			return "private " + field.Name + " as " + field.Type.BaseType;
		}
		
		protected override System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider()
		{
			return new Boo.Lang.CodeDom.BooCodeProvider();
		}
		
		protected override void FixGeneratedCode(IClass formClass, CodeMemberMethod code)
		{
			base.FixGeneratedCode(formClass, code);
			Dictionary<string, IReturnType> variables = new Dictionary<string, IReturnType>();
			foreach (IField f in formClass.DefaultReturnType.GetFields()) {
				variables[f.Name] = f.ReturnType;
			}
			variables[""] = formClass.DefaultReturnType;
			foreach (CodeStatement statement in code.Statements) {
				CodeExpressionStatement ces = statement as CodeExpressionStatement;
				if (ces != null) {
					CodeMethodInvokeExpression cmie = ces.Expression as CodeMethodInvokeExpression;
					if (cmie != null && cmie.Parameters.Count == 1) {
						CodeArrayCreateExpression cace = cmie.Parameters[0] as CodeArrayCreateExpression;
						if (cace != null) {
							IReturnType rt = ResolveType(cmie.Method.TargetObject, variables);
							if (rt != null) {
								foreach (IMethod m in rt.GetMethods()) {
									if (m.Name == cmie.Method.MethodName
									    && m.Parameters.Count == 1
									    && m.Parameters[0].IsParams
									    && m.Parameters[0].ReturnType.IsArrayReturnType)
									{
										ArrayReturnType paramArt = m.Parameters[0].ReturnType.CastToArrayReturnType();
										if (paramArt.ArrayDimensions == 1
										    && paramArt.FullyQualifiedName == cace.CreateType.BaseType)
										{
											cace.UserData["Explode"] = true;
										}
									}
								}
							}
						}
					}
				}
				CodeVariableDeclarationStatement cvds = statement as CodeVariableDeclarationStatement;
				if (cvds != null) {
					variables[cvds.Name] = new SearchClassReturnType(formClass.ProjectContent, formClass, formClass.Region.BeginLine + 1, 0, cvds.Type.BaseType, cvds.Type.TypeArguments.Count);
				}
			}
		}
		
		IReturnType ResolveType(CodeExpression expr, Dictionary<string, IReturnType> variables)
		{
			IReturnType rt;
			if (expr is CodeThisReferenceExpression) {
				return variables[""];
			} else if (expr is CodeVariableReferenceExpression) {
				string name = (expr as CodeVariableReferenceExpression).VariableName;
				if (variables.TryGetValue(name, out rt))
					return rt;
			} else if (expr is CodeFieldReferenceExpression) {
				string name = (expr as CodeFieldReferenceExpression).FieldName;
				rt = ResolveType((expr as CodeFieldReferenceExpression).TargetObject, variables);
				if (rt != null) {
					foreach (IField f in rt.GetFields()) {
						if (f.Name == name) {
							return f.ReturnType;
						}
					}
				}
			} else if (expr is CodePropertyReferenceExpression) {
				string name = (expr as CodePropertyReferenceExpression).PropertyName;
				rt = ResolveType((expr as CodePropertyReferenceExpression).TargetObject, variables);
				if (rt != null) {
					foreach (IProperty p in rt.GetProperties()) {
						if (p.Name == name) {
							return p.ReturnType;
						}
					}
				}
			}
			return null;
		}
		
		protected override string CreateEventHandler(Type eventType, string eventMethodName, string body, string indentation)
		{
			if (string.IsNullOrEmpty(body)) body = "pass";
			string param = GenerateParams(eventType);
			
			StringBuilder b = new StringBuilder();
			b.AppendLine(indentation);
			b.AppendLine(indentation + "private def " + eventMethodName + "(" + param + "):");
			if (string.IsNullOrEmpty(body)) {
				if (ICSharpCode.FormsDesigner.Gui.OptionPanels.GeneralOptionsPanel.InsertTodoComment) {
					body = "// TODO: Implement " + eventMethodName;
				}
			}
			b.AppendLine(indentation + "\t" + body);
			return b.ToString();
		}
		
		protected override DomRegion GetReplaceRegion(IDocument document, IMethod method)
		{
			DomRegion r = method.BodyRegion;
			return new DomRegion(r.BeginLine + 1, 1, r.EndLine + 1, 1);
		}
		
		protected override int GetEventHandlerInsertionLine(IClass c)
		{
			return c.Region.EndLine + 1;
		}
		
		protected static string GenerateParams(Type eventType)
		{
			MethodInfo mInfo = eventType.GetMethod("Invoke");
			string param = "";
			
			for (int i = 0; i < mInfo.GetParameters().Length; ++i)  {
				ParameterInfo pInfo  = mInfo.GetParameters()[i];
				
				param += pInfo.Name;
				param += " as ";
				
				string typeStr = pInfo.ParameterType.ToString();
				typeStr = new BooAmbience().GetIntrinsicTypeName(typeStr);
				param += typeStr;
				if (i + 1 < mInfo.GetParameters().Length) {
					param += ", ";
				}
			}
			return param;
		}
	}
}
