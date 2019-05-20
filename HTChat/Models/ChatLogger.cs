using agsXMPP;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTChat.Models
{
    public class ChatLogger
    {
        public const string LogFilename = "htchat.sqlite";
        public const string LogFolder = "HTChat";
        private static ChatLogger _instance;

        public SQLiteConnection DbConnection { get; private set; }

        string GetLogFilename() => Path.Combine(Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LogFolder)).FullName, LogFilename);

        private ChatLogger()
        {
            var dbFilename = GetLogFilename();
            
            if (!File.Exists(dbFilename))
            {
                SQLiteConnection.CreateFile(dbFilename);
            }
            DbConnection = new SQLiteConnection($"Data Source=\"{dbFilename}\";Version=3");
            DbConnection.Open();
            CreateSchema(DbConnection);
            
        }

        private void CreateSchema(SQLiteConnection conn)
        {
            new SQLiteCommand("CREATE TABLE IF NOT EXISTS chatlogs (from_jid VARCHAR(64), to_jid VARCHAR(64), timestamp INTEGER(8), message TEXT);", conn).ExecuteNonQuery();
            new SQLiteCommand("CREATE INDEX IF NOT EXISTS chatlogs_idx_from_jid ON chatlogs(from_jid);", conn).ExecuteNonQuery();
            new SQLiteCommand("CREATE INDEX IF NOT EXISTS chatlogs_idx_to ON chatlogs(to_jid);", conn).ExecuteNonQuery();
        }

        public IEnumerable<(Jid from, Jid to, DateTime timestamp, string message)> GetPreviousChats(Jid myJid, Jid theirJid, int offset, int count)
        {
            var cmd = new SQLiteCommand("SELECT * FROM chatlogs WHERE (from_jid=@from_jid AND to_jid=@to_jid) OR (from_jid=@to_jid AND to_jid=@from_jid) ORDER BY timestamp DESC LIMIT @count OFFSET @offset;", DbConnection);
            cmd.Parameters.Add(new SQLiteParameter("@from_jid", myJid.ToString()));
            cmd.Parameters.Add(new SQLiteParameter("@to_jid", theirJid.ToString()));
            cmd.Parameters.Add(new SQLiteParameter("@count", count));
            cmd.Parameters.Add(new SQLiteParameter("@offset", offset));
            var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                yield return ((string)reader["from_jid"], (string)reader["to_jid"], new DateTime((long)reader["timestamp"]), (string)reader["message"]);
            }            
        }

        public static ChatLogger Instance => _instance ?? (_instance = new ChatLogger());

        public async Task LogMessage(Jid from_jid, Jid to_jid, DateTime timestamp, string message)
        {
            var cmd = new SQLiteCommand("INSERT INTO chatlogs(from_jid, to_jid, timestamp, message) VALUES(@from_jid, @to_jid, @ts, @msg)", DbConnection);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Add(new SQLiteParameter("@from_jid", from_jid.ToString()));
            cmd.Parameters.Add(new SQLiteParameter("@to_jid", to_jid.ToString()));
            cmd.Parameters.Add(new SQLiteParameter("@ts", timestamp.Ticks));
            cmd.Parameters.Add(new SQLiteParameter("@msg", message));
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
