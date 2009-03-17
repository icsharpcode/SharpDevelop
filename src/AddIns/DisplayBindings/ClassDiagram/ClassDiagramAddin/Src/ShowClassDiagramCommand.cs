/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 23/09/2006
 * Time: 12:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Windows.Forms;

using ClassDiagram;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using System.Xml;

namespace ClassDiagramAddin
{
	public class ShowClassDiagramCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IProject p = ProjectService.CurrentProject;
			string filename = Path.Combine(p.Directory, p.Name+".cd");
			if (p == null) return;
			/*if (p.IsFileInProject(filename))
			{
				ProjectItem pi = p.Items.Find(
					delegate(ProjectItem pItem)
					{ return pItem.FileName == filename; }
				);
			}
			else*/
			{
				//MessageBox.Show("Creating a new class diagram file named "+"\"+p.Directory+filename);
				ClassCanvas classcanvas = new ClassCanvas();
				
				IProjectContent pc = ParserService.GetProjectContent(p);
				//float x = 20, y = 20;
				//float max_h = 0;
				
				foreach (IClass ct in pc.Classes)
				{
					ClassCanvasItem classitem = ClassCanvas.CreateItemFromType(ct);
					classcanvas.AddCanvasItem(classitem);
				}
				
				classcanvas.AutoArrange();
				XmlDocument xmlDocument = classcanvas.WriteToXml();
				FileUtility.ObservedSave(
					newFileName => SaveAndOpenNewClassDiagram(p, newFileName, xmlDocument),
					filename, FileErrorPolicy.ProvideAlternative
				);
			}
		}

		void SaveAndOpenNewClassDiagram(IProject p, string filename, XmlDocument xmlDocument)
		{
			xmlDocument.Save(filename);
			FileProjectItem fpi = new FileProjectItem(p, ItemType.Content);
			fpi.FileName = filename;
			ProjectService.AddProjectItem(p, fpi);
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
			p.Save();
			FileService.OpenFile(filename);
		}
	}

}
