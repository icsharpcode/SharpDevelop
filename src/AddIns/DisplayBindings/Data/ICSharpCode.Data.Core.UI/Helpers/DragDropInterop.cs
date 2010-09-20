// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
