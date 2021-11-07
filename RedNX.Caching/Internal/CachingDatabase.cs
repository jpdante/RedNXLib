using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using RedNX.Extensions.System;

namespace RedNX.Caching.Internal {
    public class CachingDatabase {

        private readonly string _connectionString;

        public CachingDatabase(string path) {
            var databasePath = Path.GetFullPath(path);
            _connectionString = $"Data Source={databasePath}";
            EnsureCreated();
        }

        private void EnsureCreated() {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS file_cache (id TEXT PRIMARY KEY, file_name TEXT(32) NOT NULL UNIQUE, size INTEGER NOT NULL, creation_date INTEGER NOT NULL, last_access INTEGER NOT NULL);";
            cmd.ExecuteNonQuery();
        }

        internal void AddEntry(FileEntry entry) {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO file_cache (id, file_name, size, creation_date, last_access) VALUES (@id, @file_name, @size, @creation_date, @last_access);";
            cmd.Parameters.AddWithValue("id", entry.Id);
            cmd.Parameters.AddWithValue("file_name", entry.FileName);
            cmd.Parameters.AddWithValue("size", entry.Size);
            cmd.Parameters.AddWithValue("creation_date", entry.CreationDate);
            cmd.Parameters.AddWithValue("last_access", entry.LastAccess);
            cmd.ExecuteNonQuery();
        }

        internal void UpdateLastAcess(string id, DateTime lastAccess) => UpdateLastAcess(id, lastAccess.ToUnixEpoch());

        internal void UpdateLastAcess(string id, int lastAccess) {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE file_cache SET last_access = @last_access WHERE id = @id;";
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("last_access", lastAccess);
            cmd.ExecuteNonQuery();
        }

        internal FileEntry GetEntry(string id) {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT file_name, size, creation_date, last_access FROM file_cache WHERE id = @id;";
            cmd.Parameters.AddWithValue("id", id);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            var fileName = reader.GetString(0);
            var size = reader.GetInt64(1);
            var creationDate = reader.GetInt32(2);
            var lastAccess = reader.GetInt32(3);
            var fileEntry = new FileEntry(id, fileName, size, creationDate, lastAccess);
            return fileEntry;
        }

        internal IReadOnlyList<FileEntry> GetEntries() {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, file_name, size, creation_date, last_access FROM file_cache;";
            using var reader = cmd.ExecuteReader();
            var entries = new List<FileEntry>();
            while (reader.Read()) {
                var id = reader.GetString(0);
                var fileName = reader.GetString(1);
                var size = reader.GetInt64(2);
                var creationDate = reader.GetInt32(3);
                var lastAccess = reader.GetInt32(4);
                entries.Add(new FileEntry(id, fileName, size, creationDate, lastAccess));
            }
            return entries;
        }

        internal void RemoveEntry(string id) {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM file_cache WHERE id = @id;";
            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();
        }

        internal bool ContainsEntry(string id) {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id FROM file_cache WHERE id = @id;";
            cmd.Parameters.AddWithValue("id", id);
            using var reader = cmd.ExecuteReader();
            return reader.HasRows;
        }

        internal IReadOnlyList<string> GetLargestSize(long requiredSpace) {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, size FROM file_cache ORDER BY size DESC;";
            long availableSpace = 0;
            var entries = new List<string>();
            using var reader = cmd.ExecuteReader();
            while (requiredSpace > availableSpace && reader.Read()) {
                entries.Add(reader.GetString(0));
                availableSpace += reader.GetInt64(1);
            }
            return entries;
        }

        internal long SumSize() {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT size FROM file_cache;";
            long sumSize = 0;
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                sumSize += reader.GetInt64(0);
            }
            return sumSize;
        }

        internal void Clear() {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM file_cache;";
            cmd.ExecuteNonQuery();
        }
    }
}