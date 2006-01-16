/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 16.11.2004
 * Time: 10:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Xml;

//TODO remove "override getXmlData" change all sections to IStoreable

namespace SharpReportCore
{
	/// <summary>
	/// Description of IStoreable.
	/// Remove this class when porting to NET 2.0, 
	/// because this interface is build in
	/// </summary>
	/// 
	public interface IStoreable {
		XmlDocument GetXmlData();
	}
}
