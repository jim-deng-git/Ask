using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkV3.Exceptions
{
    [Serializable]
    public class SiteNullException : Exception
    {
        public SiteNullException() { }
        public SiteNullException(string message) : base(message) { }
        public SiteNullException(string message, Exception inner) : base(message, inner) { }
        protected SiteNullException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}