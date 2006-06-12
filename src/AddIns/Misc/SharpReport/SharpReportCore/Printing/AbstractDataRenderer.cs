/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 23.03.2006
 * Time: 11:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Printing;
namespace SharpReportCore{
	/// <summary>
	/// Description of AbstractDataRenderer.
	/// </summary>
	public class AbstractDataRenderer : AbstractRenderer{
		DataManager dataManager;
		DataNavigator navigator;
		
		
		public AbstractDataRenderer(ReportModel model,DataManager dataManager):base(model){
			if (dataManager == null) {
				throw new ArgumentNullException("dataManager");
			}
			this.dataManager = dataManager;
		}
		
		#region overrides
		protected override void ReportBegin(object sender, PrintEventArgs e){
			base.ReportBegin(sender, e);
		}
		
		
		protected override int RenderSection(BaseSection section, ReportPageEventArgs rpea){
			return 0;
		}
		#endregion
		protected int DoItems (ReportPageEventArgs rpea) {
			IContainerItem container = null;
			bool hasContainer = false;
			foreach (BaseReportItem item in this.CurrentSection.Items) {
				container = item as IContainerItem;
				if (container != null) {
					hasContainer = true;
					break;
				}
			}
			if (hasContainer) {
				return DoContainerControl(this.CurrentSection,container,rpea);
			} else {
				return base.RenderSection(this.CurrentSection, rpea);
			}
		}
		protected int old_DoItems (BaseSection section, ReportPageEventArgs rpea) {
			IContainerItem container = null;
			bool hasContainer = false;
			foreach (BaseReportItem item in section.Items) {
				container = item as IContainerItem;
				if (container != null) {
					hasContainer = true;
					break;
				}
			}
			if (hasContainer) {
				return DoContainerControl(section,container,rpea);
			} else {
				return base.RenderSection(section, rpea);
			}
		}
		
		private int DoContainerControl (BaseSection section,
		                                IContainerItem container,
		                                ReportPageEventArgs rpea) {
			
			if (container == null) {
				return section.Size.Height;
			}
			this.DataNavigator.Fill(container.Items);
			Point drawPoint	= new Point(0,0);
			if (section.Visible){
				section.Render (rpea);
				
				foreach (BaseReportItem item in section.Items) {
					if (item.Parent == null) {
						item.Parent = section;
					}
					item.SectionOffset = section.SectionOffset;
					base.DrawSingleItem (rpea,item);
					drawPoint.Y = section.SectionOffset + section.Size.Height;
					rpea.LocationAfterDraw = new PointF (rpea.LocationAfterDraw.X,section.SectionOffset + section.Size.Height);
					
				}
				if ((section.CanGrow == false)&& (section.CanShrink == false)) {
					return section.Size.Height;					
				}				
				return drawPoint.Y;
			}
			return drawPoint.Y;
		}
		
		#region Properties
		
		protected DataManager DataManager {
			get {
				return dataManager;
			}
		}
		
		protected DataNavigator DataNavigator{
			get {return this.navigator;}
			set {this.navigator = value;}
		}
		
		#endregion
		
		#region IDisposable
		public override void Dispose()
		{
			if (this.dataManager != null) {
				this.dataManager.Dispose();
				this.dataManager = null;
			}
			if (this.navigator != null) {
				this.navigator= null;
			}
			base.Dispose();
		}
		#endregion
	}
}
