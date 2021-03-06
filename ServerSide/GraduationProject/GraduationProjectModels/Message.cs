﻿using System.Collections.Generic;

namespace GraduationProjectModels
{
    // Message event.
    public class Message: IEntity<Message>
    {
        //Id of recipient.
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
