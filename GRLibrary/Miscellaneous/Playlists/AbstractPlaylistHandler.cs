﻿using GRLibrary.Miscellaneous.Playlists.ConcretePlaylistHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace GRLibrary.Miscellaneous.Playlists
{
    public abstract class AbstractPlaylistHandler
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        protected abstract IEnumerable<string> GetSongsFromPlaylistImplementation(string playlistFile);
        protected abstract void AddSongsToPlaylistImplementation(string playlistFile, IEnumerable<string> newSongs);
        protected abstract void DeleteSongsFromPlaylistImplementation(string playlistFile, IEnumerable<string> songsToDelete);
        public IEnumerable<string> GetSongsFromPlaylist(string playlistFile, bool removeDuplicatedItems = true, bool loadTransitively = true)
        {
            string locationOfFile = Path.GetDirectoryName(playlistFile);
            IEnumerable<string> referencedFiles = GetSongsFromPlaylistImplementation(playlistFile).Where(item => IsAllowedAsPlaylistItem(item));
            List<string> newList = new List<string>();
            foreach (string item in referencedFiles)
            {
                try
                {
                    string playlistItem;
                    if (Path.IsPathRooted(item))
                    {
                        playlistItem = item;
                    }
                    else
                    {
                        playlistItem = Path.GetFullPath(Path.Combine(locationOfFile, item));
                    }
                    //TODO: here is a bug: if the file "a.m3u" contains the line "a.m3u" (transitively) this operation may cause an endless-loop
                    if (IsReadablePlaylist(playlistItem.ToLower()))
                    {
                        if (loadTransitively)
                        {
                            newList.AddRange(ExtensionsOfReadablePlaylists[Path.GetExtension(playlistItem.ToLower()).Substring(1)].GetSongsFromPlaylist(playlistItem, removeDuplicatedItems, loadTransitively));
                        }
                    }
                    else
                    {
                        newList.Add(playlistItem);
                    }
                }
                catch
                {
                    Utilities.NoOperation();
                }
                referencedFiles = newList;
            }
            if (removeDuplicatedItems)
            {
                referencedFiles = new HashSet<string>(referencedFiles);
            }
            return referencedFiles;
        }

        public static bool IsReadablePlaylist(string file)
        {
            return ExtensionsOfReadablePlaylists.ContainsKey(Path.GetExtension(file.ToLower()).Substring(1));
        }
        public static Dictionary<string, AbstractPlaylistHandler> ExtensionsOfReadablePlaylists = new Dictionary<string, AbstractPlaylistHandler>() { { "m3u", new M3UHandler() }, { "pls", new PLSHandler() }, { "wpl", new WPLHandler() } };
        public void AddSongsToPlaylist(string playlistFile, IEnumerable<string> newSongs)
        {
            AddSongsToPlaylistImplementation(playlistFile, newSongs.Where(item => IsAllowedAsPlaylistItem(item)));
        }
        public void DeleteSongsFromPlaylist(string playlistFile, IEnumerable<string> songsToDelete)
        {
            DeleteSongsFromPlaylistImplementation(playlistFile, songsToDelete.Where(item => IsAllowedAsPlaylistItem(item)));
        }
        public static bool IsAllowedAsPlaylistItem(string item)
        {
            return !item.Equals(string.Empty);
        }
    }
}

