using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Pandit
{
  

    public static class PanditErrors
    {
        

        public static Error NotFound(Guid panditId) => Error.NotFound(
            "Pandit.NotFound",
            $"The pandit with the Id = '{panditId}' was not found");
    }
}
