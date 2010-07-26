/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 09.11.2009
 * Zeit: 20:25
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Data;

namespace ICSharpCode.Reports.Core.Project.Interfaces
{
	/// <summary>
	/// Description of IDataAccessStrategy.
	/// </summary>
	public interface IDataAccessStrategy
	{
		bool OpenConnection ();
		DataSet ReadData();
	}
}
