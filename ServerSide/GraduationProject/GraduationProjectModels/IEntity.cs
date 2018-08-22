using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectModels
{
    public interface IEntity<T>
    {
        long Id { get; set; }

        bool Validate();

        void Edit(T param);
    }
}
