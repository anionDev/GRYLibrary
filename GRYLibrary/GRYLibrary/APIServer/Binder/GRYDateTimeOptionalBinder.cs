using GRYLibrary.Core.Miscellaneous;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Binder
{
    public class GRYDateTimeOptionalBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            return GUtilities.GenericModelBinder(value => GRYDateTime.FromString(value), nameof(GRYDateTime), false)(bindingContext);
        }
    }
}
