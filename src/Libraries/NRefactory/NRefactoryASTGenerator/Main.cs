// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.CodeDom;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using NRefactoryASTGenerator.AST;

namespace NRefactoryASTGenerator
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			string directory = "../../../Project/Src/Parser/AST/";
			string visitorsDir = "../../../Project/Src/Parser/Visitors/";
			Debug.WriteLine("AST Generator running...");
			if (!File.Exists(directory + "INode.cs")) {
				Debug.WriteLine("did not find output directory");
				return;
			}
			if (!File.Exists(visitorsDir + "IASTVisitor.cs")) {
				Debug.WriteLine("did not find visitor output directory");
				return;
			}
			
			List<Type> nodeTypes = new List<Type>();
			foreach (Type type in typeof(MainClass).Assembly.GetTypes()) {
				if (type.IsClass && typeof(INode).IsAssignableFrom(type)) {
					nodeTypes.Add(type);
				}
			}
			nodeTypes.Sort(delegate(Type a, Type b) { return a.Name.CompareTo(b.Name); });
			
			CodeCompileUnit ccu = new CodeCompileUnit();
			CodeNamespace cns = new CodeNamespace("ICSharpCode.NRefactory.Parser.AST");
			ccu.Namespaces.Add(cns);
			cns.Imports.Add(new CodeNamespaceImport("System"));
			cns.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
			cns.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
			cns.Imports.Add(new CodeNamespaceImport("System.Drawing"));
			foreach (Type type in nodeTypes) {
				if (type.GetCustomAttributes(typeof(CustomImplementationAttribute), false).Length == 0) {
					CodeTypeDeclaration ctd = new CodeTypeDeclaration(type.Name);
					if (type.IsAbstract) {
						ctd.TypeAttributes |= TypeAttributes.Abstract;
					}
					ctd.BaseTypes.Add(new CodeTypeReference(type.BaseType.Name));
					cns.Types.Add(ctd);
					
					ProcessType(type, ctd);
					
					foreach (object o in type.GetCustomAttributes(false)) {
						if (o is TypeImplementationModifierAttribute) {
							(o as TypeImplementationModifierAttribute).ModifyImplementation(cns, ctd, type);
						}
					}
					
					if (!type.IsAbstract) {
						CodeMemberMethod method = new CodeMemberMethod();
						method.Name = "AcceptVisitor";
						method.Attributes = MemberAttributes.Public | MemberAttributes.Override;
						method.Parameters.Add(new CodeParameterDeclarationExpression("IAstVisitor", "visitor"));
						method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "data"));
						method.ReturnType = new CodeTypeReference(typeof(object));
						CodeExpression ex = new CodeVariableReferenceExpression("visitor");
						ex = new CodeMethodInvokeExpression(ex, "Visit",
						                                    new CodeThisReferenceExpression(),
						                                    new CodeVariableReferenceExpression("data"));
						method.Statements.Add(new CodeMethodReturnStatement(ex));
						ctd.Members.Add(method);
						
						method = new CodeMemberMethod();
						method.Name = "ToString";
						method.Attributes = MemberAttributes.Public | MemberAttributes.Override;
						method.ReturnType = new CodeTypeReference(typeof(string));
						method.Statements.Add(new CodeMethodReturnStatement(CreateToString(type)));
						ctd.Members.Add(method);
					}
				}
			}
			
			using (StringWriter writer = new StringWriter()) {
				new Microsoft.CSharp.CSharpCodeProvider().GenerateCodeFromCompileUnit(ccu, writer, null);
				File.WriteAllText(directory + "Generated.cs", writer.ToString());
			}
		}
		
		static CodeExpression CreateToString(Type type)
		{
			CodeMethodInvokeExpression ie = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)),
			                                                               "Format");
			CodePrimitiveExpression prim = new CodePrimitiveExpression();
			ie.Parameters.Add(prim);
			string text = "[" + type.Name;
			int index = 0;
			do {
				foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
					text += " " + GetPropertyName(field.Name) + "={" + index.ToString() + "}";
					index++;
					if (typeof(System.Collections.ICollection).IsAssignableFrom(field.FieldType)) {
						ie.Parameters.Add(new CodeSnippetExpression("GetCollectionString(" + GetPropertyName(field.Name) + ")"));
					} else {
						ie.Parameters.Add(new CodeVariableReferenceExpression(GetPropertyName(field.Name)));
					}
				}
				type = type.BaseType;
			} while (type != null);
			prim.Value = text + "]";
			if (ie.Parameters.Count == 1)
				return prim;
			else
				return ie;
			//	return String.Format("[AnonymousMethodExpression: Parameters={0} Body={1}]",
			//	                     GetCollectionString(Parameters),
			//	                     Body);
		}
		
		static void ProcessType(Type type, CodeTypeDeclaration ctd)
		{
			foreach (FieldInfo field in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic)) {
				CodeMemberField f = new CodeMemberField(ConvertType(field.FieldType), field.Name);
				f.Attributes = 0;
				ctd.Members.Add(f);
			}
			foreach (FieldInfo field in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic)) {
				CodeMemberProperty p = new CodeMemberProperty();
				p.Name = GetPropertyName(field.Name);
				p.Attributes = MemberAttributes.Public | MemberAttributes.Final;
				p.Type = ConvertType(field.FieldType);
				p.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(field.Name)));
				CodeExpression ex;
				if (field.FieldType.IsValueType)
					ex = new CodePropertySetValueReferenceExpression();
				else
					ex = GetDefaultValue("value", field);
				p.SetStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(field.Name), ex));
				ctd.Members.Add(p);
			}
			foreach (ConstructorInfo ctor in type.GetConstructors()) {
				CodeConstructor c = new CodeConstructor();
				c.Attributes = MemberAttributes.Public;
				ctd.Members.Add(c);
				ConstructorInfo baseCtor = GetBaseCtor(type);
				foreach(ParameterInfo param in ctor.GetParameters()) {
					c.Parameters.Add(new CodeParameterDeclarationExpression(ConvertType(param.ParameterType),
					                                                        param.Name));
					if (baseCtor != null && Array.Exists(baseCtor.GetParameters(), delegate(ParameterInfo p) { return param.Name == p.Name; }))
						continue;
					c.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(GetPropertyName(param.Name)),
					                                         new CodeVariableReferenceExpression(param.Name)));
				}
				if (baseCtor != null) {
					foreach(ParameterInfo param in baseCtor.GetParameters()) {
						c.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(param.Name));
					}
				}
				// initialize fields that were not initialized by parameter
				foreach (FieldInfo field in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic)) {
					if (field.FieldType.IsValueType && field.FieldType != typeof(Point))
						continue;
					if (Array.Exists(ctor.GetParameters(), delegate(ParameterInfo p) { return field.Name == p.Name; }))
						continue;
					c.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(field.Name),
					                                         GetDefaultValue(null, field)));
				}
			}
		}
		
		internal static ConstructorInfo GetBaseCtor(Type type)
		{
			ConstructorInfo[] list = type.BaseType.GetConstructors();
			if (list.Length == 0)
				return null;
			else
				return list[0];
		}
		
		internal static CodeExpression GetDefaultValue(string inputVariable, FieldInfo field)
		{
			string code;
			// get default value:
			if (field.FieldType == typeof(string)) {
				code = "\"\"";
				if (field.GetCustomAttributes(typeof(QuestionMarkDefaultAttribute), false).Length > 0) {
					if (inputVariable == null)
						return new CodePrimitiveExpression("?");
					else
						return new CodeSnippetExpression("string.IsNullOrEmpty(" + inputVariable + ") ? \"?\" : " + inputVariable);
				}
			} else if (field.FieldType.FullName.StartsWith("System.Collections.Generic.List")) {
				code = "new List<" + field.FieldType.GetGenericArguments()[0].Name + ">()";
			} else if (field.FieldType == typeof(Point)) {
				code = "new Point(-1, -1)";
			} else {
				code = field.FieldType.Name + ".Null";
			}
			if (inputVariable != null) {
				code = inputVariable + " ?? " + code;
			}
			return new CodeSnippetExpression(code);
		}
		
		internal static string GetPropertyName(string fieldName)
		{
			return char.ToUpper(fieldName[0]) + fieldName.Substring(1);
		}
		
		internal static CodeTypeReference ConvertType(Type type)
		{
			if (type.IsGenericType && !type.IsGenericTypeDefinition) {
				CodeTypeReference tr = ConvertType(type.GetGenericTypeDefinition());
				foreach (Type subType in type.GetGenericArguments()) {
					tr.TypeArguments.Add(ConvertType(subType));
				}
				return tr;
			} else if (type.FullName.StartsWith("NRefactory") || type.FullName.StartsWith("System.Collections")) {
				return new CodeTypeReference(type.Name);
			} else {
				return new CodeTypeReference(type);
			}
		}
	}
}
