﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skrypt {
    public class Variable {
        public string Name { get; set; }
        public BaseObject Value { get; set; }
        public bool IsConstant { get; set; }

        public IScoped Scope { get; set; }

        public Variable (string name) {
            Name = name;
            Value = default(BaseObject);
        }

        public Variable (string name, IScoped scope) : this(name) {
            Scope = scope;
        }

        public Variable(string name, BaseObject value) : this(name) {
            Value = value;
        }

        public Variable(string name, BaseObject value, IScoped scope) : this(name, value) {
            Scope = scope;
        }
    }
}
