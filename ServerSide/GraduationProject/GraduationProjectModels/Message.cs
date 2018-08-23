using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectModels
{
    // Message event.
    public class Message: IEntity<Message>
    {
        public long Id { get; set; }

        public IEnumerable<long> FileIds { get; set; }

        public bool Validate()
        {
            return FileIds != null;
        }

        public void Edit(Message message)
        {
            FileIds = message.FileIds;
        }
    }
}
