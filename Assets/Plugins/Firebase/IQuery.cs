using UnityEngine;
using System.Collections;
using System;

public interface IQuery  {
	event EventHandler<ChangedEventArgs> ValueUpdated;
	event EventHandler<ChangedEventArgs> ChildAdded;

	event EventHandler<ErrorEventArgs> Error;
}

public class ChangedEventArgs : EventArgs
{
	public IDataSnapshot DataSnapshot { get; set; }
}

public class ErrorEventArgs : EventArgs 
{
	public IFirebaseError Error { get; set; }
}

