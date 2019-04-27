﻿using System;
using System.Collections.Generic;
using System.Text;

namespace InterfacesCalico
{
    public interface InterfaceGeneric
    {
        void sendRequest(String url, List<String> parameters);

        String process(String url, List<String> parameters, DateTime dateLast);
    }
}