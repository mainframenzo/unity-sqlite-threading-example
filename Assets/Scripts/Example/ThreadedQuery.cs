using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example
{
  public class ThreadedQuery<T>
  {
    public System.Type type;
    public string query;
    public Action<T> callback;

    public ThreadedQuery(string query)
    {
      this.type = typeof(T);
      this.query = query;
      this.callback = (T) => { };
    }

    public ThreadedQuery(string query, Action<T> callback)
    {
      this.type = typeof(T);
      this.query = query;
      this.callback = callback;
    }
  }
}
