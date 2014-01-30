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
using System.Reflection;
using System.Collections;
using System.Windows.Forms;

#endregion

namespace ICSharpCode.Data.Core.UI.Helpers
{
    public static class DragDropInterop
    {
        public static T GetObjectFromDragEventArgs<T>(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(T)))
            {
                return default(T);
            }
            try
            {
                var temp = e.Data.GetData(typeof(T));
                if (temp != null)
                {
                    return (T)temp;
                }
                var fieldInfo = e.Data.GetType().GetField("innerData", BindingFlags.NonPublic | BindingFlags.Instance);
                temp = fieldInfo.GetValue(e.Data);
                fieldInfo = temp.GetType().GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
                fieldInfo = temp.GetType().GetField("innerData", BindingFlags.NonPublic | BindingFlags.Instance);
                temp = fieldInfo.GetValue(temp);
                fieldInfo = temp.GetType().GetField("_innerData", BindingFlags.NonPublic | BindingFlags.Instance);
                temp = fieldInfo.GetValue(temp);
                fieldInfo = temp.GetType().GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance);
                var hashtable = (Hashtable)fieldInfo.GetValue(temp);
                var array = (object[])hashtable.Cast<DictionaryEntry>().First().Value;
                temp = array[0];
                fieldInfo = temp.GetType().GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance);
                return (T)fieldInfo.GetValue(temp);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
