using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMS.Interop.OAuth.Utils {
    public class LexicographicalComparer : IComparer<String> {
        public int Compare(string x, string y) {
            return String.Compare(x, y, StringComparison.Ordinal);
        }
    }
}
