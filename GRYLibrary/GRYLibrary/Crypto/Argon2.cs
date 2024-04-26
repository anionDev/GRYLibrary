﻿using System.Text;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GRYLibrary.Core.Crypto
{
    public class Argon2 : HashAlgorithm
    {
        public override byte[] Hash(byte[] data)
        {
            throw new NotImplementedException();
        }
        public override byte[] GetIdentifier()
        {
            return GUtilities.PadLeft(Encoding.ASCII.GetBytes(nameof(Argon2)), 10);
        }
    }
}