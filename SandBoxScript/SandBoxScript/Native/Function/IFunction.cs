﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBoxScript {
    public interface IFunction {
        BaseObject Run(Engine engine, BaseValue self, BaseValue[] args);
    }
}
