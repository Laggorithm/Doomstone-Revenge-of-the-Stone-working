using System;
using System.IO;
using System.Collections;
using UnityEngine;
using Mono.Data.Sqlite; // Ensure this is available in your project

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;
    private int currentElapsedTime;
     
    private const float saveInterval = 1f; // Interval in seconds

    private void Start()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "gameData.db");
        InitializeDatabase();
        Debug.Log(dbPath);

        if (dbPath == null)
        {
            Debug.Log("Path Error");
        }

        // Start the coroutine to save elapsed time every second
        StartCoroutine(SaveElapsedTimeCoroutine());
    }

    private void InitializeDatabase()
    {
        if (!File.Exists(dbPath))
        {
            CreateDatabase();
        }
        else
        {
            Debug.Log("Database already exists.");
        }
    }

    private void CreateDatabase()
    {
        using (var connection = new SqliteConnection("Data Source=" + dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS PlayerData (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ElapsedTime INTEGER
                    );
                ";
                command.ExecuteNonQuery();
            }

            Debug.Log("Database and table created.");
        }
    }

    private IEnumerator SaveElapsedTimeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(saveInterval);
            SaveElapsedTime(currentElapsedTime);
        }
    }

    public void SetElapsedTime(int elapsedTime)
    {
        currentElapsedTime = elapsedTime;
    }

    public void SaveElapsedTime(int elapsedTime)
    {
        int savedElapsedTime = LoadElapsedTime();

        if (elapsedTime > savedElapsedTime)
        {
            using (var connection = new SqliteConnection("Data Source=" + dbPath))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO PlayerData (ElapsedTime)
                        VALUES (@elapsedTime);
                    ";
                    command.Parameters.AddWithValue("@elapsedTime", elapsedTime);
                    command.ExecuteNonQuery();
                }

                Debug.Log("ElapsedTime updated in SQLite database.");
            }
        }
        else
        {
            Debug.Log("Current elapsed time is not greater than saved elapsed time. No update needed.");
        }
    }

    public int LoadElapsedTime()
    {
        int elapsedTime = 0;

        using (var connection = new SqliteConnection("Data Source=" + dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT ElapsedTime
                    FROM PlayerData
                    ORDER BY Id DESC
                    LIMIT 1;
                ";
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        elapsedTime = reader.GetInt32(0);
                    }
                }
            }
        }

        Debug.Log("ElapsedTime loaded from SQLite database.");
        return elapsedTime;
    }
}
