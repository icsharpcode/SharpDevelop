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
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Attributes;
using System.ComponentModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using System.Collections;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property
{
    public class ScalarProperty : PropertyBase
    {
        public ScalarProperty()
        {
            _nullable = true;
        }

        protected override Func<TypeBase, IList> GetPropertyCollection
        {
            get { return entityType => entityType.ScalarProperties; }
        }

        private bool _isKey;
        [DisplayName("Entity Key")]
        [DisplayVisibleCondition("CanBeKey")]
        public bool IsKey
        {
            get { return _isKey; }
            set 
            {
                _isKey = value;
                OnPropertyChanged("IsKey");
                if (value)
                    Nullable = false;
                OnPropertyChanged("Nullable");
                OnPropertyChanged("CanBeNullable");
            }
        }

        public bool CanBeKey
        {
            get 
            {
                var entityType = EntityType as EntityType;
                if (entityType == null)
                    return false;
                return entityType.IsEntitySet;
            }
        }

        private PropertyType _type;
        public PropertyType Type
        {
            get { return _type; }
            set 
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public bool CanBeNullable
        {
            get { return ! IsKey;}
        }

        private bool _nullable;
        [DefaultValue(true)]
        [DisplayEnabledCondition("CanBeNullable")]
        public bool Nullable
        {
            get { return _nullable; }
            set 
            {
                _nullable = value;
                OnPropertyChanged("Nullable");
            }
        }

        private Visibility _setVisibility; 
        [DisplayName("Setter")]
        public Visibility SetVisibility
        {
            get { return _setVisibility; }
            set 
            {
                _setVisibility = value;
                OnPropertyChanged("SetVisibility");
            }
        }

        public bool IsString
        {
            get { return Type == PropertyType.String; }
        }

        private int? _maxLength;
        [DisplayVisibleCondition("IsString")]
        public int? MaxLength
        {
            get { return _maxLength; }
            set 
            {
                _maxLength = value;
                OnPropertyChanged("MaxLength");
            }
        }

        private bool? _unicode;
        [DisplayVisibleCondition("IsString")]
        public bool? Unicode
        {
            get { return _unicode; }
            set 
            {
                _unicode = value;
                OnPropertyChanged("Unicode");
            }
        }

        private bool? _fixedLength;
        [DisplayVisibleCondition("IsString")]
        [DisplayName("Fixed Length")]
        public bool? FixedLength
        {
            get { return _fixedLength; }
            set 
            {
                _fixedLength = value;
                OnPropertyChanged("FixedLength");
            }
        }

        public bool IsDecimal
        {
            get 
            {
                switch (Type)
                {
                    case PropertyType.Decimal:
                    case PropertyType.Double:
                    case PropertyType.Single:
                        return true;
                }
                return false;
            }
        }

        private int? _precision;
        [DisplayVisibleCondition("IsDecimal")]
        public int? Precision
        {
            get { return _precision; }
            set 
            {
                _precision = value;
                OnPropertyChanged("Precision");
            }
        }

        private int? _scale;
        [DisplayVisibleCondition("IsDecimal")]
        public int? Scale
        {
            get { return _scale; }
            set 
            { 
                _scale = value;
                OnPropertyChanged("Scale");
            }
        }

        private ConcurrencyMode _concurrencyMode;
        [DisplayName("Concurrency Mode")]
        public ConcurrencyMode ConcurrencyMode
        {
            get { return _concurrencyMode; }
            set 
            {
                _concurrencyMode = value;
                OnPropertyChanged("ConcurrencyMode");
            }
        }

        private string _defaultValue;
        [DisplayName("Default Value")]
        public string DefaultValue
        {
            get { return _defaultValue; }
            set 
            {
                _defaultValue = value;
                OnPropertyChanged("DefaultValue");
            }
        }

        private string _collation;
        public string Collation
        {
            get { return _collation; }
            set 
            {
                _collation = value;
                OnPropertyChanged("Collation");
            }
        }

        protected override PropertyBase Create()
        {
            return new ScalarProperty();
        }
        internal override PropertyBase Duplicate()
        {
            var value =  (ScalarProperty)base.Duplicate();
            value.IsKey = IsKey;
            value.Type = Type;
            value.Nullable = Nullable;
            value.SetVisibility = SetVisibility;
            value.MaxLength = MaxLength;
            value.Unicode = Unicode;
            value.FixedLength = FixedLength;
            value.Precision = Precision;
            value.Scale = Scale;
            value.ConcurrencyMode = ConcurrencyMode;
            value.DefaultValue = DefaultValue;
            value.Collation = Collation;
            return value;
        }
    }
}
