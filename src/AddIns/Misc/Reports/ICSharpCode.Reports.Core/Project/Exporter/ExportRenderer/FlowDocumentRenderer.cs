// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.IO.Packaging;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

using ICSharpCode.Reports.Core.WPF;

namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer
{
	/// <summary>
	/// Description of XPSRenderer.
	/// </summary>
	public class FlowDocumentRenderer:BaseExportRenderer
	{
		private PagesCollection pages;
		private FlowDocumentCreator pageCreator;
		
		#region Constructor
		
		public static FlowDocumentRenderer CreateInstance (PagesCollection pages) {
			
			var instance = new FlowDocumentRenderer(pages);
	
			return instance;
		}
		
		private FlowDocumentRenderer(PagesCollection pages):base(pages)
		{
			this.pages = pages;
			Console.WriteLine("FlowDocumentRenderer - Create Instance");		
			this.pageCreator = new FlowDocumentCreator();
		}
		
		#endregion
		
		#region overrides
		
		public override void Start()
		{
			base.Start();
			Console.WriteLine("FlowDocumentRenderer - Start");		
		}
		
	
		public override void RenderOutput()
		{
			base.RenderOutput();
			Console.WriteLine("FlowDocumentRenderer - RenderOutput");
			
			IDocumentPaginatorSource d  = new FlowDocument(pageCreator.CreatePage(pages[0]));

			
			if ( d is FlowDocument) 
			{
				// DocumenattViewer does not support FlowDocument, so we'll convert it
					MemoryStream xpsStream = new MemoryStream();
					Package package = Package.Open(xpsStream, FileMode.Create, FileAccess.ReadWrite);
					string packageUriString = "memorystream://data.xps";
					PackageStore.AddPackage(new Uri(packageUriString), package);

					XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Normal, packageUriString);
					XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
					
					writer.Write(d.DocumentPaginator);
					Document = xpsDocument.GetFixedDocumentSequence();
//					cleanup = delegate {
//						viewer.Document = null;
//						xpsDocument.Close();
//						package.Close();
//						PackageStore.RemovePackage(new Uri(packageUriString));
//					};
			} 
			else {
				Document = d;
			}
		}
		
		
		public override void End()
		{
			base.End();
			Console.WriteLine("FlowDocumentRenderer - End");		
		}
			
		#endregion
		
		public IDocumentPaginatorSource Document {get;private set;}
	}
}
