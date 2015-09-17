﻿using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using AOT;
using System.Collections.Generic;
using System.Threading;

#if UNITY_IOS
public class QueryiOSImpl : IQuery {
	IntPtr nativeReference;
	EventHandler<ChangedEventArgs> valueUpdatedEvent, childAddedEvent, childRemovedEvent, childChangedEvent, childMovedEvent;
	bool subscribedValueChange = false, subscribedChildAdded = false, subscribedChildRemoved = false,
			subscribedChildChanged = false, subscribedChildMoved = false;
	
	static onValueChangedEventHandler valueChangedHandler = new onValueChangedEventHandler(onValueChanged);
	static onValueChangedEventHandler childAddedHandler = new onValueChangedEventHandler(onChildAdded);
	static onValueChangedEventHandler childRemovedHandler = new onValueChangedEventHandler(onChildRemoved);
	static onValueChangedEventHandler childChangedHandler = new onValueChangedEventHandler(onChildChanged);
	static onValueChangedEventHandler childMovedHandler = new onValueChangedEventHandler(onChildMoved);

	static Dictionary<IntPtr, WeakReference> global_table = new Dictionary<IntPtr, WeakReference> ();
	static object _sync = new object ();

	public QueryiOSImpl(IntPtr nativeReference) {
		this.nativeReference = nativeReference;
		lock (_sync) {
			global_table[nativeReference] = new WeakReference(this);
		}
	}

	~QueryiOSImpl() {
		if (nativeReference != IntPtr.Zero) {
			lock(_sync) {
				global_table.Remove (nativeReference);
			}
		}
	}

	/// <summary>
	/// TODO: when we support Queries directly (not as Firebase*), we'll have to make sure
	/// this pointer gets stored as a Query versus Firebase as its vtable will be different.
	/// </summary>
	/// <returns>The OS object.</returns>
	protected IntPtr GetiOSObject() {
		return nativeReference;
	}

	internal static object GetCachedInstance(IntPtr reference) {
		lock (_sync) {
			WeakReference weakReference;
			if (global_table.TryGetValue(reference, out weakReference)) {
				return weakReference.Target;
			}
		}
		return null;
	}

	internal static T GetOrCreateCachedInstance<T>(IntPtr reference, Func<T> createFunction) where T:class {
		lock (_sync) {
			WeakReference weakReference;
			if (global_table.TryGetValue(reference, out weakReference)) {
				T result = weakReference.Target as T;
				if (result != null) {
					return result;
				}
			}
			return createFunction();
		}
	}

	public delegate void onValueChangedEventHandler( IntPtr dataSnapshot, IntPtr referenceId );
	
	[DllImport("__Internal")]
	private static extern void _FirebaseObserveValueChange( IntPtr firebase, onValueChangedEventHandler onChanged, IntPtr referenceId);

	[DllImport("__Internal")]
	private static extern void _FirebaseObserveChildAdded (IntPtr firebase, onValueChangedEventHandler onChanged, IntPtr referenceId);

	[DllImport("__Internal")]
	private static extern void _FirebaseObserveChildRemoved (IntPtr firebase, onValueChangedEventHandler onChanged, IntPtr referenceId);

	[DllImport("__Internal")]
	private static extern void _FirebaseObserveChildChanged (IntPtr firebase, onValueChangedEventHandler onChanged, IntPtr referenceId);

	[DllImport("__Internal")]
	private static extern void _FirebaseObserveChildMoved (IntPtr firebase, onValueChangedEventHandler onChanged, IntPtr referenceId);
	
	#region IQuery implementation
	public event EventHandler<ChangedEventArgs> ValueUpdated {
		add {
			valueUpdatedEvent += value;
			
			if (!subscribedValueChange) {
				subscribedValueChange = true;
				_FirebaseObserveValueChange(GetiOSObject(), valueChangedHandler, nativeReference);
			}
			
		}
		remove {
			valueUpdatedEvent -= value;
		}
	}

	public event EventHandler<ChangedEventArgs> ChildAdded {
		add {
			childAddedEvent += value;
			
			if (!subscribedChildAdded) {
				subscribedChildAdded = true;
				_FirebaseObserveChildAdded(GetiOSObject(), childAddedHandler, nativeReference);
			}
		}
		remove {
			childAddedEvent -= value;
		}
	}

