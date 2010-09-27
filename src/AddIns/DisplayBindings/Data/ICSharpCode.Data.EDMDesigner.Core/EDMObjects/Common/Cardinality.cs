// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common
{
    public enum Cardinality
    {
        One,
        ZeroToOne,
        Many
    }

    public static class CardinalityStringConverter
    {
        public static Cardinality CardinalityFromString(string cardinality)
        {
            switch (cardinality)
            {
                case "1":
                    return Cardinality.One;
                case "0..1":
                    return Cardinality.ZeroToOne;
                case "*":
                    return Cardinality.Many;
            }

            throw new NotImplementedException();
        }

        public static string CardinalityToString(Cardinality cardinality)
        {
            switch (cardinality)
            {
                case Cardinality.One:
                    return "1";
                case Cardinality.ZeroToOne:
                    return "0..1";
                case Cardinality.Many:
                    return "*";
            }

            throw new NotImplementedException();
        }
    }
}
