// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Property
{
    public class Property : EDMObjectBase
    {
        #region Fields

        private string _type;
        private bool _isKey;
        private bool? _nullable;
        private int? _maxLength;
        private bool? _fixedLength;
        private int? _precision;
        private int? _scale;
        private string _collation;
        private string _defaultValue;

        #endregion

        #region Properties

        public EntityType.EntityType EntityType { get; private set; }
        public bool? Unicode { get; set; }
        public StoreGeneratedPattern? StoreGeneratedPattern { get; set; }
        public string StoreName { get; set; }
        public string StoreSchema { get; set; }
        public string StoreType { get; set; }

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public bool IsKey
        {
            get { return _isKey; }
            set
            {
                _isKey = value;
                OnPropertyChanged("IsKey");
            }
        }

        public bool? Nullable
        {
            get { return _nullable; }
            set
            {
                _nullable = value;
                OnPropertyChanged("Nullable");
            }
        }

        public int? MaxLength
        {
            get { return _maxLength; }
            set
            {
                _maxLength = value;
                OnPropertyChanged("MaxLength");
            }
        }

        public bool? FixedLength
        {
            get { return _fixedLength; }
            set
            {
                _fixedLength = value;
                OnPropertyChanged("FixedLength");
            }
        }

        public int? Precision
        {
            get { return _precision; }
            set
            {
                _precision = value;
                OnPropertyChanged("Precision");
            }
        }

        public int? Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                OnPropertyChanged("Scale");
            }
        }

        public string Collation
        {
            get { return _collation; }
            set
            {
                _collation = value;
                OnPropertyChanged("Collation");
            }
        }

        public string DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                _defaultValue = value;
                OnPropertyChanged("DefaultValue");
            }
        }

        #endregion

        #region Constructor

        public Property(EntityType.EntityType entityType)
        {
            EntityType = entityType;
        }

        #endregion
    }
}
