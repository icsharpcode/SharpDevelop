// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
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
		
		public CodeDOMGenerator(IDesignerHost host, CodeDomProvider codeProvider)
		{
			this.host = host;
			this.codeProvider = codeProvider;
		}
		
		public void ConvertContentDefinition(TextWriter writer)
		{
			DesignerSerializationManager  serializationManager = (DesignerSerializationManager )host.GetService(typeof(IDesignerSerializationManager));
			IDisposable session = serializationManager.CreateSession();
			DesignerResourceService designerResourceService = (DesignerResourceService)host.GetService(typeof(System.ComponentModel.Design.IResourceService));
			designerResourceService.SerializationStarted(true);
			
			CodeDomSerializer rootSerializer = (CodeDomSerializer)serializationManager.GetSerializer(host.RootComponent.GetType(), typeof(CodeDomSerializer));
			
			if (rootSerializer == null) {
				throw new Exception("No root serializer found");
			}
			
			ICollection statements = rootSerializer.Serialize(serializationManager, host.RootComponent) as ICollection;
			codeDOMGeneratorUtility.CreateCodeGeneratorOptions.IndentString  = "\t\t";
			
			foreach (CodeStatement statement in statements) {
				if (!(statement is CodeVariableDeclarationStatement)) {
					try {
						codeProvider.GenerateCodeFromStatement(statement, 
						                                       writer, 
						                                       codeDOMGeneratorUtility.CreateCodeGeneratorOptions);
					} catch (Exception e) {
						codeProvider.GenerateCodeFromStatement(new CodeCommentStatement("TODO: Error while generating statement : " + e.Message), 
						                                       writer, 
						                                       codeDOMGeneratorUtility.CreateCodeGeneratorOptions);
					}
				}
			}
			designerResourceService.SerializationEnded(true);
			session.Dispose();
		}
	}
}
