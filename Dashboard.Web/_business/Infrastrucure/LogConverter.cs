using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FileHelpers;

namespace Dashboard.Web._business.Infrastrucure
{
    public class LogConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            return from;
        }

        public override string FieldToString(object fieldValue)
        {
            return fieldValue.ToString();
        }
    }
}