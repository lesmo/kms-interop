using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMS.Comm.InnerCore {
    public abstract class ICoreCommandContent<T> : ICoreCommandContent {
        public T Content
            = default(T);

        public ICoreCommandContent() {
        }
        public ICoreCommandContent(T content) {
            this.Content = content;
        }
        public ICoreCommandContent(byte[] contentBytes) {
            this.Deserialize(contentBytes);
        }

        /// <summary>
        /// Serializa el contenido del Comando
        /// </summary>
        /// <returns>Arreglo en Bytes del Contenido</returns>
        public abstract byte[] Serialize();

        /// <summary>
        /// Deserializa el contenido del Comando
        /// </summary>
        /// <returns></returns>
        public abstract void Deserialize(byte[] input);
    }
}
