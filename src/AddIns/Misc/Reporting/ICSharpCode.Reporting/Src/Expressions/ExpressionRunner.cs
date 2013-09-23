// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Expressions
{
	/// <summary>
	/// Description of ExpressionRunner.
	/// </summary>
	class ExpressionRunner:BaseExporter
	{
		 
//	http://www.killswtch.net/2013/08/01/time-arithmetic-with-irony/
//	http://blog.miraclespain.com/archive/2009/Oct-07.html	
//		
		private readonly ExpressionVisitor visitor;
		
		 public ExpressionRunner(Collection<ExportPage> pages):base(pages)
		{
			visitor = new ExpressionVisitor (Pages);
			
		}
		
		public override void Run()
		{
			Console.WriteLine();
			Console.WriteLine("Start ExpressionVisitor");
			foreach (var page in Pages) {
				var acceptor = page as IAcceptor;
				acceptor.Accept(visitor);
//				RunInternal(page);
			}
			Console.WriteLine("Finish ExpressionVisitor");
			Console.WriteLine();
		}
		
//		"Visitor"
//		http://irony.codeplex.com/discussions/213938
//		
//		http://irony.codeplex.com/discussions/35310
		
		/*
		void RunInternal(IExportContainer container)
		{
//			Console.WriteLine();
//			Console.WriteLine("{0}{1}",leading,container.Name);
			foreach (var item in container.ExportedItems) {
				var exportContainer = item as IExportContainer;
				var acceptor = item as IAcceptor;
				if (exportContainer != null) {
					if (exportContainer.ExportedItems.Count > 0) {
//						acceptor.Accept(visitor);
						bRunInternal(exportContainer.ExportedItems);
						acceptor.Accept(visitor);
//						ShowDebug(leading = leading + "--",exportContainer);
						
					}
				}
//				acceptor.Accept(visitor);
			}
		}
		*/
		/*
		
		void bRunInternal(List<IExportColumn> list)
		{
//			Console.WriteLine();
//			Console.WriteLine("{0}{1}",leading,container.Name);
			foreach (var item in list) {
				var exportContainer = item as IExportContainer;
				var acceptor = item as IAcceptor;
				if (exportContainer != null) {
					if (exportContainer.ExportedItems.Count > 0) {
						acceptor.Accept(visitor);
						RunInternal(exportContainer);
						
//						ShowDebug(leading = leading + "--",exportContainer);
						
					}
				}
				acceptor.Accept(visitor);
			}
		}
		*/
		/*
		void bRunInternal(IExportContainer container)
		{
//			Console.WriteLine();
//			Console.WriteLine("{0}{1}",leading,container.Name);
			foreach (var item in container.ExportedItems) {
				var exportContainer = item as IExportContainer;
				var acceptor = item as IAcceptor;
				if (exportContainer != null) {
					if (exportContainer.ExportedItems.Count > 0) {
						acceptor.Accept(visitor);
						RunInternal(exportContainer);
						
//						ShowDebug(leading = leading + "--",exportContainer);
						
					}
				}
				acceptor.Accept(visitor);
			}
		}
		*/
		
		/*
		//Items first, then container
		void aRunInternal(IExportContainer container)
		{
//			Console.WriteLine();
//			Console.WriteLine("{0}{1}",leading,container.Name);
			foreach (var item in container.ExportedItems) {
				var exportContainer = item as IExportContainer;
				var acceptor = item as IAcceptor;
				if (exportContainer != null) {
					if (exportContainer.ExportedItems.Count > 0) {
						RunInternal(exportContainer);
						acceptor.Accept(visitor);
//						ShowDebug(leading = leading + "--",exportContainer);
						
					}
				}
				acceptor.Accept(visitor);
			}
		}
		*/
		/*
		void InternalRun(ExportPage page)
		{
			page.Accept(visitor);
			foreach (var item in page.ExportedItems) {
//				ShowContainerRecursive(null,item);
				var acceptor = item as IAcceptor;
				acceptor.Accept(visitor);
			}
		}
		*/
	}
}
