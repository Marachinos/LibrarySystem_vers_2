using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem.Core.Interfaces;

public interface ISearchable
{
    bool Matches(string searchTerm);
}
