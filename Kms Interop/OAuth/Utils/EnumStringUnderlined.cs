using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Kms.Interop.OAuth.Utils {
    public static class EnumExpansion {
        /// <summary>
        ///     Devuelve la representación de "UnaCadena" como "una_cadena" de
        ///     éste elemento de la enumeración
        /// </summary>
        public static string CamelCaseToUnderlineString(this Enum thisEnum) {
            return new Regex("([A-Z])").Replace(
                thisEnum.ToString(),
                "_$1"
            ).Substring(1).ToLower();
        }
    }
}
