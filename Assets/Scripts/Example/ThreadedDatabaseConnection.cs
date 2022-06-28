using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Diagnostics;

namespace Example
{
  // This class is responsible for managing a database connection in a thread.
  // Only 1 threaded connection should be instantiated in your application.
  // This class uses an atomic FIFO queue to get queries into the database thread (you can't directly invoke a function inside the database thread).
  // This class uses callback functions to get queried data to the application thread. 
  public class ThreadedDatabaseConnection
  {
    private readonly string DatabaseFilePath;
    private readonly Thread DatabaseThread;
    private volatile bool Running;

    public ConcurrentQueue<object> DatabaseQueryQueue = new ConcurrentQueue<object>();
    
    public ThreadedDatabaseConnection(string DatabaseFilePath)
    {
      this.DatabaseFilePath = DatabaseFilePath;

      this.DatabaseThread = new Thread(Run);
      this.DatabaseThread.Name = "Database Thread";
      this.DatabaseThread.Start();

      Running = true;
    }

    private async void Run(object obj)
    {
      UnityEngine.Debug.Log("Database connection thread starting");

      while (Running)
      {
        if (!Database.Instance.IsConnected())
        {
          UnityEngine.Debug.Log("Connecting to database");

          await Database.Instance.Connect(DatabaseFilePath);

          UnityEngine.Debug.Log("Database connected");
        }
        else
        {
          await TryRunQueuedQueriesInThread();
        }

        Thread.Sleep(0); // Don't toaster CPU
      }

      UnityEngine.Debug.Log("Database thread stopped");
    }

    private async Task TryRunQueuedQueriesInThread()
    {
      object ThreadedQuery;
      if (DatabaseQueryQueue.TryDequeue(out ThreadedQuery))
      {
        try
        {
          await TryRunQueuedQuery(ThreadedQuery);
        }
        catch (Exception exception)
        {
          UnityEngine.Debug.Log("Failed to dequeue query");
          UnityEngine.Debug.LogError(exception);
        }
      }
    }

    private async Task TryRunQueuedQuery(object query)
    {
      Type type = query.GetType();

      // Using if/elseif since switch didn't support Type and may or may not work for this version of c# 4.x
      // https://stackoverflow.com/questions/4478464/c-sharp-switch-on-type
      // I suspect there's a better way of doing this, but this is working for now.
      if (type == typeof(ThreadedQuery<int>))
      {
        await QueryInt(query);
      }
      else if (type == typeof(ThreadedQuery<string>))
      {
        await QueryString(query);
      }
      else if (type == typeof(ThreadedQuery<List<CollectionDBSchema>>))
      {
        await QueryCollection(query);
      }
      else
      {
        UnityEngine.Debug.LogWarning($"Unable to execute query: {((ThreadedQuery<int>)query).query}");
      }
    }

    public async Task QueryInt(object query)
    {
      var ThreadedQuery = (ThreadedQuery<int>)query;
      UnityEngine.Debug.Log($"Running query: {ThreadedQuery.query}");

      var result = await Database.Instance.ScalarIntQuery(ThreadedQuery.query);

      ThreadedQuery.callback(result);
    }

    public async Task QueryString(object query)
    {
      var ThreadedQuery = (ThreadedQuery<string>)query;
      UnityEngine.Debug.Log($"Running query: {ThreadedQuery.query}");

      var result = await Database.Instance.ScalarStringQuery(ThreadedQuery.query);

      if (ThreadedQuery.callback != null)
      {
        ThreadedQuery.callback(result);
      }
    }

    public async Task QueryCollection(object query)
    {
      var ThreadedQuery = (ThreadedQuery<List<CollectionDBSchema>>)query;
      UnityEngine.Debug.Log($"Running query: {ThreadedQuery.query}");

      List<CollectionDBSchema> result = await Database.Instance.QueryCollection<CollectionDBSchema>(ThreadedQuery.query);

      ThreadedQuery.callback(result);
    }

    public void QueueQuery<T>(string query)
    {
      UnityEngine.Debug.Log($"Queing the query: {query}");

      DatabaseQueryQueue.Enqueue(new ThreadedQuery<T>(query));
    }

    public void QueueQuery<T>(string query, Action<T> callback)
    {
      UnityEngine.Debug.Log($"Queing the query w/ a callback: {query}");

      DatabaseQueryQueue.Enqueue(new ThreadedQuery<T>(query, callback));
    }

    public void Close()
    {
      Running = false;
    }
  }
}