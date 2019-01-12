﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skrypt.ANTLR;
using Antlr4.Runtime;

namespace Skrypt {
    public class Member {
        public BaseObject Value;
        public bool IsPrivate;
        public RuleContext DefinitionBlock;

        public Member(BaseObject v, bool isPrivate, RuleContext block) {
            Value = v;
            IsPrivate = isPrivate;
            DefinitionBlock = block;
        }
    }
}
