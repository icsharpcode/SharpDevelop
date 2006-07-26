/*
 * User: dickon
 * Date: 26/07/2006
 * Time: 01:07
 * 
 */

using System;
using SharpDbTools.Model;

namespace SharpDbTools.Viewer
{
	/// <summary>
	/// Description of ViewerFactory.
	/// </summary>
	public class ViewerFactory
	{
		static ViewerFactory instance = new ViewerFactory();
		
		ViewerFactory()
		{
		}
		
		public static ViewerFactory GetInstance()
		{
			return instance;
		}
		
		public static IViewer GetViewer(string metaDataCollectionName, 
		                                DbConnectionInfo connectionInfo)
		{
			return null;
		}
	}
}
