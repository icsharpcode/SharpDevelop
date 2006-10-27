/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 06.11.2004
 * Time: 10:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Xml;
using SharpReportCore;

namespace SharpReport.Designer{
	/// <summary>
	/// Helperclass with static Members for Xml
	/// </summary>
	/// 
	public class XmlHelpers {
		/// <summary>
		/// Set the values for AbstrctColumns like for sorting etc
		/// </summary>
		/// <param name="reader">See XMLFormReader</param>
		/// <param name="item">AbstractColumn</param>
		/// <param name="ctrlElem">Element witch contains the values</param>
		public static void BuildAbstractColumn (XmlFormReader reader,
		                                        XmlElement ctrlElem,
		                                        AbstractColumn item) {
			
			try {
				XmlNodeList nodeList = ctrlElem.ChildNodes;
				foreach (XmlNode node in nodeList) {
					if (node is XmlElement) {
						XmlElement elem = (XmlElement)node;
						if (elem.HasAttribute("value")) {
							reader.SetValue (item,elem.Name,elem.GetAttribute("value"));
						}
					}
				}
			} catch (Exception) {
				throw;
			}
		}
		/// <summary>
		/// This Class fills an Reportparameter
		/// </summary>
		/// <param name="reader">See XMLFormReader</param>
		/// <param name="parElement">XmlElement ReportParameter</param>
		/// <param name="item"><see cref="ReportParameter"</param>
		public static void BuildReportParameter(XmlFormReader reader,
		                                        XmlElement parElement,
		                                        SharpReportCore.AbstractParameter item) {
			try {
				XmlNodeList nodeList = parElement.ChildNodes;
				foreach (XmlNode node in nodeList) {
					XmlElement elem = (XmlElement)node;
						if (elem.HasAttribute("value")) {
						reader.SetValue ((SqlParameter)item,elem.Name,elem.GetAttribute("value"));
					}
				}
			} catch (Exception) {
				
			}
		}
			
	}
}
