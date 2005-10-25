// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class TypeRef : AbstractRow
	{
		public static readonly int TABLE_ID = 0x01;
		
		uint resolutionScope; // index into Module, ModuleRef, AssemblyRef or TypeRef tables, or null; more precisely, a ResolutionScope coded index
		uint name;
		uint nspace;
		
		public uint ResolutionScope {
			get {
				return resolutionScope;
			}
			set {
				resolutionScope = value;
			}
		}
		
		public uint Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public uint Nspace {
			get {
				return nspace;
			}
			set {
				nspace = value;
			}
		}
		
		public override void LoadRow()
		{
			resolutionScope = ReadCodedIndex(CodedIndex.ResolutionScope);
			name            = LoadStringIndex();
			nspace          = LoadStringIndex();
		}
	}
}
