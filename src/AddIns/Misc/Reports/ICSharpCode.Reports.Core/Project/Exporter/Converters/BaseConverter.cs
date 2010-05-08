/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 28.12.2008
 * Zeit: 17:30
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Events;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{

	/// <summary>
	/// Description of BaseConverter.
	/// </summary>
	
	public class BaseConverter:IBaseConverter
	{
		
		private IDataNavigator dataNavigator;
		private ExporterPage singlePage;
		private SectionBounds sectionBounds;
		private Rectangle parentRectangle;
		private IExportItemsConverter exportItemsConverter;
		private ILayouter layouter;
		private Size saveSize;
	
		public event EventHandler <NewPageEventArgs> PageFull;
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		
		
		public BaseConverter(IDataNavigator dataNavigator,ExporterPage singlePage,
		                     IExportItemsConverter exportItemsConverter,ILayouter layouter)
		{
			if (dataNavigator == null) {
				throw new ArgumentNullException("dataNavigator");
			}
			if (singlePage == null) {
				throw new ArgumentNullException("singlePage");
			}
			if (exportItemsConverter == null) {
				throw new ArgumentNullException("exportItemsConverter");
			}
			if (layouter == null) {
				throw new ArgumentNullException("layouter");
			}
			this.singlePage = singlePage;
			this.dataNavigator = dataNavigator;
			this.sectionBounds = this.singlePage.SectionBounds;
			this.exportItemsConverter = exportItemsConverter;
			this.layouter = layouter;
		}
		
		
		protected void FirePageFull (ExporterCollection items)
		{
			EventHelper.Raise<NewPageEventArgs>(PageFull,this,new NewPageEventArgs(items));
		}
		
			
		protected void FireSectionRendering (BaseSection section)
		{
			SectionRenderEventArgs srea = new SectionRenderEventArgs(section,
			                                                         this.SinglePage.PageNumber,
			                                                         this.dataNavigator.CurrentRow,
			                                                         section);
			EventHelper.Raise<SectionRenderEventArgs>(SectionRendering,this,srea);
		}
		
		
		
		protected  ExporterCollection ConvertItems (ISimpleContainer row,Point offset)		                                          
		{

			IExportColumnBuilder exportLineBuilder = row as IExportColumnBuilder;

			if (exportLineBuilder != null) {

				ExportContainer lineItem = this.exportItemsConverter.ConvertToContainer(offset,row);
				BaseReportItem baseReportItem = row as BaseReportItem;

				this.exportItemsConverter.ParentRectangle = new Rectangle(baseReportItem.Location,baseReportItem.Size);
				if (baseReportItem.BackColor != GlobalValues.DefaultBackColor) {
					foreach (BaseReportItem i in row.Items) {
						i.BackColor = baseReportItem.BackColor;
					}
				}
				
				ExporterCollection list = this.exportItemsConverter.ConvertSimpleItems(offset,row.Items);
					
				lineItem.Items.AddRange(list);
				
				ExporterCollection containerList = new ExporterCollection();
				containerList.Add (lineItem);
				return containerList;
			}
			return null;
		}
		
		
		#region IBaseConverter
		
		public virtual ExporterCollection Convert(BaseReportItem parent, BaseReportItem item)
		{
			this.parentRectangle = new Rectangle(parent.Location,parent.Size);
			return new ExporterCollection();;
		}
		
		
		public Rectangle ParentRectangle {
			get { return parentRectangle; }
		}
		
		
		public ExporterPage SinglePage {
			get { return singlePage; }
		}
		
		public SectionBounds SectionBounds {
			get { return sectionBounds; }
		}
		
		public IDataNavigator DataNavigator {
			get { return dataNavigator; }
		}
		
		
		public ILayouter Layouter {
			get { return layouter; }
		}
		
		public Graphics Graphics {get;set;}
		#endregion
		
		protected void  SaveSize(Size size)
		{
			this.saveSize = size;
		}
		
		
		protected Size RestoreSize
		{
			get {return this.saveSize;}
		}
		
		
		protected void FillAndLayoutRow (ISimpleContainer row)
		{
			DataNavigator.Fill(row.Items);
			PrintHelper.SetLayoutForRow(Graphics,Layouter,row);
		}
		
		
		protected Point BaseConvert(ExporterCollection myList,ISimpleContainer container,int leftPos,Point curPos)
		{
			container.Location = new Point (leftPos,container.Location.Y);	
			ExporterCollection ml = this.ConvertItems (container, curPos);		
			myList.AddRange(ml);
			return new Point (leftPos,curPos.Y + container.Size.Height + (3 *GlobalValues.GapBetweenContainer));
		}
	}
}
