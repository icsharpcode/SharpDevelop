/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 15.07.2009
 * Time: 21:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using ICSharpCode.Core;
using System;
using System.Linq;
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WorkflowDesigner
{
	/// <summary>
	/// Display binding that attaches the workflow designer to .xaml files.
	/// </summary>
	public class WorkflowDisplayBinding : ISecondaryDisplayBinding
	{
		public bool CanAttachTo(IViewContent content)
		{
			ITextEditorProvider p = content as ITextEditorProvider;
			if (p != null) {
				try {
					using (XmlTextReader r = new XmlTextReader(p.TextEditor.Document.CreateReader())) {
						r.XmlResolver = null;
						// find the opening of the root element:
						while (r.Read() && r.NodeType != XmlNodeType.Element);
						// attach if this is a workflow node
						return r.NamespaceURI == "http://schemas.microsoft.com/netfx/2009/xaml/activities";
					}
				} catch (XmlException e) {
					LoggingService.Debug("WorkflowDisplayBinding got exception: " + e.Message);
					return false;
				}
			} else {
				return false;
			}
		}
		
		/// <summary>
		/// When you return true for this property, the CreateSecondaryViewContent method
		/// is called again after the LoadSolutionProjects thread has finished.
		/// </summary>
		public bool ReattachWhenParserServiceIsReady {
			get { return false; }
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			return new IViewContent[] { new WorkflowDesignerViewContent(viewContent.PrimaryFile) };
		}
	}
}
