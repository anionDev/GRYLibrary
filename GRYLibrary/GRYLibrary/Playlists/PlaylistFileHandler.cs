using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRYLibrary.Core.Playlists
{

    public abstract class PlaylistFileHandler
    {
        public Encoding Encoding { get; set; } = new UTF8Encoding(false);
        public abstract void CreatePlaylist(string file);
        public abstract (ISet<string> included, ISet<string> excluded) GetItemsAndExcludedItems(string playlistFile);
        public virtual ISet<string> GetSongs(string playlistFile) {
            var x = GetItemsAndExcludedItems(playlistFile);
            return x.included.Except(x.excluded).ToHashSet();
        }
        public abstract void AddItemsToPlaylist(string playlistFile, IEnumerable<string> newItems);
        public abstract void DeleteItemsFromPlaylist(string playlistFile, IEnumerable<string> itemsToDelete);



    }
}
