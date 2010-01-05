/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 18.11.2009
 * Zeit: 19:30
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;

namespace TestProject
{
	
	public static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Form2 frm = new Form2();
			frm.ShowDialog();
		}
	}
}