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
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

using ICSharpCode.Scripting;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Used to generate Python code after the form has been changed in the designer.
	/// </summary>
	public class PythonCodeDomSerializer : IScriptingCodeDomSerializer
	{
		PythonCodeBuilder codeBuilder;
		string indentString = String.Empty;
		string rootResourceName = String.Empty;
		IDesignerSerializationManager serializationManager;
		
		public PythonCodeDomSerializer() 
			: this("\t")
		{
		}
		
		public PythonCodeDomSerializer(string indentString)
		{
			this.indentString = indentString;
		}
				
		public string GenerateInitializeComponentMethodBody(IDesignerHost host, IDesignerSerializationManager serializationManager)
		{
			return GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, 0);
		}

		public string GenerateInitializeComponentMethodBody(IDesignerHost host, IDesignerSerializationManager serializationManager, string rootNamespace, int initialIndent)
		{
			codeBuilder = new PythonCodeBuilder(initialIndent);
			codeBuilder.IndentString = indentString;

			CodeMemberMethod method = FindInitializeComponentMethod(host, serializationManager);
			GetResourceRootName(rootNamespace, host.RootComponent);
			AppendStatements(method.Statements);

			return codeBuilder.ToString();
		}
		
		CodeMemberMethod FindInitializeComponentMethod(IDesignerHost host, IDesignerSerializationManager serializationManager)
		{
			this.serializationManager = serializationManager;
			object rootComponent = host.RootComponent;
			TypeCodeDomSerializer serializer = serializationManager.GetSerializer(rootComponent.GetType(), typeof(TypeCodeDomSerializer)) as TypeCodeDomSerializer;
			CodeTypeDeclaration typeDec = serializer.Serialize(serializationManager, rootComponent, host.Container.Components) as CodeTypeDeclaration;
			foreach (CodeTypeMember member in typeDec.Members) {
				CodeMemberMethod method = member as CodeMemberMethod;
				if (method != null) {
					if (method.Name == "InitializeComponent") {
						return method;
					}
				}
			}
			return null;			
		}
		
		void AppendStatements(CodeStatementCollection statements)
		{
			foreach (CodeStatement statement in statements) {
				AppendStatement(statement);
			}
		}
		
		void AppendStatement(CodeStatement statement)
		{
			if (statement is CodeExpressionStatement) {
			 	AppendExpressionStatement((CodeExpressionStatement)statement);
			} else if (statement is CodeCommentStatement) {
				AppendCommentStatement((CodeCommentStatement)statement);
			} else if (statement is CodeAssignStatement) {
				AppendAssignStatement((CodeAssignStatement)statement);
			} else if (statement is CodeVariableDeclarationStatement) {
				AppendVariableDeclarationStatement((CodeVariableDeclarationStatement)statement);
			} else if (statement is CodeAttachEventStatement) {
				AppendAttachEventStatement((CodeAttachEventStatement)statement);
			} else {
				Console.WriteLine("AppendStatement: " + statement.GetType().Name);
			}
		}
		
		void AppendExpressionStatement(CodeExpressionStatement statement)
		{
			codeBuilder.AppendIndented(String.Empty);
			AppendExpression(statement.Expression);
			codeBuilder.AppendLine();
		}

		void AppendCommentStatement(CodeCommentStatement statement)
		{
			codeBuilder.AppendIndented(String.Empty);
			codeBuilder.Append("# ");
			codeBuilder.Append(statement.Comment.Text);
			codeBuilder.AppendLine();
		}
		
		void AppendExpression(CodeExpression expression)
		{
			if (expression is CodeMethodInvokeExpression) {
				AppendMethodInvokeExpression((CodeMethodInvokeExpression)expression);
			} else if (expression is CodePropertyReferenceExpression) {
				AppendPropertyReferenceExpression((CodePropertyReferenceExpression)expression);
			} else if (expression is CodeObjectCreateExpression) {
				AppendObjectCreateExpression((CodeObjectCreateExpression)expression);
			} else if (expression is CodePrimitiveExpression) {
				AppendPrimitiveExpression((CodePrimitiveExpression)expression);
			} else if (expression is CodeFieldReferenceExpression) {
				AppendFieldReferenceExpression((CodeFieldReferenceExpression)expression);
			} else if (expression is CodeThisReferenceExpression) {
				AppendThisReferenceExpression();
			} else if (expression is CodeTypeReferenceExpression) {
				AppendTypeReferenceExpression((CodeTypeReferenceExpression)expression);
			} else if (expression is CodeArrayCreateExpression) {
				AppendArrayCreateExpression((CodeArrayCreateExpression)expression);
			} else if (expression is CodeVariableReferenceExpression) {
				AppendVariableReferenceExpression((CodeVariableReferenceExpression)expression);
			} else if (expression is CodeDelegateCreateExpression) {
				AppendDelegateCreateExpression((CodeDelegateCreateExpression)expression);
			} else if (expression is CodeCastExpression) {
				AppendCastExpression((CodeCastExpression)expression);
			} else if (expression is CodeBinaryOperatorExpression) {
				AppendBinaryOperatorExpression((CodeBinaryOperatorExpression)expression);
			} else {
				Console.WriteLine("AppendExpression: " + expression.GetType().Name);
			}
		}
		
		/// <summary>
		/// Appends a method call (e.g. "self.SuspendLayout()");
		/// </summary>
		void AppendMethodInvokeExpression(CodeMethodInvokeExpression expression)
		{
			AppendMethodReferenceExpression(expression.Method);
			AppendParameters(expression.Parameters);
		}

		void AppendMethodReferenceExpression(CodeMethodReferenceExpression expression)
		{
			AppendExpression(expression.TargetObject);
			codeBuilder.Append(".");
			codeBuilder.Append(expression.MethodName);
		}
		
		void AppendParameters(CodeExpressionCollection parameters)
		{
			codeBuilder.Append("(");
			bool firstParameter = true;
			foreach (CodeExpression expression in parameters) {
				if (firstParameter) {
					firstParameter = false;					
				} else {
					codeBuilder.Append(", ");
				}
				AppendExpression(expression);
			}
			codeBuilder.Append(")");
		}
		
		void AppendAssignStatement(CodeAssignStatement statement)
		{
			codeBuilder.AppendIndented(String.Empty);
			AppendExpression(statement.Left);
			codeBuilder.Append(" = ");
			AppendExpression(statement.Right);
			codeBuilder.AppendLine();
		}
		
		void AppendPropertyReferenceExpression(CodePropertyReferenceExpression expression)
		{
			AppendExpression(expression.TargetObject);
			codeBuilder.Append(".");
			codeBuilder.Append(expression.PropertyName);
		}
		
		void AppendObjectCreateExpression(CodeObjectCreateExpression expression)
		{
			AppendTypeReference(expression.CreateType);
			AppendParameters(expression.Parameters);
		}
		
		/// <summary>
		/// Appends a constant (e.g. string or int).
		/// </summary>
		void AppendPrimitiveExpression(CodePrimitiveExpression expression)
		{
			codeBuilder.Append(PythonPropertyValueAssignment.ToString(expression.Value));
		}
		
		void AppendFieldReferenceExpression(CodeFieldReferenceExpression expression)
		{
			AppendExpression(expression.TargetObject);
			codeBuilder.Append(".");
			if (expression.FieldName != null) {
				if (expression.TargetObject is CodeThisReferenceExpression) {
					if (!IsInherited(expression.FieldName)) {
						codeBuilder.Append("_");
					}
				}
				codeBuilder.Append(expression.FieldName);
			}
		}
		
		void AppendThisReferenceExpression()
		{
			codeBuilder.Append("self");
		}
		
		void AppendTypeReferenceExpression(CodeTypeReferenceExpression expression)
		{
			AppendTypeReference(expression.Type);
		}
		
		void AppendTypeReference(CodeTypeReference typeRef)
		{
			string typeRefText = typeRef.BaseType.Replace('+', '.');
			codeBuilder.Append(typeRefText);
		}
		
		/// <summary>
		/// Creates an array expression:
		/// 
		/// (System.Array[System.Object](\r\n" +
		///	        ["aaa",
		///         "bbb",
		///	        "ccc\"])) 
		/// </summary>
		void AppendArrayCreateExpression(CodeArrayCreateExpression expression)
		{
			codeBuilder.Append("System.Array[");
			AppendTypeReference(expression.CreateType);
			codeBuilder.Append("]");
			
			AppendInitializers(expression.Initializers);
		}
		
		/// <summary>
		/// Appends initializers for an array.
		/// </summary>
		void AppendInitializers(CodeExpressionCollection initalizers)
		{
			codeBuilder.Append("(");
			codeBuilder.AppendLine();
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndented("[");

			bool firstInitializer = true;
			foreach (CodeExpression expression in initalizers) {
				if (firstInitializer) {
					firstInitializer = false;					
				} else {
					codeBuilder.Append(",");
					codeBuilder.AppendLine();
					codeBuilder.AppendIndented(String.Empty);
				}
				AppendExpression(expression);
			}
			
			codeBuilder.Append("])");
			codeBuilder.DecreaseIndent();
		}
		
		/// <summary>
		/// Appends a local variable declaration.
		/// </summary>
		void AppendVariableDeclarationStatement(CodeVariableDeclarationStatement statement)
		{
			if (statement.Name == "resources") {
				codeBuilder.AppendIndented("resources = System.Resources.ResourceManager(\"");
				codeBuilder.Append(rootResourceName);
				codeBuilder.Append("\", System.Reflection.Assembly.GetEntryAssembly())");
				codeBuilder.AppendLine();
			} else {
				codeBuilder.AppendIndented(statement.Name);
				codeBuilder.Append(" = ");
				AppendExpression(statement.InitExpression);
				codeBuilder.AppendLine();
			}
		}
		
		void AppendVariableReferenceExpression(CodeVariableReferenceExpression expression)
		{
			codeBuilder.Append(expression.VariableName);
		}
		
		void AppendAttachEventStatement(CodeAttachEventStatement statement)
		{
			codeBuilder.AppendIndented(String.Empty);
			AppendExpression(statement.Event.TargetObject);
			codeBuilder.Append(".");
			codeBuilder.Append(statement.Event.EventName);
			
			codeBuilder.Append(" += ");
			
			AppendExpression(statement.Listener);
			
			codeBuilder.AppendLine();
		}
		
		void AppendDelegateCreateExpression(CodeDelegateCreateExpression expression)
		{
			AppendExpression(expression.TargetObject);
			codeBuilder.Append(".");
			codeBuilder.Append(expression.MethodName);
		}
		
		void GetResourceRootName(string rootNamespace, IComponent component)
		{
			rootResourceName = component.Site.Name;
			if (!String.IsNullOrEmpty(rootNamespace)) {
				rootResourceName = rootNamespace + "." + rootResourceName;
			}
		}
		
		void AppendCastExpression(CodeCastExpression expression)
		{
			AppendExpression(expression.Expression);
		}

		bool IsInherited(string componentName)
		{
			return IsInherited(serializationManager.GetInstance(componentName));
		}
		
		static bool IsInherited(object component)
		{
			InheritanceAttribute attribute = GetInheritanceAttribute(component);
			if (attribute != null) {
				return attribute.InheritanceLevel != InheritanceLevel.NotInherited;
			}
			return false;
		}
		
		static InheritanceAttribute GetInheritanceAttribute(object component)
		{
			if (component != null) {
				AttributeCollection attributes = TypeDescriptor.GetAttributes(component);
				return attributes[typeof(InheritanceAttribute)] as InheritanceAttribute;
			}
			return null;	
		}
		
		/// <summary>
		/// Appends expressions like "AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top"
		/// </summary>
		void AppendBinaryOperatorExpression(CodeBinaryOperatorExpression expression)
		{
			AppendExpression(expression.Left);
			AppendBinaryOperator(expression.Operator);
			AppendExpression(expression.Right);
		}
		
		void AppendBinaryOperator(CodeBinaryOperatorType operatorType)
		{
			codeBuilder.Append(" ");
			switch (operatorType) {
				case CodeBinaryOperatorType.BitwiseOr:
					codeBuilder.Append("|");
					break;
			}
			codeBuilder.Append(" ");
		}
	}
}
