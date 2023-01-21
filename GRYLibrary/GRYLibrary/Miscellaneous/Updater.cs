﻿using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GRYLibrary.Core.Miscellaneous
{
    public class Updater
    {
        private readonly string _CurrentLocation;
        private readonly string _AppName;
        private readonly IList<string> _Locations;
        private readonly Func<Version> _GetLatestVersion;
        private readonly Func<byte[]> _GetArchiveOfLatestVersion;
        private byte[] _ArchiveOfLatestVersion = null;
        public string CommandlineArgumentsForNewInstance { get; set; } = null;
        public Updater(IList<string> locations, Func<Version> getLatestVersion, Func<byte[]> getArchiveOfLatestVersion)
        {
            this._CurrentLocation = GetCurrentLocation();
            this._AppName = GetAppName();
            this._Locations = locations;
            _GetLatestVersion = getLatestVersion;
            _GetArchiveOfLatestVersion = getArchiveOfLatestVersion;
        }

        private string GetAppName()
        {
            return Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        private string GetCurrentLocation()
        {
           return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public void Update()
        {
            if (!_Locations.Contains(_CurrentLocation))
            {
                throw new ArgumentException("The current location must be contained in the list of all locations.");
            }
            int minimumRequiredAmount = 2;
            if (_Locations.Count < minimumRequiredAmount)
            {
                throw new ArgumentException($"At least {minimumRequiredAmount} locations are required.");
            }
            Version latestVersion = _GetLatestVersion();
            Version versionOfCurrentLocation = GetVersionOfLocation(_CurrentLocation);
            bool currentVersionIsOutdated = !latestVersion.Equals(versionOfCurrentLocation);
            string locationForRestart = null;
            foreach (var location in _Locations)
            {
                if (location != _CurrentLocation)
                {
                    Version versionOfLocation = GetVersionOfLocation(location);
                    if (latestVersion.Equals(location))
                    {
                        UpdateLocation(location);
                    }
                }
                locationForRestart = location;
            }
            if (currentVersionIsOutdated)
            {
                StartLocation(locationForRestart);
                StopCurrentLocation();
            }
        }


        private Version GetVersionOfLocation(string location)
        {
            return GetVersionOfDLLFile(GetProductDLLFile(location));
        }

        private string GetProductDLLFile(string location)
        {
            return Path.Combine(location, $"{_AppName}.dll");
        }

        private Version GetVersionOfDLLFile(string file)
        {
            return new Version(FileVersionInfo.GetVersionInfo(file).ProductVersion);
        }

        private void StopCurrentLocation()
        {
            Environment.Exit(0);
        }

        private void StartLocation(string location)
        {
            var filename = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var externalProgramExecutor = new ExternalProgramExecutor(Path.Combine(location, filename), CommandlineArgumentsForNewInstance);
            externalProgramExecutor.Configuration.WaitingState = new RunAsynchronously();
            externalProgramExecutor.Run();
        }

        private void UpdateLocation(string location)
        {
            DownloadLatestVersionIfRequired();
            throw new NotImplementedException();//TODO replace program in "location" by "_ArchiveOfLatestVersion"
        }

        private void DownloadLatestVersionIfRequired()
        {
            if (_ArchiveOfLatestVersion == null)
            {
                _ArchiveOfLatestVersion = _GetArchiveOfLatestVersion();
            }
        }
    }
}