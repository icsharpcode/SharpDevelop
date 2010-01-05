// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using SharpQuery.Collections;
using SharpQuery.Connection;
using SharpQuery.SchemaClass;

namespace SharpQuery.Gui.TreeView
{
	public interface ISharpQueryNode
	{
		///<summary>
		/// <returns> Path to a string #develop menu command object.</returns>
		/// <remarks>You should add this extension path in the addin file.</remarks>
		/// </summary>
		string AddinContextMenu { get; }

		///<summary>
		/// His schema class from wich to displayed data informations.
		///</summary>
		ISchemaClass SchemaClass { get; }

		///<summary>
		/// the <see cref=".ISharpQueryNode.SchemaClass">SchemaClass</see>'s connection.
		///</summary>
		IConnection Connection { get; }

		///<summary>
		/// Those, are list of the childs schema.( columns, etc etc )
		/// i am using a dictionnary because is more simplest to write
		/// <code>Entities["PROCEDURES"]</code> than <code>Entities[0]</code>.
		///</summary>
		SharpQueryListDictionary Entities { get; }


		///<summary>
		/// Calls the <see cref=".ISharpQueryNode.Clear()">Clear()</see> method.
		/// Calls the <see cref=".ISchemaClass.Refresh()">Refresh()</see> method of his <see cref=".ISharpQueryNode.SchemaClass">SchemaClass</see> member.
		/// Calls the <see cref=".ISchemaClass.BuildsChilds()">BuildsChild()</see> method.
		/// </summary>
		void Refresh();

		///<summary>
		/// Calls the <code>Clear()</code> method of each child nodes.
		/// Calls the <see cref=".ISchemaClass.Clear()">Clear()</see> methode of his <see cref=".ISharpQueryNode.SchemaClass">SchemaClass</see> member.
		/// Remove each child nodes.
		/// </summary>
		void Clear();

		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// </summary>
		void Execute(int rows);

		///<summary>
		/// Builds childs <see cref=".ISharpQueryNode">ISharpQueryNode</see>
		/// </summary>
		void BuildsChilds();
	}
}
