// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Workflow.ComponentModel.Design;
using System.Reflection;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel.Compiler;
using System.CodeDom;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.ReflectionLayer;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;

#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of WorkflowDesignerGenerator.
	/// </summary>
	public abstract class WorkflowDesignerEventBindingService : IWorkflowDesignerEventBindingService, IServiceProvider
	{
		IServiceProvider provider;
		string codeFileName;
        CodeCompileUnit ccu;

		public WorkflowDesignerEventBindingService(IServiceProvider provider, string codeSeparationFileName)
		{
			this.provider = provider;
			this.codeFileName = codeSeparationFileName;
		}


		
		public void UpdateCCU()
		{
			LoggingService.Debug("UpdateCCU");

			TypeProvider typeProvider = (TypeProvider)this.GetService(typeof(ITypeProvider));

			if (ccu != null) 
				typeProvider.RemoveCodeCompileUnit(ccu);
			
			ccu = Parse();
			
			if (ccu != null) 
				typeProvider.AddCodeCompileUnit(ccu);

			
			
		}
		
		private void RefreshCCU(object sender, EventArgs e)
		{
		}
		
		CodeCompileUnit Parse()
		{
			
			string fileContent = ParserService.GetParseableFileContent(codeFileName);
			
			ICSharpCode.NRefactory.IParser  parser = ICSharpCode.NRefactory.ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(fileContent));
			parser.Parse();
			if (parser.Errors.Count > 0) {
				throw new Exception("Syntax errors in " + codeFileName + ":\r\n" + parser.Errors.ErrorOutput);
			}
			
			CodeDomVisitor visitor = new CodeDomVisitor();
			visitor.VisitCompilationUnit(parser.CompilationUnit, null);
			
			return visitor.codeCompileUnit;
		}

		public object GetService(Type serviceType)
		{
			return provider.GetService(serviceType);
		}
		
		public string CodeFileName {
			get {
				return codeFileName;
			}
		}

		public string CreateUniqueMethodName(IComponent component, EventDescriptor e)
		{
			LoggingService.Debug("CreateUniqueMethodName(" + component + ", " + e + ")");
			return String.Format("{0}{1}", Char.ToUpper(component.Site.Name[0]) + component.Site.Name.Substring(1), e.DisplayName);
		}
		
		public EventDescriptor GetEvent(PropertyDescriptor property)
		{
			EventPropertyDescriptor epd = property as EventPropertyDescriptor;
			if (epd == null)
				return null;
			
			return epd.eventDescriptor;
		}
		
		public PropertyDescriptorCollection GetEventProperties(EventDescriptorCollection events)
		{
			ArrayList props = new ArrayList ();
			
			foreach (EventDescriptor e in events)
				props.Add (GetEventProperty (e));
			
			return new PropertyDescriptorCollection ((PropertyDescriptor[]) props.ToArray (typeof (PropertyDescriptor)));
		}
		
		public PropertyDescriptor GetEventProperty(EventDescriptor e)
		{
			return new EventPropertyDescriptor(this,e);
		}
		
		
		public virtual ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			ArrayList compatibleMethods = new ArrayList();
			IClass  completeClass;

			IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			if (designerHost != null && designerHost.RootComponent != null)
			{
				IRootDesigner rootDesigner = designerHost.GetDesigner(designerHost.RootComponent) as IRootDesigner;
				
				LoggingService.DebugFormatted("DesignerHost.RootComponent={0}", rootDesigner.Component.Site.Name);
				
				ParseInformation info = ParserService.ParseFile(this.codeFileName);
				ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
				MethodInfo methodInfo = edesc.EventType.GetMethod("Invoke");
				
				foreach (IClass c in cu.Classes) {
					if (c.Name == rootDesigner.Component.Site.Name){
						LoggingService.DebugFormatted("Got designer class!");
						completeClass = c.GetCompoundClass();
						
						LoggingService.DebugFormatted("Looking for compatible methods");
						
						foreach (IMethod method in completeClass.Methods) {
							if (method.Parameters.Count == methodInfo.GetParameters().Length) {
								bool found = true;
								for (int i = 0; i < methodInfo.GetParameters().Length; ++i) {
									ParameterInfo pInfo = methodInfo.GetParameters()[i];
									IParameter p = method.Parameters[i];
									if (p.ReturnType.FullyQualifiedName != pInfo.ParameterType.ToString()) {
										found = false;
										break;
									}
								}
								if (found) {
									compatibleMethods.Add(method.Name);
								}
								
							}
						}
					}
				}
			}
			
			return compatibleMethods;
		}
		
		protected virtual int GetCursorLine(IDocument document, IMethod method)
		{
			return method.BodyRegion.BeginLine + 1;
		}
		
		protected abstract string CreateEventHandler(IClass completeClass, EventDescriptor edesc, string eventMethodName, string body, string indentation);
		
		
		public bool ShowCode()
		{
			FileService.OpenFile(codeFileName);
			
			return true;
		}
		
		public bool ShowCode(int lineNumber)
		{
			ITextEditorControlProvider t = FileService.OpenFile(codeFileName) as ITextEditorControlProvider;
			t.TextEditorControl.ActiveTextAreaControl.JumpTo(lineNumber);
			
			return true;
		}

		public bool ShowCode(IComponent component, EventDescriptor e)
		{
			
			Activity activity = component as Activity;
			if (component == null)
				throw new ArgumentException("component must be derived from Activity");
			
			string methodName = string.Empty;
			
			// Find method name associated with the EventDescriptor.
			Hashtable events = activity.GetValue(WorkflowMarkupSerializer.EventsProperty) as Hashtable;
			
			if (events != null) {
				if (events.ContainsKey(e.Name))
					methodName = events[e.Name] as string;
			}
			
			return UseMethod(component, e, methodName);
			
		}

		public bool ShowCode(IComponent component, EventDescriptor e, string methodName)
		{
			LoggingService.DebugFormatted("ShowCode {0}", methodName);

			IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			IRootDesigner rootDesigner = designerHost.GetDesigner(designerHost.RootComponent) as IRootDesigner;

			ParseInformation info = ParserService.ParseFile(this.codeFileName);
			ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
			
			ITextEditorControlProvider t = FileService.OpenFile(codeFileName) as ITextEditorControlProvider;
			
			IClass  completeClass;
			foreach (IClass c in cu.Classes) {
				if (c.Name == rootDesigner.Component.Site.Name){
					completeClass = c.GetCompoundClass();

					foreach (IMethod method in completeClass.Methods) {
						if (method.Name == methodName)
						{
							int position = GetCursorLine(t.TextEditorControl.Document, method);
							t.TextEditorControl.ActiveTextAreaControl.JumpTo(position-1);
							return true;
						}
					}
				}
			}
			
			return false;
		}

		
		/// <summary>
		/// Gets a method implementing the signature specified by the event descriptor
		/// </summary>
		protected static IMethod ConvertDescriptorToDom(IClass completeClass, EventDescriptor edesc, string methodName)
		{
			MethodInfo mInfo = edesc.EventType.GetMethod("Invoke");
			DefaultMethod m = new DefaultMethod(completeClass, methodName);
			m.ReturnType = ReflectionReturnType.Create(m, mInfo.ReturnType, false);
			foreach (ParameterInfo pInfo in mInfo.GetParameters()) {
				m.Parameters.Add(new ReflectionParameter(pInfo, m));
			}
			return m;
		}
		
		/// <summary>
		/// Gets a method implementing the signature specified by the event descriptor
		/// </summary>
		protected static ICSharpCode.NRefactory.Ast.MethodDeclaration
			ConvertDescriptorToNRefactory(IClass completeClass, EventDescriptor edesc, string methodName)
		{
			return ICSharpCode.SharpDevelop.Dom.Refactoring.CodeGenerator.ConvertMember(
				ConvertDescriptorToDom(completeClass, edesc, methodName),
				new ClassFinder(completeClass, completeClass.BodyRegion.BeginLine + 1, 1)
			) as ICSharpCode.NRefactory.Ast.MethodDeclaration;
		}
		
		protected virtual int GetEventHandlerInsertionLine(IClass c)
		{
			return c.Region.EndLine;
		}

		public bool UseMethod(IComponent component, EventDescriptor edesc, string methodName)
		{
			LoggingService.DebugFormatted("UseMethod {0}", methodName);
			IClass  completeClass;

			IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			if (designerHost != null && designerHost.RootComponent != null)	{
				
				IRootDesigner rootDesigner = designerHost.GetDesigner(designerHost.RootComponent) as IRootDesigner;
				ITextEditorControlProvider t = FileService.OpenFile(codeFileName) as ITextEditorControlProvider;
				
				LoggingService.DebugFormatted("DesignerHost.RootComponent={0}", rootDesigner.Component.Site.Name);
				
				ParseInformation info = ParserService.ParseFile(this.codeFileName);
				ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
				MethodInfo methodInfo = edesc.EventType.GetMethod("Invoke");
				
				foreach (IClass c in cu.Classes) {
					if (c.Name == rootDesigner.Component.Site.Name){
						LoggingService.DebugFormatted("Got designer class!");
						completeClass = c.GetCompoundClass();
						
						LoggingService.DebugFormatted("Looking for matching methods...");
						
						
						foreach (IMethod method in completeClass.Methods) {
							if ((method.Name == methodName) &&
							    (method.Parameters.Count == methodInfo.GetParameters().Length)) {
								bool found = true;
								LoggingService.DebugFormatted("Name & nbr parms match, checking types...");
								for (int i = 0; i < methodInfo.GetParameters().Length; ++i) {
									ParameterInfo pInfo = methodInfo.GetParameters()[i];
									IParameter p = method.Parameters[i];
									if (p.ReturnType.FullyQualifiedName != pInfo.ParameterType.ToString()) {
										found = false;
										break;
									}
								}
								if (found) {
									LoggingService.DebugFormatted("Found matching method {0}", method.Name);
									int position = GetCursorLine(t.TextEditorControl.Document, method);
									t.TextEditorControl.ActiveTextAreaControl.JumpTo(position-1);
									return true;
								}
							}

						}
						
						LoggingService.DebugFormatted("Creating new method...");
						int line = GetEventHandlerInsertionLine(c);
						int offset = t.TextEditorControl.Document.GetLineSegment(line - 1).Offset;
						t.TextEditorControl.Document.Insert(offset, CreateEventHandler(completeClass, edesc, methodName, "", "\t\t"));
						UpdateCCU();
						return ShowCode(component, edesc, methodName);
					}
				}
			}
			
			return false;
		}
		
	}
}
