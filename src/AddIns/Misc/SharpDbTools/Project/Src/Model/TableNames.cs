// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

/*
 * User: dickon
 * Date: 30/07/2006
 * Time: 23:35
 * 
 */

using System;

namespace SharpDbTools.Model
{
	/// <summary>
	/// Description of Tables.
	/// </summary>
	public sealed class TableNames
	{
		public const string MetaDataCollections = "MetaDataCollections";
		public const string ConnectionInfo = "ConnectionInfo";
		public static string[] PrimaryObjects = new string[] { "Tables", "Procedures",  "Functions", "Views", "Users" };
	}
}
