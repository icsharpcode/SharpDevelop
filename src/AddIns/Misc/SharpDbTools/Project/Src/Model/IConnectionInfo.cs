/*
 * User: dickon
 * Date: 25/07/2006
 * Time: 22:11
 * 
 */

using System;

namespace SharpDbTools.Model
{
	/// <summary>
	/// Defines the basic contract for an IConnectionInfo object. The basic idea
	/// is to flag the current state of the underlying connection - has a connection
	/// been made based on the information in the IConnectionInfo object? Has the
	/// underlying model been built from the connection?
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
