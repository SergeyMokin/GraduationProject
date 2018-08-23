using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectModels
{
    // Interface of class what contains in database.
    public interface IEntity<T>
    {
        long Id { get; set; }

        bool Validate();

        void Edit(T param);
    }
}
