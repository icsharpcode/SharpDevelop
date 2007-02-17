// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Globalization;
using System.Text;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Xml;
using System.Reflection;
using System.Drawing.Design;
using System.IO;
using System.Collections;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
#endregion

namespace WorkflowDesigner.Loaders
{
	enum AuthoringMode {
		NoCode,
		CodeSeparation
	}
	
	/// <summary>
	/// Description of XomlDesignerLoader.
	/// </summary>
	public class XomlDesignerLoader : BasicWorkflowDesignerLoader
	{
		private AuthoringMode authoringMode = AuthoringMode.NoCode;
		private string xoml;
		private string codeBesideFileName;
		
		public XomlDesignerLoader(IViewContent viewContent) : base(viewContent)
		{
			// Loock for a code beside file for CodeSeparation mode.
			if (Project != null) {
				FileProjectItem fpi = Project.FindFile(FileName);
				string codeFileName = FileName + "." + Project.LanguageProperties.CodeDomProvider.FileExtension;
				FileProjectItem dfpi = Project.FindFile(codeFileName);
				if (dfpi.DependentUpon == Path.GetFileName(fpi.VirtualName))	{
					authoringMode = AuthoringMode.CodeSeparation;
					codeBesideFileName = codeFileName;
				}
			}
		}

		public XomlDesignerLoader(IViewContent viewContent, Stream stream) : this(viewContent)
		{
			Encoding encoding = ICSharpCode.SharpDevelop.ParserService.DefaultFileEncoding;
			xoml = ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(stream, ref encoding, encoding);
		}

		public string Xoml {
			get { return xoml; }
			set { xoml = value; }
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			
			// Install the additional services into the designer.
			if (authoringMode == AuthoringMode.CodeSeparation) {
				LoaderHost.AddService(typeof(IMemberCreationService), new MemberCreationService(LoaderHost));
				LoaderHost.AddService(typeof(IEventBindingService), new CSharpWorkflowDesignerEventBindingService(LoaderHost,codeBesideFileName));	
			}
		}
		
		protected override void DoPerformFlush(IDesignerSerializationManager serializationManager)
		{
			Activity rootActivity = LoaderHost.RootComponent as Activity;
			if (rootActivity != null) {
				StringBuilder sb = new StringBuilder();
				XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(sb, CultureInfo.CurrentCulture));
				try {
					WorkflowMarkupSerializer xomlSerializer = new WorkflowMarkupSerializer();
					xomlSerializer.Serialize(serializationManager, xmlWriter, rootActivity);
					xoml = sb.ToString();
				} finally {
					xmlWriter.Close();
				}
			}
		}
		
		protected override void DoPerformLoad(IDesignerSerializationManager serializationManager)
		{
			LoadXoml(serializationManager);
		}
		
		protected void LoadXoml(IDesignerSerializationManager serializationManager)
		{
			// get the root activity from the xml.
			XmlReader  reader = new XmlTextReader(new StringReader(xoml));
			Activity rootActivity = null;
			try	{
				WorkflowMarkupSerializer xomlSerializer = new WorkflowMarkupSerializer();
				rootActivity = xomlSerializer.Deserialize(serializationManager, reader) as Activity;
			} finally {
				reader.Close();
			}

			LoaderHost.Container.Add(rootActivity, rootActivity.QualifiedName);
			CompositeActivity compositeActivity = rootActivity as CompositeActivity;
			if (compositeActivity != null)
				AddChildren(compositeActivity);
			
			SetBaseComponentClassName(rootActivity.GetValue(WorkflowMarkupSerializer.XClassProperty) as string);
			
		}
		
		protected void AddChildren(CompositeActivity compositeActivity)
		{
			foreach (Activity activity in compositeActivity.Activities)	{
				LoaderHost.Container.Add(activity, activity.QualifiedName);

				CompositeActivity compositeActivity2 = activity as CompositeActivity;
				if (compositeActivity2 != null)
					AddChildren(compositeActivity2);
			}
		}
		
	}
}
