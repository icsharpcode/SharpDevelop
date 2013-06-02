/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.BaseClasses
{
	/// <summary>
	/// Description of Page.
	/// </summary>
	///
	
	public class Page:IExportContainer,IPage
	{
		public Page(IPageInfo pageInfo,Size pageSize)
		{
			if (pageInfo == null) {
				throw new ArgumentNullException("pageInfo");
			}
			PageInfo = pageInfo;
			Name = "Page";
			Size = pageSize;
			exportedItems = new List<IExportColumn>();
		}
		
		public bool IsFirstPage {get;set;}
		
		
		public IPageInfo PageInfo {get;private set;}
		

		public string Name {get;set;}
		
		
		public System.Drawing.Size Size {get;set;}

		
		public System.Drawing.Point Location {get;set;}
			
		
		public List<IExportColumn> exportedItems;
		
		public List<IExportColumn> ExportedItems {
			get { return exportedItems; }
		}
		
		
		public IExportContainer CreateExportColumn()
		{
			throw new NotImplementedException();
		}
		
		public ICSharpCode.Reporting.Arrange.IArrangeStrategy GetArrangeStrategy()
		{
			throw new NotImplementedException();
		}
		
		public Size DesiredSize {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public Color ForeColor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public Color BackColor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		
		public Color FrameColor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		
		public IExportColumn Parent {
			get {
				return null;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool CanGrow {get;set;}
		
		public bool CanShrink {get;set;}
		
		public Rectangle DisplayRectangle {
			get {
				return new Rectangle(Location,Size);
			}
		}
	}
}
