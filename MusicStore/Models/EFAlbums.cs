using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicStore.Models
{
    public class EFAlbums : IAlbumsMock
    {
        //db connection moved here from Albums controller
        private MusicStoreModel db = new MusicStoreModel();

        public IQueryable<Album> Albums { get { return db.Albums; } }
        public IQueryable<Artist> Artists { get { return db.Artists; } }
        public IQueryable<Genre> Genres { get { return db.Genres; } }

        public void Delete(Album album)
        {
            db.Albums.Remove(album);
            db.SaveChanges();
        }

        public Album Save(Album album)
        {
            if(album.AlbumId == 0)
            {
                db.Albums.Add(album);
            }
            else
            {
                db.Entry(album).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            return album;
        }
    }
}