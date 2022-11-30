using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous
{
    public class Updater
    {
        private readonly string _BackendAddress;
        private readonly bool _VerifyBackend;
        private readonly string _CurrentLocation;
        private readonly string _AppName;
        private readonly IList<string> _Locations;
        public Updater(string backendAddress, IList<string> locations, bool verifyBackend)
        {
            this._BackendAddress = backendAddress;
            this._CurrentLocation = GetCurrentLocation();
            this._AppName = GetAppName();
            this._Locations = locations;
            this._VerifyBackend = verifyBackend;
        }

        private string GetAppName()
        {
            throw new NotImplementedException();
        }

        private string GetCurrentLocation()
        {
            throw new NotImplementedException();
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
            Version latestVersion = GetLatestVersion();
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

        private Version GetLatestVersion()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private void UpdateLocation(string location)
        {
            throw new NotImplementedException();
        }

        private byte[] GetArchiveOfLatestVersion()
        {
            throw new NotImplementedException();
        }
    }
}