	public event EventHandler<ChangedEventArgs> ChildRemoved {
		add {
			childRemovedEvent += value;
			
			if (!subscribedChildRemoved) {
				subscribedChildRemoved = true;
				_FirebaseObserveChildRemoved(GetiOSObject(), childRemovedHandler, nativeReference);
			}
		}
		remove {
			childRemovedEvent -= value;
		}
	}

	public event EventHandler<ChangedEventArgs> ChildChanged {
		add {
			childChangedEvent += value;
			
			if (!subscribedChildChanged) {
				subscribedChildChanged = true;
				_FirebaseObserveChildChanged(GetiOSObject(), childChangedHandler, nativeReference);
			}
		}
		remove {
			childChangedEvent -= value;
		}
	}

	public event EventHandler<ChangedEventArgs> ChildMoved {
		add {
			childMovedEvent += value;
			
			if (!subscribedChildMoved) {
				subscribedChildMoved = true;
				_FirebaseObserveChildMoved(GetiOSObject(), childMovedHandler, nativeReference);
			}
		}
		remove {
			childMovedEvent -= value;
		}
	}

	public event System.EventHandler<ErrorEventArgs> Error;
	#endregion

	[MonoPInvokeCallbackAttribute(typeof(onValueChangedEventHandler))]
	static void onValueChanged(IntPtr snapshot, IntPtr reference) {
		if (snapshot == IntPtr.Zero) {
			return;
		}
		QueryiOSImpl target = (QueryiOSImpl)GetCachedInstance(reference);
		if (target == null) {
			Debug.Log ("unable to locate target for value callback!");
			return;
		}
		EventHandler<ChangedEventArgs> handler = target.valueUpdatedEvent;
		if (handler != null)
		{
			handler(target, new ChangedEventArgs() { DataSnapshot = new DataSnapshotiOSImpl(snapshot) });
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(onValueChangedEventHandler))]
	static void onChildAdded(IntPtr snapshot, IntPtr reference) {
		if (snapshot == IntPtr.Zero) {
			return;
		}
		QueryiOSImpl target = (QueryiOSImpl)GetCachedInstance(reference);
		if (target == null) {
			Debug.Log ("unable to locate target for child callback!");
			return;
		}
		EventHandler<ChangedEventArgs> handler = target.childAddedEvent;
		if (handler != null)
		{
			handler(target, new ChangedEventArgs() { DataSnapshot = new DataSnapshotiOSImpl(snapshot) });
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(onValueChangedEventHandler))]
	static void onChildRemoved(IntPtr snapshot, IntPtr reference) {
		if (snapshot == IntPtr.Zero) {
			return;
		}
		QueryiOSImpl target = (QueryiOSImpl)GetCachedInstance(reference);
		if (target == null) {
			Debug.Log ("unable to locate target for child callback!");
			return;
		}
		EventHandler<ChangedEventArgs> handler = target.childRemovedEvent;
		if (handler != null)
		{
			handler(target, new ChangedEventArgs() { DataSnapshot = new DataSnapshotiOSImpl(snapshot) });
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(onValueChangedEventHandler))]
	static void onChildChanged(IntPtr snapshot, IntPtr reference) {
		if (snapshot == IntPtr.Zero) {
			return;
		}
		QueryiOSImpl target = (QueryiOSImpl)GetCachedInstance(reference);
		if (target == null) {
			Debug.Log ("unable to locate target for child callback!");
			return;
		}
		EventHandler<ChangedEventArgs> handler = target.childChangedEvent;
		if (handler != null)
		{
			handler(target, new ChangedEventArgs() { DataSnapshot = new DataSnapshotiOSImpl(snapshot) });
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(onValueChangedEventHandler))]
	static void onChildMoved(IntPtr snapshot, IntPtr reference) {
		if (snapshot == IntPtr.Zero) {
			return;
		}
		QueryiOSImpl target = (QueryiOSImpl)GetCachedInstance(reference);
		if (target == null) {
			Debug.Log ("unable to locate target for child callback!");
			return;
		}
		EventHandler<ChangedEventArgs> handler = target.childMovedEvent;
		if (handler != null)
		{
			handler(target, new ChangedEventArgs() { DataSnapshot = new DataSnapshotiOSImpl(snapshot) });
		}
	}
	
}
#endif