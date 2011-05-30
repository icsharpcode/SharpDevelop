// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation.Host;
using ICSharpCode.Scripting;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PowerShellHostRawUserInterface : PSHostRawUserInterface
	{
		IScriptingConsole scriptingConsole;
		
		public const int MinimumColumns = 80;
		public static readonly ConsoleColor NoConsoleColor = (ConsoleColor)(-1);
		
		public PowerShellHostRawUserInterface(IScriptingConsole scriptingConsole)
		{
			this.scriptingConsole = scriptingConsole;
		}
		
		public override string WindowTitle { get; set; }
		
		public override Size WindowSize {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		
		public override Size BufferSize {
			get {
				int columns = GetColumns();
				return new Size(columns, 0);
			}
			set { throw new NotImplementedException(); }
		}
		
		int GetColumns()
		{
			int width = scriptingConsole.GetMaximumVisibleColumns();
			if (width > MinimumColumns) {
				return width;
			}
			return MinimumColumns;
		}
		
		public override Coordinates WindowPosition {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		
		public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
		{
			throw new NotImplementedException();
		}
		
		public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
		{
			throw new NotImplementedException();
		}
		
		public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
		{
			throw new NotImplementedException();
		}
		
		public override KeyInfo ReadKey(ReadKeyOptions options)
		{
			throw new NotImplementedException();
		}
		
		public override Size MaxWindowSize {
			get { throw new NotImplementedException(); }
		}
		
		public override Size MaxPhysicalWindowSize {
			get { throw new NotImplementedException(); }
		}
		
		public override bool KeyAvailable {
			get { throw new NotImplementedException(); }
		}
		
		public override BufferCell[,] GetBufferContents(Rectangle rectangle)
		{
			throw new NotImplementedException();
		}
		
		public override ConsoleColor ForegroundColor {
			get { return NoConsoleColor; }
			set { }
		}
		
		public override void FlushInputBuffer()
		{
			throw new NotImplementedException();
		}
		
		public override int CursorSize {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		
		public override Coordinates CursorPosition {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		
		public override ConsoleColor BackgroundColor {
			get { return NoConsoleColor; }
			set { }
		}
	}
}
