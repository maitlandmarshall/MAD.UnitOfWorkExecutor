using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.UnitOfWorkExecutor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UnitOfWorkAttribute : Attribute
    {
        internal UnitOfWorkType WorkType
        {
            get
            {
                if (string.IsNullOrEmpty(this.RunAtTime))
                    return UnitOfWorkType.Timed;

                return UnitOfWorkType.Scheduled;
            }
        }

        // Timed
        public int RunEverySeconds { get; set; } = 5;

        // Scheduled
        /// <summary>
        /// In the format of hh:mm i.e 22:30 for 10:30 PM.
        /// </summary>
        public string RunAtTime { get; set; }

    }
}
