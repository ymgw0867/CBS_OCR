﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBS_OCR.common
{
    public interface IMaster
    {
        T GetData<T>(string id);

        List<T> Read<T>();
    }
}
