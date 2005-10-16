// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Text.RegularExpressions;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;
using ICSharpCode.Core;
using ICSharpCode.FormDesigner.Services;

namespace ICSharpCode.FormDesigner
{
	/// <summary>
	/// This class is able to generate a CodeDOM definition out of a XML file.
	/// </summary>
	public class CodeDOMGenerator
	{
		IDesignerHost   host;
		CodeDomProvider codeProvider;
		
		CodeDOMGeneratorUtility codeDOMGeneratorUtility = new CodeDOMGeneratorUtility();
		List<string> addedVariables = new List<string>();
		string indentation;
		
		public CodeDOMGenerator(IDesignerHost host, CodeDomProvider codeProvider, string indentation)
		{
			this.host = host;
			this.codeProvider = codeProvider;
			this.indentation = indentation;
		}
		
		public void ConvertContentDefinition(TextWriter writer)
		{
			LoggingService.Info("Start CodeCOMGenerator.ConvertContentDefinition");
			DesignerSerializationManager serializationManager = (DesignerSerializationManager)host.GetService(typeof(IDesignerSerializationManager));
			IDisposable session = serializationManager.CreateSession();
			DesignerResourceService designerResourceService = (DesignerResourceService)host.GetService(typeof(System.ComponentModel.Design.IResourceService));
			designerResourceService.SerializationStarted(true);
			
			addedVariables.Clear();
			
			List<CodeStatement> statements = new List<CodeStatement>();
			
			foreach (IComponent component in host.Container.Components) {
				if (!IsComponentAdded(component)) {
					GenerateComponentCode(component, statements, serializationManager);
				}
			}
			
			// Sort statements a bit:
			// Move initialization statements up (but keep the order of everything else!)
			int insertIndex = 0;
			for (int i = 0; i < statements.Count; i++) {
				CodeStatement statement = statements[i];
				if (statement is CodeVariableDeclarationStatement
				    || (statement is CodeAssignStatement
				        && ((CodeAssignStatement)statement).Right is CodeObjectCreateExpression
				        && ((CodeAssignStatement)statement).Left is CodeVariableReferenceExpression)) {
					// move i up to insertIndex
					for (int j = i; j > insertIndex; j--) {
						statements[j] = statements[j - 1];
					}
					statements[insertIndex++] = statement;
				}
			}
			// Move SuspendLayout statements up
			for (int i = insertIndex; i < statements.Count; i++) {
				CodeStatement statement = statements[i];
				if (IsMethodCall(statement, "SuspendLayout")) {
					// move i up to insertIndex
					for (int j = i; j > insertIndex; j--) {
						statements[j] = statements[j - 1];
					}
					statements[insertIndex++] = statement;
				}
			}
			// Move ResumeLayout statements down
			insertIndex = statements.Count - 1;
			for (int i = insertIndex; i >= 0; i--) {
				CodeStatement statement = statements[i];
				if (IsMethodCall(statement, "ResumeLayout")) {
					// move i down to insertIndex
					for (int j = i; j < insertIndex; j++) {
						statements[j] = statements[j + 1];
					}
					statements[insertIndex--] = statement;
				}
			}
			
			
			CodeGeneratorOptions options = codeDOMGeneratorUtility.CreateCodeGeneratorOptions;
			options.IndentString = indentation;
			foreach (CodeStatement statement in statements) {
				// indentation isn't generated when calling GenerateCodeFromStatement
				writer.Write(options.IndentString);
				try {
//						outputGenerator.PublicGenerateCodeFromStatement(statement, Console.Out, options);
					codeProvider.GenerateCodeFromStatement(statement, writer, options);
				} catch (Exception e) {
					codeProvider.GenerateCodeFromStatement(new CodeCommentStatement("TODO: Error while generating statement : " + e.Message),
					                                       writer,
					                                       options);
					LoggingService.Error(e);
				}
			}
			
			designerResourceService.SerializationEnded(true);
			session.Dispose();
			LoggingService.Info("End CodeCOMGenerator.ConvertContentDefinition");
		}
		
		bool IsMethodCall(CodeStatement statement, string methodName)
		{
			CodeExpressionStatement ces = statement as CodeExpressionStatement;
			if (ces == null) return false;
			CodeMethodInvokeExpression cmie = ces.Expression as CodeMethodInvokeExpression;
			if (cmie == null) return false;
			return cmie.Method.MethodName == methodName;
		}
		
		public static bool IsNonVisualComponent(IDesignerHost host, IComponent component)
		{
			IDesigner designer = host.GetDesigner(component);
			return !(designer is ControlDesigner);
		}
		
		void GenerateComponentCode(IComponent component, List<CodeStatement> outputStatements, DesignerSerializationManager serializationManager)
		{
			LoggingService.Debug("Generate code for: " + component.Site.Name);
			Type componentType = component.GetType();
			ExpressionContext exprContext = new ExpressionContext(new CodeThisReferenceExpression(), componentType, component, component);
			((IDesignerSerializationManager)serializationManager).Context.Append(exprContext);
			
			CodeDomSerializer serializer = (CodeDomSerializer)serializationManager.GetSerializer(componentType, typeof(CodeDomSerializer));
			
			if (serializer == null) {
				throw new Exception("No serializer found for component type=" + componentType.ToString());
			}
			
			ICollection statements = serializer.Serialize(serializationManager, component) as ICollection;
			
//			ICSharpCode.NRefactory.Parser.CodeDOMVerboseOutputGenerator outputGenerator = new ICSharpCode.NRefactory.Parser.CodeDOMVerboseOutputGenerator();

			foreach (CodeStatement statement in statements) {
				CodeVariableDeclarationStatement variableDecl = statement as CodeVariableDeclarationStatement;
				if (variableDecl != null) {
					LoggingService.Debug("variable declaration: " + variableDecl.Name);
					if (variableDecl.Name == "resources") {
						FixResourcesVariableDeclarationStatement(variableDecl);
					} else {
						// skip generating the variable declaration if the component is a main
						// component that gets its own field
						// TreeNode is an example that does NOT get its own root component!
						bool foundComponent = false;
						foreach (IComponent c in host.Container.Components) {
							if (variableDecl.Name == c.Site.Name) {
								foundComponent = true;
								break;
							}
						}
						if (foundComponent) {
							addedVariables.Add(((CodeVariableDeclarationStatement)statement).Name);
							continue;
						}
					}
				}
				
				outputStatements.Add(statement);
			}
		}
		
		bool IsComponentAdded(IComponent component)
		{
			if (component.Site != null) {
				return addedVariables.Contains(component.Site.Name);
			}
			return false;
		}
		
		/// <summary>
		/// HACK - Fix the resources variable declaration.  The CodeDomSerializer
		/// creates an incorrect code expression object.
		/// </summary>
		void FixResourcesVariableDeclarationStatement(CodeVariableDeclarationStatement variableDecl)
		{
			CodeObjectCreateExpression exp = variableDecl.InitExpression as CodeObjectCreateExpression;
			if (exp != null) {
				CodeTypeReference typeRef = new CodeTypeReference(host.RootComponent.Site.Name);
				CodeTypeOfExpression typeofExpression = new CodeTypeOfExpression(typeRef);

				exp.Parameters.Clear();
				exp.Parameters.Add(typeofExpression);
			}
		}
	}
}
