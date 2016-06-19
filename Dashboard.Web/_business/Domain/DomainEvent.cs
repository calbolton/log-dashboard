using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Web._business.Domain
{
    /// <summary>
    /// Represents a base domain event (inherited events will be raised)
    /// </summary>
    [Serializable]
    public class DomainEvent
    {
        // Required for JSON
        protected DomainEvent()
        {

        }

        /// <summary>
        /// Create a new domain event
        /// </summary>
        /// <param name="name">The name of the event</param>
        /// <param name="type">The type of event</param>
        /// <param name="userId">The user that raised the event</param>
        protected DomainEvent(string name, string type, string userId)
        {
            Type = type;
            UserId = userId;
            Name = name;
            DateTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// The name of the event
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// The type of event
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The date and time the event occurred
        /// </summary>
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// The user that raised the event
        /// </summary>
        public string UserId { get; set; }
    }
}