using System;
using SQLite;
using TheListSellerAppXamariniOS.Views.Reels;

namespace TheListSellerAppXamariniOS.Data.Database
{
    public class AppDbContext
    {
        public readonly SQLiteConnection _database;

        public AppDbContext(string dbPath)
        {
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Reel>();
        }
    }
}


