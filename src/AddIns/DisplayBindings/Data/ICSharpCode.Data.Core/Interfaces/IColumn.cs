// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.Core.Interfaces
{
    public interface IColumn : IDatabaseObjectBase
    {
        string ColumnSummary { get; }
        string DataType { get; set; }
        string SystemType { get; set; }
        int Length { get; set; }       
        int ColumnId { get; set; }
        int FullTextTypeColumn { get; set; }
        bool IsComputed { get; set; }
        bool IsCursorType { get; set; }
        bool IsDeterministic { get; set; }
        bool IsForeignKey { get; }
        bool IsFulltextIndexed { get; set; }
        bool IsIdentity { get; set; }
        bool IsIdNotForRepl { get; set; }
        bool IsIndexable { get; set; }
        bool IsNullable { get; set; }
        bool IsOutParam { get; set; }
        bool IsPrecise { get; set; }
        bool IsPrimaryKey { get; set; }
        bool IsRowGuidCol { get; set; }
        bool IsUserDefinedDataType { get; }
        bool IsSystemVerified { get; set; }
        bool IsXmlIndexable { get; set; }
        int Precision { get; set; }
        int Scale { get; set; }
        bool SystemDataAccess { get; set; }
        bool UserDataAccess { get; set; }
        bool UsesAnsiTrim { get; set; }
    }
}
