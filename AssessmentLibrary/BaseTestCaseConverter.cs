﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace AssessmentLibrary
{
    public class BaseTestCaseConverter : CustomCreationConverter<BaseTestCase>
    {
        public override BaseTestCase Create(Type objectType)
        {
            
            //this.
            throw new NotImplementedException();
        }
    }
}
