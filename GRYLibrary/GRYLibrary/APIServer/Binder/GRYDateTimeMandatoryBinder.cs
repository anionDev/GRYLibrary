﻿using GRYLibrary.Core.Misc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using GUtilities = GRYLibrary.Core.Misc.Utilities;
using System.Threading.Tasks;

namespace GRYLibrary.Core.APIServer.Binder
{
    public class GRYDateTimeMandatoryBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext) => GUtilities.GenericModelBinder(value => GRYDateTime.FromString(value), nameof(GRYDateTime), true)(bindingContext);
    }
}
