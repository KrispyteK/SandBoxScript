﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skrypt {
    public sealed class Options {
        public bool DiscardGlobal { get; private set; }
        public int MaxRecursionDepth { get; private set; } = -1;
        public long MaxMemory { get; private set; }

        /// <summary>
        /// Gets rid of new global variables after executing code.
        /// </summary>
        public Options DiscardGlobalScope(bool discard = true) {
            DiscardGlobal = discard;
            return this;
        }

        /// <summary>
        /// Sets the maximum recursion depth.
        /// </summary>
        public Options LimitRecursion(int maxRecursionDepth) {
            MaxRecursionDepth = maxRecursionDepth;
            return this;
        }

        public Options LimitMemory(long memoryLimit) {
            MaxMemory = memoryLimit;
            return this;
        }
    }
}
