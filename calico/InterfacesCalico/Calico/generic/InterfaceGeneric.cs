using Nini.Config;
using System;
using System.Collections.Generic;

namespace InterfacesCalico
{
    public interface InterfaceGeneric
    {
        bool process(IConfigSource source, DateTime? dateTime);
    }
}
