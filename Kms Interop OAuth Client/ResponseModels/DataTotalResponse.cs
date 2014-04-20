using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kms.Interop.CloudClient.ResponseModels {
    public class DataTotalResponse {
        /// <summary>
        ///     Distancia total alcanzada corriendo.
        /// </summary>
        public long RunningTotalDistance {
            get;
            set;
        }
        /// <summary>
        ///     Distancia total alcanzada caminando.
        /// </summary>
        public long WalkingTotalDistance {
            get;
            set;
        }
        /// <summary>
        ///     Distancia total alcanzada.
        /// </summary>
        public long TotalDistance {
            get;
            set;
        }

        /// <summary>
        ///     Pasos totales dados corriendo.
        /// </summary>
        public long RunningTotalSteps {
            get;
            set;
        }
        /// <summary>
        ///     Pasos totales dados caminando.
        /// </summary>
        public long WalkingTotalSteps {
            get;
            set;
        }
        /// <summary>
        ///     Pasos totales dados.
        /// </summary>
        public long TotalSteps {
            get;
            set;
        }

        /// <summary>
        ///     Fecha del último dato registrado.
        /// </summary>
        public DateTime LastModified {
            get;
            set;
        }
    }
}
