﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBoxScript {
    public class StringInstance : BaseInstance {
        public override string Name => "String";

        public string Value { get; set; }

        public StringInstance(Engine engine, string value) : base(engine) {
            Value = value;
        }

        public override string ToString() {
            return Value.ToString();
        }
    }
}
