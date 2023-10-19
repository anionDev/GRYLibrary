﻿using GRYLibrary.Core.Miscellaneous;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Binder
{
    public class GRYDateTimeMandatoryBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            return GUtilities.GenericModelBinder(value => GRYDateTime.FromString(value), nameof(GRYDateTime), true)(bindingContext);
        }
    }
}
