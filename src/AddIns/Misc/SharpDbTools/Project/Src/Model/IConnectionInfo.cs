/*
 * User: Dickon Field
 * Date: 10/07/2006
 * Time: 09:23
 * 
 */

using System;

namespace SharpDbTools.Model
{
	/// <summary>
	/// Presents the basic contract for ConnectionInfo classes
	/// </summary>
	public interface IConnectionInfo
	{
		bool HasConnection
		{
			get;
		}
		
		bool HasModel
		{
			get;
		}
	}
}
