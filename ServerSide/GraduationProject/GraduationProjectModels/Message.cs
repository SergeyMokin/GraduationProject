using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectModels
{
    public class Message: IEntity<Message>
    {
        public long Id { get; set; }

        public IEnumerable<long> FileIds { get; set; }

        public bool Validate()
        {
            return true;
        }

        public void Edit(Message message)
        {

        }
    }
}
