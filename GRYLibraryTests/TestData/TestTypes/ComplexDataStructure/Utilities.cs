﻿using GRYLibrary.Core;
using System;

namespace GRYLibrary.TestData.TestTypes.ComplexDataStructure
{
    public static class Utilities
    {
        internal static string GetRandomPhoneNumber()
        {
            return Guid.NewGuid().ToString();
        }
        internal static DateTime GetRandomDateOfBirth()
        {
            DateTime result = new DateTime(1960, 1, 1);
            result = result.AddDays(_IdGenerator.GenerateNewId());
            result = result.AddSeconds(_IdGenerator.GenerateNewId());
            return result;
        }

        internal static string GetRandomWebsite()
        {
            return $"https://{GetRandomName()}.com";
        }
        private static readonly IdGenerator<int> _IdGenerator = IdGenerator.GetDefaultIntIdGenerator();
        internal static string GetRandomName()
        {
            return "label_" + _IdGenerator.GenerateNewId();
        }
        internal static string GetRandomNotes()
        {
            return GetRandomName();
        }

        internal static string GetRandomCountry()
        {
            return GetRandomName();
        }

        internal static string GetRandomStateProvince()
        {
            return GetRandomName();
        }

        internal static string GetRandomPostalCode()
        {
            return GetRandomName();
        }

        internal static string GetRandomFloorLevel()
        {
            return GetRandomName();
        }

        internal static string GetRandomCountryRegion()
        {
            return GetRandomName();
        }

        internal static string GetRandomCity()
        {
            return GetRandomName();
        }

        internal static string GetRandomBuilding()
        {
            return GetRandomName();
        }

        internal static string GetRandomAddressLine2()
        {
            return GetRandomName();
        }

        internal static string GetRandomAddressLine1()
        {
            return GetRandomName();
        }
    }
}