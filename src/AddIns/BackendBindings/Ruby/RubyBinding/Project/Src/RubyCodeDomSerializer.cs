// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Text;

using ICSharpCode.Scripting;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Used to generate Ruby code after the form has been changed in the designer.
	/// </summary>
	public class RubyCodeDomSerializer : IScriptingCodeDomSerializer
	{
		RubyCodeBuilder codeBuilder;
		
		string indentString = String.Empty;
		IDesignerSerializationManager serializationManager;
		
		string rootResourceName = String.Empty;
		
		public RubyCodeDomSerializer() 
			: this("\t")
		{
		}
		
		public RubyCodeDomSerializer(string indentString)
		{
			this.indentString = indentString;
		}
				
		public string GenerateInitializeComponentMethodBody(IDesignerHost host, IDesignerSerializationManager serializationManager)
		{
			return GenerateInitializeComponentMethodBody(host, serializationManager, 0);
		}
		
		public string GenerateInitializeComponentMethodBody(IDesignerHost host, IDesignerSerializationManager serializationManager, int initialIndent)
		{
			return GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, initialIndent);
		}
		
		public string GenerateInitializeComponentMethodBody(IDesignerHost host, IDesignerSerializationManager serializationManager, string rootNamespace, int initialIndent)
		{
			CodeMemberMethod method = FindInitializeComponentMethod(host, serializationManager);
			return GenerateMethodBody(host.RootComponent, method, serializationManager, rootNamespace, initialIndent);
		}
		
		public string GenerateMethodBody(IComponent rootComponent, CodeMemberMethod method, IDesignerSerializationManager serializationManager, string rootNamespace, int initialIndent)
		{
			this.serializationManager = serializationManager;

			codeBuilder = new RubyCodeBuilder(initialIndent);
			codeBuilder.IndentString = indentString;

			GetResourceRootName(rootNamespace, rootComponent);
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
			if (expression.TargetObject is CodeCastExpression) {
				AppendCastMethodReferenceExpression(expression);
			} else {
				AppendExpression(expression.TargetObject);
				codeBuilder.Append(".");
				codeBuilder.Append(expression.MethodName);
			}
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
			codeBuilder.Append(".new");
			AppendParameters(expression.Parameters);
		}
		
		/// <summary>
		/// Appends a constant (e.g. string or int).
		/// </summary>
		void AppendPrimitiveExpression(CodePrimitiveExpression expression)
		{
			codeBuilder.Append(RubyPropertyValueAssignment.ToString(expression.Value));
		}
		
		void AppendFieldReferenceExpression(CodeFieldReferenceExpression expression)
		{
			if (expression.TargetObject is CodeThisReferenceExpression) {
				// Do not append full target object expression.
			} else {
				AppendExpression(expression.TargetObject);
				codeBuilder.Append(".");
			}
			if (expression.FieldName != null) {
				if (expression.TargetObject is CodeThisReferenceExpression) {
					if (IsInherited(expression.FieldName)) {
						AppendThisReferenceExpression();
						codeBuilder.Append(".");
					} else {
						codeBuilder.Append("@");
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
			string baseTypeName = typeRef.BaseType;
			string typeName = baseTypeName.Replace(".", "::");
			typeName = typeName.Replace("+", "::");
			codeBuilder.Append(typeName);
		}
		
		/// <summary>
		/// Creates an array expression:
		/// 
		/// System::Array[System::Object].new(\r\n" +
		///	        ["aaa",
		///         "bbb",
		///	        "ccc\"]) 
		/// </summary>
		void AppendArrayCreateExpression(CodeArrayCreateExpression expression)
		{
			codeBuilder.Append("System::Array[");
			AppendTypeReference(expression.CreateType);
			codeBuilder.Append("].new");
			
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
				codeBuilder.AppendIndented("resources = System::Resources::ResourceManager.new(\"");
				codeBuilder.Append(rootResourceName);
				codeBuilder.Append("\", System::Reflection::Assembly.GetEntryAssembly())");
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
		
		/// <summary>
		/// Using an anonymous code block to call the event handler method:
		/// 
		/// @button1.Click { |sender, e| self.Button1Click(sender, e) }
		/// </summary>
		void AppendAttachEventStatement(CodeAttachEventStatement statement)
		{
			codeBuilder.AppendIndented(String.Empty);
			AppendExpression(statement.Event.TargetObject);
			codeBuilder.Append(".");
			codeBuilder.Append(statement.Event.EventName);
			
			codeBuilder.Append(" { |sender, e| ");
			AppendExpression(statement.Listener);
			codeBuilder.Append("(sender, e) }");
			
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
		
		/// <summary>
		/// Explicit interfaces have to be called in a special way in IronRuby:
		/// 
		/// @pictureBox1.clr_member(System::ComponentModel::ISupportInitialize, :BeginInit).call()
		/// </summary>
		void AppendCastMethodReferenceExpression(CodeMethodReferenceExpression methodRef)
		{
			CodeCastExpression cast = methodRef.TargetObject as CodeCastExpression;
			AppendExpression(cast.Expression);
			codeBuilder.Append(".clr_member(");
			AppendTypeReference(cast.TargetType);
			codeBuilder.Append(", :");
			codeBuilder.Append(methodRef.MethodName);
			codeBuilder.Append(").call");
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
