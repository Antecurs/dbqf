﻿using System;
using System.Collections.Generic;
using System.Text;

namespace dbqf.Criterion.Values
{
    public class BetweenValue
    {
        public object From { get; set; }
        public object To { get; set; }

        public BetweenValue()
        {
        }
        public BetweenValue(object from, object to)
        {
            From = from;
            To = to;
        }

        public override string ToString()
        {
            return String.Format("{0} to {1}", (From != null ? From.ToString() : "minimum"), (To != null ? To.ToString() : "maximum"));
        }
    }
}
