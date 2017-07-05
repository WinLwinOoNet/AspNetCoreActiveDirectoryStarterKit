using System;

namespace Asp.Core
{
    public class DateTimeAdapter : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
