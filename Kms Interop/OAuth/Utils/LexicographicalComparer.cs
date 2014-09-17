using System;
using System.Collections.Generic;

namespace KMS.Interop.OAuth.Utils {

    public class LexicographicalComparer : IComparer<String> {
        public int Compare(string x, string y) {
            return String.Compare(x, y, StringComparison.Ordinal);
        }
    }

}