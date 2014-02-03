// Copyright (c) https://github.com/ddur
// This code is distributed under MIT license

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ICSharpCode.CodeCoverage
{
    /// <summary>StringTextSource (ReadOnly)
	/// <remarks>Line and column counting starts at 1.</remarks>
    /// <remarks>IDocument/ITextBuffer/ITextSource fails returning single char "{"?</remarks>
    /// </summary>
    public class CodeCoverageStringTextSource
    {
        private readonly string textSource;
        private struct lineInfo {
            public int Offset;
            public int Length;
        }
        private readonly lineInfo[] lines;
        public CodeCoverageStringTextSource(string source)
        {
            this.textSource = source;

            lineInfo line;
            var lineInfoList = new List<lineInfo>();
            int offset = 0;
            int counter = 0;
            bool newLine = false;
            bool cr = false;
            bool lf = false;

            foreach ( ushort ch in textSource ) {
                switch (ch) {
                    case 0xD:
                        if (lf||cr) {
                            newLine = true; // cr after cr|lf
                        } else {
                            cr = true; // cr found
                        }
                        break;
                    case 0xA:
                        if (lf) {
                            newLine = true; // lf after lf
                        } else {
                            lf = true; // lf found
                        }
                        break;
                    default:
                        if (cr||lf) {
                            newLine = true; // any non-line-end char after any line-end
                        }
                        break;
                }
                if (newLine) { // newLine detected - add line
                    line = new lineInfo();
                    line.Offset = offset;
                    line.Length = counter - offset;
                    lineInfoList.Add(line);
                    offset = counter;
                    cr = false;
                    lf = false;
                    newLine = false;
                }
                ++counter;
            }
            
            // Add last line
            line = new lineInfo();
            line.Offset = offset;
            line.Length = counter - offset;
            lineInfoList.Add(line);

            // Store to readonly field
            lines = lineInfoList.ToArray();
        }

        /// <summary>Return text/source using SequencePoint line/col info
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public string GetText(CodeCoverageSequencePoint sp) {
            return this.GetText(sp.Line, sp.Column, sp.EndLine, sp.EndColumn );
        }

        /// <summary>Return text at Line/Column/EndLine/EndColumn position
        /// <remarks>Line and Column counting starts at 1.</remarks>
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="Column"></param>
        /// <param name="EndLine"></param>
        /// <param name="EndColumn"></param>
        /// <returns></returns>
        public string GetText(int Line, int Column, int EndLine, int EndColumn) {

            var text = new StringBuilder();
            string line;
            bool argOutOfRange;

            if (Line==EndLine) {

                #region One-Line request
                line = GetLine(Line);

                //Debug.Assert(!(Column < 1), "Column < 1");
                //Debug.Assert(!(Column > EndColumn), "Column > EndColumn");
                //Debug.Assert(!(EndColumn > line.Length + 1), string.Format ("Single Line EndColumn({0}) > line.Length({1})",EndColumn, line.Length ));
                //Debug.Assert(!(EndColumn > line.Length + 1), line);

                argOutOfRange = Column < 1
                    ||   Column > EndColumn
                    ||   EndColumn > line.Length;
                if (!argOutOfRange) {
                    text.Append(line.Substring(Column-1,EndColumn-Column));
                }
                #endregion

            } else if (Line<EndLine) {

                #region Multi-line request

                #region First line
                line = GetLine(Line);

                //Debug.Assert(!(Column < 1), "Column < 1");
                //Debug.Assert(!(Column > line.Length), string.Format ("First MultiLine EndColumn({0}) > line.Length({1})",EndColumn, line.Length ));

                argOutOfRange = Column < 1
                    ||   Column > line.Length;
                if (!argOutOfRange) {
                    text.Append(line.Substring(Column-1));
                }
                #endregion

                #region More than two lines
                for ( int lineIndex = Line+1; lineIndex < EndLine; lineIndex++ ) {
                    text.Append ( GetLine ( lineIndex ) );
                }
                #endregion

                #region Last line
                line = GetLine(EndLine);

                //Debug.Assert(!(EndColumn < 1), "EndColumn < 1");
                //Debug.Assert(!(EndColumn > line.Length), string.Format ("Last MultiLine EndColumn({0}) > line.Length({1})",EndColumn, line.Length ));

                argOutOfRange = EndColumn < 1
                    ||   EndColumn > line.Length;
                if (!argOutOfRange) {
                    text.Append(line.Substring(0,EndColumn));
                }
                #endregion

                #endregion

            } else {
                //Debug.Fail("Line > EndLine");
            }
            return text.ToString();
        }

        public int LinesCount {
            get {
                return lines.Length;
            }
        }

        /// <summary>Return SequencePoint enumerated line
        /// </summary>
        /// <param name="LineNo"></param>
        /// <returns></returns>
        public string GetLine ( int LineNo ) {

            string retString = String.Empty;

            if ( LineNo > 0 && LineNo <= lines.Length ) {
                lineInfo lineInfo = lines[LineNo-1];
                retString = textSource.Substring(lineInfo.Offset, lineInfo.Length);
            } else {
                //Debug.Fail( "Line number out of range" );
            }

            return retString;
        }
        
        public static string IndentTabs ( string ToIndent, int TabSize ) {
            
            string retString = ToIndent;
            if ( ToIndent.Contains ( "\t" ) ) {
                int counter = 0;
                int remains = 0;
                int repeat = 0;
                char prevChar = char.MinValue;
                var indented = new StringBuilder();
                foreach ( char currChar in ToIndent ) {
                    if ( currChar == '\t' ) {
                        remains = counter % TabSize;
                        repeat = remains == 0 ? TabSize : remains;
                        indented.Append( ' ', repeat );
                    } else {
                        indented.Append ( currChar, 1 );
                        if ( char.IsLowSurrogate(currChar) 
                            && char.IsHighSurrogate(prevChar)
                           ) { --counter; }
                    }
                    prevChar = currChar;
                    ++counter;
                }
                retString = indented.ToString();
            }
            return retString;
        }

    }
}
