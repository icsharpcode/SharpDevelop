// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;

using ICSharpCode.SharpDevelop.Widgets;

namespace Debugger.AddIn.Visualizers
{
    public class EnumDisplayItem<T>
    {
        public T EnumValue { get; set; }
        public string DisplayValue { get; set; }

        public EnumDisplayItem()
        {
        }

        public EnumDisplayItem(T enumValue, string displayValue)
        {
            EnumValue = enumValue;
            DisplayValue = displayValue;
        }
    }

    public class EnumViewModel<T> : ViewModelBase where T : struct
    {
        private readonly CollectionView _enumValues;
        private T _enumValue;

        public EnumViewModel()
        {
            var displayItems = new List<EnumDisplayItem<T>>();

            var enumFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in enumFields)
            {
                object[] fieldAttributes = field.GetCustomAttributes(typeof(DisplayAttribute), true);
                T enumValue = (T)(field.GetValue(null));
                string enumValueDisplayString = enumValue.ToString();
                // display user-defined string if present
                if (fieldAttributes.Length > 0)
                {
                    enumValueDisplayString = ((DisplayAttribute)fieldAttributes[0]).DisplayValue;
                }
                displayItems.Add(new EnumDisplayItem<T>(enumValue, enumValueDisplayString));
            }

            _enumValues = new CollectionView(displayItems);
        }

        public CollectionView EnumValues
        {
            get { return _enumValues; }
        }

        public T SelectedEnumValue
        {
            get { return _enumValue; }
            set
            {
                if (_enumValue.Equals(value)) return;
                _enumValue = value;
                RaisePropertyChanged(() => SelectedEnumValue);
            }
        }
    }
}
