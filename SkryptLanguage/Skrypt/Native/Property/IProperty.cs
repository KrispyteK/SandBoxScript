﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skrypt {
    public interface IGetProperty {
        SkryptObject Run(SkryptEngine engine, SkryptObject self); 
    }
}
