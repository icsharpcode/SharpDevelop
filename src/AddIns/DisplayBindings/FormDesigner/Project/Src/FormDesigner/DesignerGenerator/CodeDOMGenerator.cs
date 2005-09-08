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
		CodeDomProvider      codeProvider;
		
		CodeDOMGeneratorUtility codeDOMGeneratorUtility = new CodeDOMGeneratorUtility();
		List<string> addedVariables = new List<string>();
		
		public CodeDOMGenerator(IDesignerHost host, CodeDomProvider codeProvider)
		{
			this.host = host;
			this.codeProvider = codeProvider;
		}
		
		public void ConvertContentDefinition(TextWriter writer)
		{
			LoggingService.Info("Start CodeCOMGenerator.ConvertContentDefinition");
			DesignerSerializationManager serializationManager = (DesignerSerializationManager)host.GetService(typeof(IDesignerSerializationManager));
			IDisposable session = serializationManager.CreateSession();
			DesignerResourceService designerResourceService = (DesignerResourceService)host.GetService(typeof(System.ComponentModel.Design.IResourceService));
			designerResourceService.SerializationStarted(true);
						
			addedVariables.Clear();
			
			foreach (IComponent component in host.Container.Components) {
				if (!IsComponentAdded(component)) {
					GenerateComponentCode(component, writer, serializationManager);
				}
			}
			
			designerResourceService.SerializationEnded(true);
			session.Dispose();
			LoggingService.Info("End CodeCOMGenerator.ConvertContentDefinition");
		}
		
		public static bool IsNonVisualComponent(IDesignerHost host, IComponent component)
		{
			IDesigner designer = host.GetDesigner(component);
			return !(designer is ControlDesigner);
		}
		
		void GenerateComponentCode(IComponent component, TextWriter writer, DesignerSerializationManager serializationManager)
		{
			Type componentType = component.GetType();
			ExpressionContext exprContext = new ExpressionContext(new CodeThisReferenceExpression(), componentType, component, component);
			((IDesignerSerializationManager)serializationManager).Context.Append(exprContext);
			
			CodeDomSerializer serializer = (CodeDomSerializer)serializationManager.GetSerializer(componentType, typeof(CodeDomSerializer));
			
			if (serializer == null) {
				throw new Exception("No serializer found for component type=" + componentType.ToString());
			}
			
			ICollection statements = serializer.Serialize(serializationManager, component) as ICollection;
			CodeGeneratorOptions options = codeDOMGeneratorUtility.CreateCodeGeneratorOptions;
			options.IndentString = "\t\t\t";
			
//			ICSharpCode.NRefactory.Parser.CodeDOMVerboseOutputGenerator outputGenerator = new ICSharpCode.NRefactory.Parser.CodeDOMVerboseOutputGenerator();

			foreach (CodeStatement statement in statements) {
				if ((statement is CodeVariableDeclarationStatement)) {
					addedVariables.Add(((CodeVariableDeclarationStatement)statement).Name);
				} else {
					// indentation isn't generated when calling GenerateCodeFromStatement
					writer.Write(options.IndentString);
					try {						
//						outputGenerator.PublicGenerateCodeFromStatement(statement, Console.Out, options);
						codeProvider.GenerateCodeFromStatement(statement, writer, options);
					} catch (Exception e) {
						codeProvider.GenerateCodeFromStatement(new CodeCommentStatement("TODO: Error while generating statement : " + e.Message),
						                                       writer,
						                                       options);
					}
				}
			}
		}
		
		bool IsComponentAdded(IComponent component)
		{
			if (component.Site != null) {
				return addedVariables.Contains(component.Site.Name);
			}
			return false;
		}
	}
}
