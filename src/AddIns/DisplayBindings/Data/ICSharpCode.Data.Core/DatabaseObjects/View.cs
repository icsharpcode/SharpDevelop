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
using ICSharpCode.Data.Core.Interfaces;

#endregion

namespace ICSharpCode.Data.Core.DatabaseObjects
{
    public class View : Table, IView
    {
        #region Fields

        private string _query = string.Empty;
        private string _definingQuery = string.Empty;

        #endregion

        #region Properties

        public string Query
        {
            get { return _query; }
            set
            {
                _query = value;
                OnPropertyChanged("Query");
            }
        }

        public string DefiningQuery
        {
            get { return _definingQuery; }
            set
            {
                _definingQuery = value;
                OnPropertyChanged("DefiningQuery");
            }
        }

        #endregion
    }
}
